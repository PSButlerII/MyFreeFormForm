window.patterns = {

    text: { Pattern: "^[\\w\\s\\-.,()\\[\\]\\{\\}*\\&\\^%\\$#@!~`':;<>/\\\\+=|]*$", ErrorMessage: "Only letters, numbers, and the following characters are allowed: - . , ( ) [ ] { } * & ^ % $ # @ ! ~ ` ' : ; < > ? / \\ + = |" },
    email: { Pattern: "^[^@\\s]+@[^@\\s]+\\.[^@\\s]+$", ErrorMessage: "Please enter a valid email address." },
    number: { Pattern: "^\\d+$", ErrorMessage: "Please enter a number." },
    date: { Pattern: "^\\d{4}-\\d{2}-\\d{2}$", ErrorMessage: "Please enter a date in the format YYYY-MM-DD." },
    tel: { Pattern: "^\\d{3}-\\d{3}-\\d{4}$", ErrorMessage: "Please enter a phone number in the format XXX-XXX-XXXX." },
    url: { Pattern: "^https?:\\/\\/[\\w\\-]+(\\.[\\w\\-]+)+[/#?]?.*$", ErrorMessage: "Please enter a valid URL." },
    ipaddress: { Pattern: "^\\d{1,3}(\\.\\d{1,3}){3}$", ErrorMessage: "Please enter a valid IP address." },
    postalcode: { Pattern: "^\\d{5}(-\\d{4})?$", ErrorMessage: "Please enter a valid postal code, e.g., 12345 or 12345-6789." },
    country: { Pattern: "^[A-Z]{2}$", ErrorMessage: "Please enter a valid country code." },
    state: { Pattern: "^[A-Z]{2}$", ErrorMessage: "Please enter a valid state code." },
    address: { Pattern: "^[\\w\\s\\-.,]*$", ErrorMessage: "Only letters, numbers, and the following characters are allowed: - . ," },
    // Add more field types as needed
};

export function getPattern(fieldType) {
    // Normalize the fieldType to lowercase
    const key = fieldType.toLowerCase();
    return window.patterns[key] ? window.patterns[key].Pattern : '';
};

export function getErrorMessage(fieldType) {
    // Normalize the fieldType to lowercase
    const key = fieldType.toLowerCase();
    return window.patterns[key] ? window.patterns[key].ErrorMessage : '';
};

export function validateField(fieldType, value) {
    const pattern = window.getPattern(fieldType);
    const regex = new RegExp(pattern);
    if (!regex.test(value)) {
        return window.getErrorMessage(fieldType);
    }
    return "Valid";
};

