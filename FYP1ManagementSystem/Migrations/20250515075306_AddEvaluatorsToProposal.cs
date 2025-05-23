using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FYP1ManagementSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddEvaluatorsToProposal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Evaluator1Id",
                table: "Proposals",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Evaluator2Id",
                table: "Proposals",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Proposals_Evaluator1Id",
                table: "Proposals",
                column: "Evaluator1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Proposals_Evaluator2Id",
                table: "Proposals",
                column: "Evaluator2Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Proposals_AspNetUsers_Evaluator1Id",
                table: "Proposals",
                column: "Evaluator1Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Proposals_AspNetUsers_Evaluator2Id",
                table: "Proposals",
                column: "Evaluator2Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Proposals_AspNetUsers_Evaluator1Id",
                table: "Proposals");

            migrationBuilder.DropForeignKey(
                name: "FK_Proposals_AspNetUsers_Evaluator2Id",
                table: "Proposals");

            migrationBuilder.DropIndex(
                name: "IX_Proposals_Evaluator1Id",
                table: "Proposals");

            migrationBuilder.DropIndex(
                name: "IX_Proposals_Evaluator2Id",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "Evaluator1Id",
                table: "Proposals");

            migrationBuilder.DropColumn(
                name: "Evaluator2Id",
                table: "Proposals");
        }
    }
}
