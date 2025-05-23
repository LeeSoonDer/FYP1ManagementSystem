using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FYP1ManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddProposalFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Evaluator1Comment",
                table: "Proposals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Evaluator2Comment",
                table: "Proposals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Proposals",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Evaluator1Comment",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "Evaluator2Comment",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Proposals");
        }
    }
}
