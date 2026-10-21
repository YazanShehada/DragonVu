using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DragonVu.Migrations
{
    /// <inheritdoc />
    public partial class resNameNullimage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_results_AspNetUsers_UserId",
                table: "results");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "results",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "results",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_results_AspNetUsers_UserId",
                table: "results",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_results_AspNetUsers_UserId",
                table: "results");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "results");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "results");

            migrationBuilder.AddForeignKey(
                name: "FK_results_AspNetUsers_UserId",
                table: "results",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
