using System.Collections.Generic;

namespace MyFreeFormForm.Helpers
{
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
            [FieldType.PostalCode] = (@"^\d{5}(-\d{4})?$", "Please enter a valid postal code."),
            [FieldType.Country] = (@"^[A-Z]{2}$", "Please enter a valid country code."),
            [FieldType.State] = (@"^[A-Z]{2}$", "Please enter a valid state code."),
            [FieldType.Address] = (@"^[\w\s\-\.,]*$", "Only letters, numbers, and the following characters are allowed: - . ,"),
            // Add more field types as needed
        };

        public static string GetPattern(FieldType fieldType) => patterns.TryGetValue(fieldType, out var value) ? value.Pattern : string.Empty;

        public static string GetErrorMessage(FieldType fieldType) => patterns.TryGetValue(fieldType, out var value) ? value.ErrorMessage : string.Empty;
    }
}
