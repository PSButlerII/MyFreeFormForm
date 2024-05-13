using Microsoft.AspNetCore.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MyFreeFormForm.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyFreeFormForm.Helpers.Tests
{

    [TestClass()]
    public class FileParserTests
    {
        private readonly FileParser _fileParser;

        public FileParserTests(FileParser fileParser)
        {
            _fileParser = fileParser;
        }

        [TestMethod()]
        public void ParseCsvFileTest()
        {
            // Use the CSVBook1.csv file in the TestData folder for the csv file, or you can use this data here:
            // Company Name	Company Contact	ProductName	ProductLicense	ExpirationDate	License StartDate
            // HandOver Foot   Jim Mills   Archive Accelerator Restore restore 4 / 18 / 2025   4 / 20 / 2021
            // Joe's Fried Fish	Joe Jopinelli	Archive Accelerator Export	export	1/7/2026	1/9/2019
            // The data should be parsed into a list of dictionaries, where each dictionary represents a row in the csv file.
            // The dictionary should have the column name as the key and the value as the value.
            // The dictionary should have the following keys: Company Name, Company Contact, ProductName, ProductLicense, ExpirationDate, License StartDate
            // The values should be the values from the csv file.
            // The dictionary should have the following values for the first row:
            // Company Name: HandOver Foot
            // Company Contact: Jim Mills
            // ProductName: Archive Accelerator Restore
            // ProductLicense: restore
            // ExpirationDate: 4/18/2025
            // License StartDate: 4/20/2021
            // The dictionary should have the following values for the second row:
            // Company Name: Joe's Fried Fish
            // Company Contact: Joe Jopinelli
            // ProductName: Archive Accelerator Export
            // ProductLicense: export
            // ExpirationDate: 1/7/2026
            // License StartDate: 1/9/2019
            // Will need to check that values are the correct type, and if not, convert them to the correct type.
            // For example, the ExpirationDate and License StartDate should be DateTime objects.
            // The ExpirationDate for the first row should be 4/18/2025.
            // The License StartDate for the first row should be 4/20/2021.
            // ParseCsvFile requires a IFormFile object, so we will need to create a mock IFormFile object. We will need to create a mock IFormFile object that will return the file path of the csv file.
            // We will need to create a mock IFormFile object that will return the file path of the csv file.

            // Arrange
            var formFile = new FormFile(null, 0, 0, "CSVBook1.csv", "CSVBook1.csv");

            // Act
            var result = _fileParser.ParseCsvFile(formFile);
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Result.Count);

            // Assert
            var firstRow = result.Result[0];
            Assert.AreEqual("HandOver Foot", firstRow["Company Name"]);
            Assert.AreEqual("Jim Mills", firstRow["Company Contact"]);
            Assert.AreEqual("Archive Accelerator Restore", firstRow["ProductName"]);
            Assert.AreEqual("restore", firstRow["ProductLicense"]);
            Assert.AreEqual(new DateTime(2025, 4, 18), firstRow["ExpirationDate"]);
            Assert.AreEqual(new DateTime(2021, 4, 20), firstRow["License StartDate"]);

            var secondRow = result.Result[1];
            Assert.AreEqual("Joe's Fried Fish", secondRow["Company Name"]);
            Assert.AreEqual("Joe Jopinelli", secondRow["Company Contact"]);
            Assert.AreEqual("Archive Accelerator Export", secondRow["ProductName"]);
            Assert.AreEqual("export", secondRow["ProductLicense"]);
            Assert.AreEqual(new DateTime(2026, 1, 7), secondRow["ExpirationDate"]);
            Assert.AreEqual(new DateTime(2019, 1, 9), secondRow["License StartDate"]);

            // Cleanup

        }
    }
}