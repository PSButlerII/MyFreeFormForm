using System.Collections.Generic;

namespace MyFreeFormForm.Helpers
{
    // If you want to use this class to validate form fields, you will need to modify the form builder to use these patterns. You can do this by adding a new property to the Field class that stores the FieldType and then using that property to determine which pattern to use, like this:
    // var pattern = FieldTypePatterns.GetPattern(field.FieldType);
    // var errorMessage = FieldTypePatterns.GetErrorMessage(field.FieldType);
    // Then you can use the pattern and errorMessage to validate the field value, by adding a new method to the Field class that validates the field value, like this:
    // public bool IsValid() => Regex.IsMatch(Value, pattern);
    // You can also add a new method to the Field class that returns the error message, like this:
    // public string GetErrorMessage() => errorMessage;
    // You can then use these methods to validate the form fields in the form builder, like this:
    // if (!field.IsValid())
    // {
    //     // Display error message
    //     var errorMessage = field.GetErrorMessage();
    // }
    // You can also use these methods to validate the form fields in the search service, like this:
    // if (!field.IsValid())
    // {
    //     // Display error message
    //     var errorMessage = field.GetErrorMessage();
    // }
    // You can also use these methods to validate the form fields in the search controller, like this:
    // if (!field.IsValid())
    // {
    //     // Display error message
    //     var errorMessage = field.GetErrorMessage();
    // }


    public static class FieldTypePatterns
    {
        private static readonly Dictionary<FieldType, (string Pattern, string ErrorMessage)> patterns = new Dictionary<FieldType, (string Pattern, string ErrorMessage)>
        {
            [FieldType.Text] = (@"^[\w\s\-\.,\(\)\[\]\{\}\*&\^%$#@!~`':;<>\?/\\+=|]*$", "Only letters, numbers, and the following characters are allowed: - . , ( ) [ ] { } * & ^ % $ # @ ! ~ ` ' : ; < > ? / \\ + = |"),
            [FieldType.Email] = (@"^[^@\s]+@[^@\s]+\.[^@\s]+$", "Please enter a valid email address."),
            [FieldType.Number] = (@"^\d+$", "Please enter a number."),
            [FieldType.Date] = (@"^\d{4}-\d{2}-\d{2}$", "Please enter a date in the format YYYY-MM-DD."),
            [FieldType.tel] = (@"^\d{3}-\d{3}-\d{4}$", "Please enter a phone number in the format XXX-XXX-XXXX."),
            [FieldType.url] = (@"^https?:\/\/[\w\-]+(\.[\w\-]+)+[/#?]?.*$", "Please enter a valid URL."),
            [FieldType.IpAddress] = (@"^\d{1,3}(\.\d{1,3}){3}$", "Please enter a valid IP address."),
            [FieldType.PostalCode] = (@"^\d{5}(-\d{4})?$", "Please enter a valid postal code, e.g., 12345 or 12345-6789."),
            [FieldType.Country] = (@"^[A-Z]{2}$", "Please enter a valid country code."),
            [FieldType.State] = (@"^[A-Z]{2}$", "Please enter a valid state code."),
            [FieldType.Address] = (@"^[\w\s\-\.,]*$", "Only letters, numbers, and the following characters are allowed: - . ,"),
            // Add more field types as needed
        };

        public static string GetPattern(FieldType fieldType) => patterns.TryGetValue(fieldType, out var value) ? value.Pattern : string.Empty;

        public static string GetErrorMessage(FieldType fieldType) => patterns.TryGetValue(fieldType, out var value) ? value.ErrorMessage : string.Empty;
    }
}
