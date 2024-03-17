namespace MyFreeFormForm.Helpers
{
    public enum FieldType
    {
        Text,
        Email,
        Number,
        Date,
        // Add other field types as necessary
    }

    public enum FieldOptions  // JSON or delimited list for options
    {
        JSON,
        Comma,
        Pipe,
        Semicolon,
        Colon,
        Space,
    }

}
