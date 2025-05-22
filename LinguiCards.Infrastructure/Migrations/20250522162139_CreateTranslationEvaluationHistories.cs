using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace LinguiCards.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateTranslationEvaluationHistories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TranslationEvaluationHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    OriginalText = table.Column<string>(type: "text", nullable: false),
                    UserTranslation = table.Column<string>(type: "text", nullable: false),
                    CorrectTranslation = table.Column<string>(type: "text", nullable: false),
                    Accuracy = table.Column<int>(type: "integer", nullable: false),
                    MinorIssues = table.Column<string>(type: "text", nullable: false),
                    Errors = table.Column<string>(type: "text", nullable: false),
                    CriticalErrors = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TranslationEvaluationHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TranslationEvaluationHistories_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TranslationEvaluationHistories_UserId",
                table: "TranslationEvaluationHistories",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TranslationEvaluationHistories");
        }
    }
}
