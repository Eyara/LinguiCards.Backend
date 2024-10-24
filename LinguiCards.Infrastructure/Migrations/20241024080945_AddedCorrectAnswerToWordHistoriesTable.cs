using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinguiCards.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedCorrectAnswerToWordHistoriesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CorrectAnswer",
                table: "WordChangeHistories",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CorrectAnswer",
                table: "WordChangeHistories");
        }
    }
}
