using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoctorsManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddPatientRegisteredAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "RegisteredAt",
                table: "Patients",
                type: "datetime2",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RegisteredAt",
                table: "Patients");
        }
    }
}
