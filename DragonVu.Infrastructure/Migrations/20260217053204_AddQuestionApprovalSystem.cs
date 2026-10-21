using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragonVu.Migrations
{
    /// <inheritdoc />
    public partial class AddQuestionApprovalSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                table: "questions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovedById",
                table: "questions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedById",
                table: "questions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "QuizType",
                table: "questions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "questions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                table: "questions");

            migrationBuilder.DropColumn(
                name: "ApprovedById",
                table: "questions");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "questions");

            migrationBuilder.DropColumn(
                name: "QuizType",
                table: "questions");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "questions");
        }
    }
}
