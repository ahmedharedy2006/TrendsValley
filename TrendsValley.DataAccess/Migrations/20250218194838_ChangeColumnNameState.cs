using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrendsValley.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ChangeColumnNameState : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "states",
                newName: "State_Name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "states",
                newName: "State_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "State_Name",
                table: "states",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "State_Id",
                table: "states",
                newName: "Id");
        }
    }
}
