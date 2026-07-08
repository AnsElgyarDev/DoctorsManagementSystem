using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoctorsManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class Add_SurgeryNotes_Entity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SurgeryNotes",
                table: "Prescriptions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SurgeryNotes",
                table: "Prescriptions");
        }
    }
}
