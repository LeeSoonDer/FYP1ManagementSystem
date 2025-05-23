using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FYP1ManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddSupervisorCommentToProposal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SupervisorComments",
                table: "Proposals",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SupervisorComments",
                table: "Proposals");
        }
    }
}
