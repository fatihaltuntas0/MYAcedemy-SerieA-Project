using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SerieA.API.Migrations
{
    /// <inheritdoc />
    public partial class MigUpdateRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MatchCards_Teams_TeamId",
                table: "MatchCards");

            migrationBuilder.DropForeignKey(
                name: "FK_MatchGoals_Teams_TeamId",
                table: "MatchGoals");

            migrationBuilder.DropForeignKey(
                name: "FK_Substitutions_Teams_TeamId",
                table: "Substitutions");

            migrationBuilder.AddForeignKey(
                name: "FK_MatchCards_Teams_TeamId",
                table: "MatchCards",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MatchGoals_Teams_TeamId",
                table: "MatchGoals",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Substitutions_Teams_TeamId",
                table: "Substitutions",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MatchCards_Teams_TeamId",
                table: "MatchCards");

            migrationBuilder.DropForeignKey(
                name: "FK_MatchGoals_Teams_TeamId",
                table: "MatchGoals");

            migrationBuilder.DropForeignKey(
                name: "FK_Substitutions_Teams_TeamId",
                table: "Substitutions");

            migrationBuilder.AddForeignKey(
                name: "FK_MatchCards_Teams_TeamId",
                table: "MatchCards",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MatchGoals_Teams_TeamId",
                table: "MatchGoals",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Substitutions_Teams_TeamId",
                table: "Substitutions",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
