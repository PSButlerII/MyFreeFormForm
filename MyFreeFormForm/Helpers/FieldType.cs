using Microsoft.AspNetCore.Identity;

namespace MyFreeFormForm.Helpers
{
    /// <summary>
    /// These are the field types for input fields that are supported by the form builder
    /// </summary>
    public enum FieldType
    {
        Text,
        Email,
        Number,
        Date,
        tel,
        url,
        //TODO: will be adding "file" and "password" in the future but need to figure out how to handle those. Specifically, file uploads.
        //TODO: will be adding "select" and "radio" in the future but need to figure out how to handle those. Specifically, file uploads.
        
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
