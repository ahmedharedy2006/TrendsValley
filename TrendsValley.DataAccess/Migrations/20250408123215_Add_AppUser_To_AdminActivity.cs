using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrendsValley.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Add_AppUser_To_AdminActivity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "AdminActivities",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_AdminActivities_UserId",
                table: "AdminActivities",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AdminActivities_AspNetUsers_UserId",
                table: "AdminActivities",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdminActivities_AspNetUsers_UserId",
                table: "AdminActivities");

            migrationBuilder.DropIndex(
                name: "IX_AdminActivities_UserId",
                table: "AdminActivities");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "AdminActivities",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
