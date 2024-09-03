using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LinguiCards.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLanguageDictionary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FlagUrl",
                table: "Languages");

            migrationBuilder.AddColumn<int>(
                name: "LanguageDictionaryId",
                table: "Languages",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "LanguageDictionaries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Url = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LanguageDictionaries", x => x.Id);
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Languages_LanguageDictionaries_LanguageDictionaryId",
                table: "Languages");

            migrationBuilder.DropTable(
                name: "LanguageDictionaries");

            migrationBuilder.DropIndex(
                name: "IX_Languages_LanguageDictionaryId",
                table: "Languages");

            migrationBuilder.DropColumn(
                name: "LanguageDictionaryId",
                table: "Languages");

            migrationBuilder.AddColumn<string>(
                name: "FlagUrl",
                table: "Languages",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
