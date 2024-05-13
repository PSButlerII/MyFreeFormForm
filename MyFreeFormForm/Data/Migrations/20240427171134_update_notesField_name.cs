using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyFreeFormForm.Data.Migrations
{
    /// <inheritdoc />
    public partial class update_notesField_name : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Rename the column from Note to Notes if it exists in the FormNotes table and is not name Notes.  Is this possible?
            migrationBuilder.RenameColumn(
                                name: "Note",
                                table: "FormNotes",
                                newName: "Notes");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
