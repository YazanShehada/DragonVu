using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragonVu.Migrations
{
    /// <inheritdoc />
    public partial class models : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Question_Subject_SubjectId",
                table: "Question");

            migrationBuilder.DropForeignKey(
                name: "FK_Result_AspNetUsers_UserId",
                table: "Result");

            migrationBuilder.DropForeignKey(
                name: "FK_Result_Subject_SubjectId",
                table: "Result");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Subject",
                table: "Subject");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Result",
                table: "Result");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Question",
                table: "Question");

            migrationBuilder.RenameTable(
                name: "Subject",
                newName: "subjects");

            migrationBuilder.RenameTable(
                name: "Result",
                newName: "results");

            migrationBuilder.RenameTable(
                name: "Question",
                newName: "questions");

            migrationBuilder.RenameIndex(
                name: "IX_Result_UserId",
                table: "results",
                newName: "IX_results_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Result_SubjectId",
                table: "results",
                newName: "IX_results_SubjectId");

            migrationBuilder.RenameIndex(
                name: "IX_Question_SubjectId",
                table: "questions",
                newName: "IX_questions_SubjectId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_subjects",
                table: "subjects",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_results",
                table: "results",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_questions",
                table: "questions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_questions_subjects_SubjectId",
                table: "questions",
                column: "SubjectId",
                principalTable: "subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_results_AspNetUsers_UserId",
                table: "results",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_results_subjects_SubjectId",
                table: "results",
                column: "SubjectId",
                principalTable: "subjects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_questions_subjects_SubjectId",
                table: "questions");

            migrationBuilder.DropForeignKey(
                name: "FK_results_AspNetUsers_UserId",
                table: "results");

            migrationBuilder.DropForeignKey(
                name: "FK_results_subjects_SubjectId",
                table: "results");

            migrationBuilder.DropPrimaryKey(
                name: "PK_subjects",
                table: "subjects");

            migrationBuilder.DropPrimaryKey(
                name: "PK_results",
                table: "results");

            migrationBuilder.DropPrimaryKey(
                name: "PK_questions",
                table: "questions");

            migrationBuilder.RenameTable(
                name: "subjects",
                newName: "Subject");

            migrationBuilder.RenameTable(
                name: "results",
                newName: "Result");

            migrationBuilder.RenameTable(
                name: "questions",
                newName: "Question");

            migrationBuilder.RenameIndex(
                name: "IX_results_UserId",
                table: "Result",
                newName: "IX_Result_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_results_SubjectId",
                table: "Result",
                newName: "IX_Result_SubjectId");

            migrationBuilder.RenameIndex(
                name: "IX_questions_SubjectId",
                table: "Question",
                newName: "IX_Question_SubjectId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Subject",
                table: "Subject",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Result",
                table: "Result",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Question",
                table: "Question",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Question_Subject_SubjectId",
                table: "Question",
                column: "SubjectId",
                principalTable: "Subject",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Result_AspNetUsers_UserId",
                table: "Result",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Result_Subject_SubjectId",
                table: "Result",
                column: "SubjectId",
                principalTable: "Subject",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
