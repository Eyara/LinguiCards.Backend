using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinguiCards.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLanguageIdToTranslationEvaluationHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LanguageId",
                table: "TranslationEvaluationHistories",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LanguageId",
                table: "GrammarTaskHistories",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LanguageId",
                table: "TranslationEvaluationHistories");

            migrationBuilder.DropColumn(
                name: "LanguageId",
                table: "GrammarTaskHistories");
        }
    }
}
