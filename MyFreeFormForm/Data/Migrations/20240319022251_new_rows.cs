using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFreeFormForm.Data.Migrations
{
    /// <inheritdoc />
    public partial class new_rows : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "Forms",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "FormNotes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<int>(
                name: "FieldOptions",
                table: "FormFields",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "Forms");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "FormNotes");

            migrationBuilder.AlterColumn<string>(
                name: "FieldOptions",
                table: "FormFields",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
