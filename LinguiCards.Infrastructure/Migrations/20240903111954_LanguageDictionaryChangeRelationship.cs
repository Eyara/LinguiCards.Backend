using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinguiCards.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class LanguageDictionaryChangeRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Languages_LanguageDictionaries_LanguageDictionaryId",
                table: "Languages");

            migrationBuilder.DropIndex(
                name: "IX_Languages_LanguageDictionaryId",
                table: "Languages");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_LanguageDictionaryId",
                table: "Languages",
                column: "LanguageDictionaryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Languages_LanguageDictionaries_LanguageDictionaryId",
                table: "Languages",
                column: "LanguageDictionaryId",
                principalTable: "LanguageDictionaries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Languages_LanguageDictionaries_LanguageDictionaryId",
                table: "Languages");

            migrationBuilder.DropIndex(
                name: "IX_Languages_LanguageDictionaryId",
                table: "Languages");

            migrationBuilder.CreateIndex(
                name: "IX_Languages_LanguageDictionaryId",
                table: "Languages",
                column: "LanguageDictionaryId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Languages_LanguageDictionaries_LanguageDictionaryId",
                table: "Languages",
                column: "LanguageDictionaryId",
                principalTable: "LanguageDictionaries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
