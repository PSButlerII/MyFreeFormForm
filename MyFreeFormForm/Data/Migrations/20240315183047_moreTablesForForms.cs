using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFreeFormForm.Data.Migrations
{
    /// <inheritdoc />
    public partial class moreTablesForForms : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FieldValue",
                table: "FormFields",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FieldValue",
                table: "FormFields");
        }
    }
}
