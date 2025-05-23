using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FYP1ManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddDomainToSupervisor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Domain",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Domain",
                table: "AspNetUsers");
        }
    }
}
