using OfficeOpenXml;
using System.IO;


namespace MyFreeFormForm.Helpers
{
    public class FileParser
    {


        // Placeholder for ParseExcelFile method
        public async Task<List<Dictionary<string, string>>> ParseExcelFile(IFormFile fileUpload)
        {
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
                        var header = worksheet.Cells[1, col].Value?.ToString().Trim();
                        headers.Add(header);
                    }

                    // Read data rows
                    for (int row = 2; row <= rowCount; row++) // Start at row 2 to skip header
                    {
                        var rowDict = new Dictionary<string, string>();
                        for (int col = 1; col <= colCount; col++)
                        {
                            var value = worksheet.Cells[row, col].Value?.ToString().Trim();
                            rowDict[headers[col - 1]] = value;
                        }
                        result.Add(rowDict);
                    }
                }
            }

            return result;
        }

        // Placeholder for ParseCsvFile method
        public async Task<List<Dictionary<string, string>>> ParseCsvFile(IFormFile fileUpload)
        {
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
                        }

                        result.Add(rowDict);
                    }
                }
            }
            return result;
        }

    }
}
