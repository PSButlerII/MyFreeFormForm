using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFreeFormForm.Data.Migrations
{
    /// <inheritdoc />
    public partial class parentId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ParentFormId",
                table: "Forms",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParentFormId",
                table: "Forms");
        }
    }
}
