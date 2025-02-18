using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrendsValley.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class ChangeColumnNameCity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "name",
                table: "cities",
                newName: "City_Name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "cities",
                newName: "City_Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "City_Name",
                table: "cities",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "City_Id",
                table: "cities",
                newName: "Id");
        }
    }
}
