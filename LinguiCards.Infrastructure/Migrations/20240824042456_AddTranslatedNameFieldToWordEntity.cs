using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinguiCards.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTranslatedNameFieldToWordEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TranslatedName",
                table: "Words",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TranslatedName",
                table: "Words");
        }
    }
}
