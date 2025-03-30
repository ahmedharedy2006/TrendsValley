using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrendsValley.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addNoInStockColumnToProductTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NoInStock",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Product_Id",
                keyValue: 1,
                column: "NoInStock",
                value: 0);

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Product_Id",
                keyValue: 2,
                column: "NoInStock",
                value: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NoInStock",
                table: "Products");
        }
    }
}
