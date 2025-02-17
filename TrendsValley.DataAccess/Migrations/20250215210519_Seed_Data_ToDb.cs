using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TrendsValley.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Seed_Data_ToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Brands",
                columns: new[] { "Brand_Id", "Brand_Name" },
                values: new object[,]
                {
                    { 1, "adidas" },
                    { 2, "NIKE" },
                    { 3, "Active" },
                    { 4, "ZARA" },
                    { 5, "H.M" }
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Category_id", "Category_Name" },
                values: new object[,]
                {
                    { 1, "T-Shirt" },
                    { 2, "Pantalon" },
                    { 3, "Shorts" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Product_Id", "Brand_Id", "Category_id", "Product_Details", "Product_Name", "Product_Price" },
                values: new object[,]
                {
                    { 1, 2, 3, "Nigga Black Short", "MoamenTheNigga", 69.00m },
                    { 2, 5, 1, "Nigga Black T_Shirt", "HaridyTheNigga", 54.00m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Brands",
                keyColumn: "Brand_Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Brands",
                keyColumn: "Brand_Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Brands",
                keyColumn: "Brand_Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Category_id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Product_Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Product_Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Brands",
                keyColumn: "Brand_Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Brands",
                keyColumn: "Brand_Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Category_id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Category_id",
                keyValue: 3);
        }
    }
}
