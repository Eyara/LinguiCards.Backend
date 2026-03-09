using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LinguiCards.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSrsFieldsToWord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "ActiveEaseFactor",
                table: "Words",
                type: "double precision",
                nullable: false,
                defaultValue: 2.5);

            migrationBuilder.AddColumn<int>(
                name: "ActiveIntervalDays",
                table: "Words",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "ActiveNextReviewDate",
                table: "Words",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ActiveRepetitionCount",
                table: "Words",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "PassiveEaseFactor",
                table: "Words",
                type: "double precision",
                nullable: false,
                defaultValue: 2.5);

            migrationBuilder.AddColumn<int>(
                name: "PassiveIntervalDays",
                table: "Words",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "PassiveNextReviewDate",
                table: "Words",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PassiveRepetitionCount",
                table: "Words",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Words_ActiveNextReviewDate",
                table: "Words",
                column: "ActiveNextReviewDate");

            migrationBuilder.CreateIndex(
                name: "IX_Words_PassiveNextReviewDate",
                table: "Words",
                column: "PassiveNextReviewDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Words_ActiveNextReviewDate",
                table: "Words");

            migrationBuilder.DropIndex(
                name: "IX_Words_PassiveNextReviewDate",
                table: "Words");

            migrationBuilder.DropColumn(
                name: "ActiveEaseFactor",
                table: "Words");

            migrationBuilder.DropColumn(
                name: "ActiveIntervalDays",
                table: "Words");

            migrationBuilder.DropColumn(
                name: "ActiveNextReviewDate",
                table: "Words");

            migrationBuilder.DropColumn(
                name: "ActiveRepetitionCount",
                table: "Words");

            migrationBuilder.DropColumn(
                name: "PassiveEaseFactor",
                table: "Words");

            migrationBuilder.DropColumn(
                name: "PassiveIntervalDays",
                table: "Words");

            migrationBuilder.DropColumn(
                name: "PassiveNextReviewDate",
                table: "Words");

            migrationBuilder.DropColumn(
                name: "PassiveRepetitionCount",
                table: "Words");
        }
    }
}
