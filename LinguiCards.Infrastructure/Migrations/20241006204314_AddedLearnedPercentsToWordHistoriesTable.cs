using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinguiCards.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedLearnedPercentsToWordHistoriesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "ActiveLearned",
                table: "WordChangeHistories",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "PassiveLearned",
                table: "WordChangeHistories",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "VocabularyType",
                table: "WordChangeHistories",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActiveLearned",
                table: "WordChangeHistories");

            migrationBuilder.DropColumn(
                name: "PassiveLearned",
                table: "WordChangeHistories");

            migrationBuilder.DropColumn(
                name: "VocabularyType",
                table: "WordChangeHistories");
        }
    }
}
