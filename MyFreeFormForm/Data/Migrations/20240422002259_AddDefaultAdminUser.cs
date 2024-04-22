using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFreeFormForm.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultAdminUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var adminUserId = Guid.NewGuid().ToString();
            var adminRoleId = "a85197de-2346-492e-861e-08b0370b485f";
            var passwordHash = "AQAAAAIAAYagAAAAEGsfN2hrV34XphOUw8jAWFbklaJ3tWQQbGWxdrYo5C2v3iMJN4uI6BgAF1PMUrbZvg=="; // Ensure this is securely generated
            var securityStamp = Guid.NewGuid().ToString();

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "UserName", "NormalizedUserName", "Email", "NormalizedEmail", "EmailConfirmed", "PasswordHash", "SecurityStamp", "ConcurrencyStamp","PhoneNumberConfirmed", "TwoFactorEnabled", "LockoutEnabled", "AccessFailedCount","City","FirstName","LastName","State","Zip" },
                values: new object[] { adminUserId, "admin", "ADMIN", "email@email.com", "EMAIL@EMAIL.COM", true, passwordHash, securityStamp, Guid.NewGuid().ToString(),false,false,true,0,"Atlanta","Admin","User","Ga","303310"}
            );

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "UserId", "RoleId" },
                values: new object[] { adminUserId, adminRoleId }
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Optional: Add removal logic for the user, role, and user-role link if necessary
            migrationBuilder.Sql("DELETE FROM AspNetUserRoles WHERE UserId IN (SELECT Id FROM AspNetUsers WHERE UserName = 'admin')");
            migrationBuilder.Sql("DELETE FROM AspNetUsers WHERE UserName = 'admin'");
            migrationBuilder.Sql("DELETE FROM AspNetRoles WHERE Name = 'Admin'");
        }
    }
}
