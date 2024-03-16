using OfficeOpenXml;
using System.IO;
using System.Diagnostics;


namespace MyFreeFormForm.Helpers
{
    public class FileParser(ILogger<FileParser> logger)
    {

        private readonly ILogger<FileParser> _logger = logger;

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
        public async Task<List<Dictionary<string, string>>> ParseCsvFile(IFormFile fileUpload)
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
                    var headers = headerLine.Split(',');

                    while (!reader.EndOfStream)
                    {
                        var line = await reader.ReadLineAsync();
                        var values = line.Split(',');

                        var rowDict = new Dictionary<string, string>();
                        for (int i = 0; i < headers.Length; i++)
                        {
                            rowDict[headers[i].Trim()] = values[i].Trim();
                            counter++;
                        }

                        result.Add(rowDict);
                    }
                }
            }
            _logger.LogInformation("CSV file parsing completed. Total items processed: {RowCount}", counter);
            stopwatch.Stop(); // Stop the stopwatch
            _logger.LogInformation($"CSV file parsing completed in {stopwatch.ElapsedMilliseconds} ms");

            return result;
        }

    }
}
