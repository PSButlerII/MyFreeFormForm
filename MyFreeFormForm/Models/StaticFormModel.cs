namespace MyFreeFormForm.Models
{
    public class StaticFormModel
    {
        public string CompanyName { get; set; }
        public string ContactFirstName { get; set; }
        public string ContactLastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public List<string> ProductName { get; set; }
        public string ProductLicense { get; set; }
        public string ProductVersion { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime LicenseExpirationDate { get; set; }
        
        // Define other fields as necessary
    }

}
