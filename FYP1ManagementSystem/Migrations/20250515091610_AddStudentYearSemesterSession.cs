using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FYP1ManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentYearSemesterSession : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AcademicSession",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Semester",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Year",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcademicSession",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Semester",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "AspNetUsers");
        }
    }
}
