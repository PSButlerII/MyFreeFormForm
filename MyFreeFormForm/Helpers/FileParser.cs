using OfficeOpenXml;
using System.IO;
using System.Diagnostics;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using System.Globalization;
using CsvHelper.Configuration;
using CsvHelper;

namespace MyFreeFormForm.Helpers
{
    public class FileParser(ILogger<FileParser> logger)
    {

        private readonly ILogger<FileParser> _logger = logger;

        private readonly string[] dateFormats =
        [
            
        "MM/dd/yyyy", "dd/MM/yyyy", "yyyy-MM-dd", // Common international formats
        "MM-dd-yyyy", "dd-MM-yyyy", "yyyy/MM/dd", // Include variations with dashes and slashes
        "d/M/yyyy", "d.MM.yyyy", // Less common formats including those without leading zeros
        "M/dd/yyyy", "M-dd-yyyy" // Formats with single digit month part   

        ];

        // Placeholder for ParseExcelFile method
        public async Task<List<Dictionary<string, string>>> ParseExcelFile(IFormFile fileUpload)
        {
            var stopwatch = Stopwatch.StartNew(); // Initialize and start the stopwatch
            var counter = 0;
            _logger.LogInformation("Parsing Excel file...");
            var result = new List<Dictionary<string, string>>();

            // Ensure EPPlus license context is set (required for EPPlus 5.x and above)
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var stream = new MemoryStream())
            {
                await fileUpload.CopyToAsync(stream);
                stream.Position = 0; // Reset stream position to the beginning

                using (var package = new ExcelPackage(stream))
                {
                    // Get the first worksheet
                    ExcelWorksheet worksheet = package.Workbook.Worksheets.FirstOrDefault();
                    if (worksheet == null) return result; // Return empty if no worksheet is found

                    // Assume first row is the header and columns are the fields
                    int colCount = worksheet.Dimension.End.Column;  // Get the number of columns
                    int rowCount = worksheet.Dimension.End.Row;     // Get the number of rows

                    // Read headers
                    var headers = new List<string>();
                    for (int col = 1; col <= colCount; col++)
                    {
                        _logger.LogInformation("Reading header from column {Column}", col);
                        _logger.LogInformation("Value: {Value}", worksheet.Cells[1, col].Value?.ToString().Trim());
                        var header = worksheet.Cells[1, col].Value?.ToString().Trim();
                        headers.Add(header);
                        counter++;
                    }

                    // Read data rows
                    for (int row = 2; row <= rowCount; row++) // Start at row 2 to skip header
                    {
                        var rowDict = new Dictionary<string, string>();
                        for (int col = 1; col <= colCount; col++)
                        {
                            _logger.LogInformation("Reading data from row {Row}, column {Column}", row, col);
                            _logger.LogInformation("Value: {Value}", worksheet.Cells[row, col].Value?.ToString().Trim());
                            var value = worksheet.Cells[row, col].Value?.ToString().Trim();
                           //check for date and format it yyyy-MM-dd
                           if (DateTime.TryParse(value, out DateTime dateValue))
                           {
                               value = dateValue.ToString("yyyy-MM-dd");
                           }
                            rowDict[headers[col - 1]] = value;
                            counter++;
                        }
                        result.Add(rowDict);
                    }
                }
            }
            _logger.LogInformation("Excel file parsing completed. Total items processed: {RowCount}", counter);
            stopwatch.Stop(); // Stop the stopwatch
            _logger.LogInformation($"Excel file parsing completed in {stopwatch.ElapsedMilliseconds} ms");
            return result;
        }

        // Placeholder for ParseCsvFile method
        /*        public async Task<List<Dictionary<string, string>>> ParseCsvFile(IFormFile fileUpload)
                {
                    var stopwatch = Stopwatch.StartNew(); // Initialize and start the stopwatch
                    var counter = 0;
                    _logger.LogInformation("Parsing CSV file...");

                    var result = new List<Dictionary<string, string>>();
                    using (var stream = new MemoryStream())
                    {
                        await fileUpload.CopyToAsync(stream);
                        stream.Position = 0; // Reset stream position to the beginning
                        using (var reader = new StreamReader(stream))
                        {
                            string headerLine = await reader.ReadLineAsync();
                            // Check to see what the delimiter is
                            string delimiter = headerLine.Contains(',') ? "," : headerLine.Contains(';') ? ";" : headerLine.Contains('\t') ? "\t" : headerLine.Contains('|') ? "|" : headerLine.Contains(':') ? ":" : null;
                            if (delimiter == null)
                            {
                                _logger.LogError("Unsupported delimiter");
                                return result;
                            }
                            _logger.LogInformation("Delimiter: {Delimiter}", delimiter);

                            var headers = headerLine.Split(delimiter); // Split the header line using the delimiter

                            while (!reader.EndOfStream)
                            {
                                var line = await reader.ReadLineAsync();
                                var values = line.Split(delimiter);

                                var rowDict = new Dictionary<string, string>();
                                for (int i = 0; i < headers.Length; i++)
                                {
                                    rowDict[headers[i].Trim()] = values[i].Trim();
                                    counter++;
                                }

                                result.Add(rowDict);
                            }
                            // Add the delimiter to the result
                            //result.Add(new Dictionary<string, string> { { "Delimiter", delimiter } });
                        }
                    }
                    _logger.LogInformation("CSV file parsing completed. Total items processed: {RowCount}", counter);
                    stopwatch.Stop(); // Stop the stopwatch
                    _logger.LogInformation($"CSV file parsing completed in {stopwatch.ElapsedMilliseconds} ms");

                    return result;
                }*/

        public async Task<List<Dictionary<string, object>>> ParseCsvFile(IFormFile fileUpload)
        {
            var stopwatch = Stopwatch.StartNew();
            var counter = 0;
            _logger.LogInformation("Parsing CSV file...");

            var result = new List<Dictionary<string, object>>();
            using (var stream = new MemoryStream())
            {
                await fileUpload.CopyToAsync(stream);
                stream.Position = 0;
                using (var reader = new StreamReader(stream))
                {
                    string headerLine = await reader.ReadLineAsync();
                    string delimiter = DetectDelimiter(headerLine);
                    if (delimiter == null)
                    {
                        _logger.LogError("Unsupported delimiter");
                        return result;
                    }

                    var headers = headerLine.Split(new string[] { delimiter }, StringSplitOptions.None);
                    var headerTypes = new Type[headers.Length]; // Array to store the inferred types for each column

                    // First, read all lines to detect types
                    var lines = new List<string[]>();
                    while (!reader.EndOfStream)
                    {
                        var line = await reader.ReadLineAsync();
                        var values = line.Split(new string[] { delimiter }, StringSplitOptions.None);
                        lines.Add(values);
                        UpdateColumnType(headerTypes, values); // Method to infer and update column types
                    }

                    // Process lines and convert types
                    foreach (var values in lines)
                    {
                        var rowDict = new Dictionary<string, object>();
                        for (int i = 0; i < headers.Length; i++)
                        {
                            try
                            {
                                object convertedValue = values[i].Trim();
                                if (headerTypes[i] == typeof(DateTime) && DateTime.TryParse(convertedValue.ToString(), out DateTime dateValue))
                                {
                                    convertedValue = dateValue.ToString("yyyy-MM-dd"); // Here you may format it as needed
                                }
                                else if (headerTypes[i] != typeof(string))
                                {
                                    convertedValue = Convert.ChangeType(convertedValue, headerTypes[i], CultureInfo.InvariantCulture);
                                }

                                rowDict[headers[i].Trim()] = convertedValue;
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError("Error converting type for column {Column} with value {Value}: {Exception}", headers[i], values[i], ex);
                            }
                        }
                        result.Add(rowDict);
                    }

                }
            }
            _logger.LogInformation("CSV file parsing completed. Total items processed: {RowCount}", counter);
            stopwatch.Stop();
            _logger.LogInformation($"CSV file parsing completed in {stopwatch.ElapsedMilliseconds} ms");

            return result;
        }

        public void UpdateColumnType(Type[] headerTypes, string[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                string trimmedValue = values[i].Trim();

                // Check if the value is null or empty
                if (string.IsNullOrEmpty(trimmedValue))
                {
                    headerTypes[i] = typeof(string);
                    continue;
                }

                // Attempt to parse the date using multiple formats
                bool isDate = IsDateTime(trimmedValue, out DateTime parsedDate);
                //bool isDate = DateTime.TryParse(trimmedValue, out DateTime parsedDate);
                if (isDate)
                {
                    headerTypes[i] = typeof(DateTime);
                    _logger.LogInformation($"Column {i} detected as DateTime with value {trimmedValue}, parsed as {parsedDate.ToString("yyyy-MM-dd")}");
                }
                else if (int.TryParse(trimmedValue, out _))
                {
                    headerTypes[i] = typeof(int);
                    _logger.LogInformation($"Column {i} detected as Integer with value {trimmedValue}");
                }
                else if (double.TryParse(trimmedValue, out _))
                {
                    headerTypes[i] = typeof(double);
                    _logger.LogInformation($"Column {i} detected as Double with value {trimmedValue}");
                }
                else
                {
                    headerTypes[i] = typeof(string);
                    _logger.LogInformation($"Column {i} detected as String with value {trimmedValue}");
                }
            }
        }

        // Helper method that looks at a string and determines if its dateTime.  note that the time may not be included in the date
        private bool IsDateTime(string value, out DateTime parsedDate)
        {
            if (DateTime.TryParse(value, out DateTime dateValue))
            {
                parsedDate = dateValue;
                return true;
            }

            foreach (var format in dateFormats)
            {
                if (DateTime.TryParseExact(value, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
                {
                    parsedDate = date;
                    return true;
                }
            }

            parsedDate = DateTime.MinValue;
            return false;
        }

        private string? DetectDelimiter(string? headerLine)
        {
            if (headerLine == null) return null;

            // Check for common delimiters
            if (headerLine.Contains(',')) return ",";
            if (headerLine.Contains(';')) return ";";
            if (headerLine.Contains('\t')) return "\t";
            if (headerLine.Contains('|')) return "|";
            if (headerLine.Contains(':')) return ":";

            return null;
        }

    }
}
