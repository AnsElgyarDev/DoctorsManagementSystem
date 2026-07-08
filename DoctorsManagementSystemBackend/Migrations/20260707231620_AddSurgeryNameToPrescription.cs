using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoctorsManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddSurgeryNameToPrescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SurgeryName",
                table: "Prescriptions",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SurgeryName",
                table: "Prescriptions");
        }
    }
}
