using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinguiCards.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DivideVocabularyToPassiveAndActive : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LearnedPercent",
                table: "Words",
                newName: "PassiveLearnedPercent");

            migrationBuilder.AddColumn<double>(
                name: "ActiveLearnedPercent",
                table: "Words",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActiveLearnedPercent",
                table: "Words");

            migrationBuilder.RenameColumn(
                name: "PassiveLearnedPercent",
                table: "Words",
                newName: "LearnedPercent");
        }
    }
}
