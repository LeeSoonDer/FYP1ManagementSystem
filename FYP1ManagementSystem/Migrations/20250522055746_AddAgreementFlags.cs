using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FYP1ManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddAgreementFlags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsStudentAgreed",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsSupervisorAgreed",
                table: "AspNetUsers",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsStudentAgreed",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "IsSupervisorAgreed",
                table: "AspNetUsers");
        }
    }
}
