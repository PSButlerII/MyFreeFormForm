using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFreeFormForm.Data.Migrations
{
    /// <inheritdoc />
    public partial class seed_Roles : Migration
    {
        private string DevRoleId = Guid.NewGuid().ToString();
        private string TesterRoleId = Guid.NewGuid().ToString();
        private string ResearchTechRoleId = Guid.NewGuid().ToString();
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
            table: "AspNetRoles",
            columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
            values: new object[] { "c073f42c-79e8-41a6-a5d7-0ed41ae7aca0", "df8961da-91f7-4729-a4aa-312b6fcd7c8f", "User", "USER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "a85197de-2346-492e-861e-08b0370b485f", "27e54b6b-7578-4229-8a3e-7a5a5651df3b", "Admin", "ADMIN" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "FA307A77-8548-4FDC-8952-4B1782E21870", "8D0B7B44-B2D7-4079-869D-E98F0CE56DE8", "Manager", "MANAGER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
            values: new object[] { $"{DevRoleId}",  "9D7908A5-1C19-4E53-A617-0BAC7344C996","Developer", "DEVELOPER"
            });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { $"{TesterRoleId}", "8A960E78-5A30-4E40-91B3-3B71278FB556", "Tester", "TESTER" });

            migrationBuilder.InsertData(
                 table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { $"{ResearchTechRoleId}", "F45A509A-297D-4C88-8E42-50EC592D3114", "ResearchTechnician", "RESEARCHTECHNICIAN" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
