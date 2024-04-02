# My FreeForm Form Application

Welcome to My FreeForm Form, an ASP.NET Core application designed for dynamic form creation and management. This application leverages the power of ASP.NET Core, Entity Framework, and modern web development practices to provide a comprehensive solution for managing forms and integrating various data sources.

## Features

- **Dynamic Form Creation**: Easily design and create customizable forms to fit your specific needs.
- **Form Management**: View, edit, and manage all your forms in one place.
- **Data Integration**: Upload and integrate data into your forms with ease.
- **Markdown Support**: Includes support for rendering Markdown content within the application.

## Getting Started

These instructions will get you a copy of the project up and running on your local machine for development and testing purposes.

### Prerequisites

- [.NET 5.0 SDK or later](https://dotnet.microsoft.com/download)
- [Visual Studio 2019 or later](https://visualstudio.microsoft.com/downloads/) with the ASP.NET and web development workload installed
- SQL Server (LocalDB or Express)

### Installation

1. **Clone the Repository**

```bash
git clone https://yourrepositoryurl.git
```

2. ** Navigate to the Project Directory**
```bash
cd MyFreeFormForm
```

3. **Restore NuGet Packages**
```bash
dotnet restore
```
4. **Update the Database**
Make sure your connection strings in appsettings.json are correctly set up for your SQL Server instance, then update the database.
```bash
dotnet ef database update
```
6. **Run the Application**
```bash
dotnet run
```

Navigate to https://localhost:5001 (or the port specified in your environment) to view the application.

### Usage
- **Creating a Form**: Navigate to Forms/dynamic to start creating a dynamic form.
- **Managing Forms**: Access /Forms/Manage to view and edit existing forms.
- **Uploading Data**: Use /Forms/UploadFile to upload data files that can be integrated with your forms.

### Contributing
Please read CONTRIBUTING.md for details on our code of conduct, and the process for submitting pull requests to us.

### Versioning
We use SemVer for versioning. For the versions available, see the tags on this repository.

### Authors
Your Name - Initial work - YourUsername
See also the list of contributors who participated in this project.

### License
This project is licensed under the MIT License - see the LICENSE.md file for details.

### Acknowledgments
Hat tip to anyone whose code was used
Inspiration
etc.


Remember to replace placeholders like `https://yourrepositoryurl.git`, `[YourUsername]`, and `"Your Name"` with your actual repository URL, GitHub username, and name. You may also want to adjust sections according to the specific features and technologies your project uses.
