using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinguiCards.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedGrammarTaskTypeDictionary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           migrationBuilder.InsertData(
                table: "GrammarTaskTypeDictionary",
                columns: new[] { "Id", "Name", "Description" },
                values: new object[,]
                {
                    { 1, "fill_blank", "Вставь пропущенное слово" },
                    { 2, "multiple_choice", "Выбери правильную форму" },
                    { 3, "rewrite", "Перепиши с правильной грамматикой" },
                    { 4, "transform", "Преобразуй предложение" },
                    { 5, "identify_form", "Определи грамматическую форму" },
                    { 6, "match_pairs", "Соотнеси элементы" },
                    { 7, "word_order", "Расставь слова в правильном порядке" },
                    { 8, "select_ending", "Выбери правильное окончание" },
                    { 9, "table_fill", "Заполни таблицу форм" },
                    { 10, "find_error", "Найди ошибку" },
                    { 11, "syntactic_role", "Определи синтаксическую роль" },
                    { 12, "preposition_choice", "Выбери правильный предлог" },
                    { 13, "contextual_form", "Поставь слово в нужную форму по контексту" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            for (int i = 1; i <= 13; i++)
            {
                migrationBuilder.DeleteData(
                    table: "GrammarTaskTypeDictionary",
                    keyColumn: "Id",
                    keyValue: i);
            }
        }
    }
}
