using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrendsValley.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Brand_Id",
                keyValue: 1,
                column: "Brand_Name",
                value: "adidass");

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Brand_Id",
                keyValue: 2,
                column: "Brand_Name",
                value: "NIKEe");

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Brand_Id",
                keyValue: 3,
                column: "Brand_Name",
                value: "Activee");

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Brand_Id",
                keyValue: 4,
                column: "Brand_Name",
                value: "ZARAa");

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Brand_Id",
                keyValue: 5,
                column: "Brand_Name",
                value: "H.Ms");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Category_id",
                keyValue: 1,
                column: "Category_Name",
                value: "T-Shirts");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Category_id",
                keyValue: 2,
                column: "Category_Name",
                value: "Pantalons");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Category_id",
                keyValue: 3,
                column: "Category_Name",
                value: "Shortss");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Product_Id",
                keyValue: 1,
                columns: new[] { "Product_Details", "Product_Name" },
                values: new object[] { "Black Shortss", "Black Shortssss" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Product_Id",
                keyValue: 2,
                columns: new[] { "Product_Details", "Product_Name" },
                values: new object[] { "Black T_Shirtsss", "Black T-shirtsss" });

            migrationBuilder.UpdateData(
                table: "cities",
                keyColumn: "Id",
                keyValue: 1,
                column: "name",
                value: "Arcadias");

            migrationBuilder.UpdateData(
                table: "cities",
                keyColumn: "Id",
                keyValue: 2,
                column: "name",
                value: "Breas");

            migrationBuilder.UpdateData(
                table: "cities",
                keyColumn: "Id",
                keyValue: 3,
                column: "name",
                value: "Chicos");

            migrationBuilder.UpdateData(
                table: "cities",
                keyColumn: "Id",
                keyValue: 4,
                column: "name",
                value: "Ajos");

            migrationBuilder.UpdateData(
                table: "cities",
                keyColumn: "Id",
                keyValue: 5,
                column: "name",
                value: "Cliftons");

            migrationBuilder.UpdateData(
                table: "states",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Californias");

            migrationBuilder.UpdateData(
                table: "states",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Floridas");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Brand_Id",
                keyValue: 1,
                column: "Brand_Name",
                value: "adidas");

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Brand_Id",
                keyValue: 2,
                column: "Brand_Name",
                value: "NIKE");

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Brand_Id",
                keyValue: 3,
                column: "Brand_Name",
                value: "Active");

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Brand_Id",
                keyValue: 4,
                column: "Brand_Name",
                value: "ZARA");

            migrationBuilder.UpdateData(
                table: "Brands",
                keyColumn: "Brand_Id",
                keyValue: 5,
                column: "Brand_Name",
                value: "H.M");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Category_id",
                keyValue: 1,
                column: "Category_Name",
                value: "T-Shirt");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Category_id",
                keyValue: 2,
                column: "Category_Name",
                value: "Pantalon");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Category_id",
                keyValue: 3,
                column: "Category_Name",
                value: "Shorts");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Product_Id",
                keyValue: 1,
                columns: new[] { "Product_Details", "Product_Name" },
                values: new object[] { "Black Short", "Black Short" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Product_Id",
                keyValue: 2,
                columns: new[] { "Product_Details", "Product_Name" },
                values: new object[] { "Black T_Shirt", "Black T-shirt" });

            migrationBuilder.UpdateData(
                table: "cities",
                keyColumn: "Id",
                keyValue: 1,
                column: "name",
                value: "Arcadia");

            migrationBuilder.UpdateData(
                table: "cities",
                keyColumn: "Id",
                keyValue: 2,
                column: "name",
                value: "Brea");

            migrationBuilder.UpdateData(
                table: "cities",
                keyColumn: "Id",
                keyValue: 3,
                column: "name",
                value: "Chico");

            migrationBuilder.UpdateData(
                table: "cities",
                keyColumn: "Id",
                keyValue: 4,
                column: "name",
                value: "Ajo");

            migrationBuilder.UpdateData(
                table: "cities",
                keyColumn: "Id",
                keyValue: 5,
                column: "name",
                value: "Clifton");

            migrationBuilder.UpdateData(
                table: "states",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "California");

            migrationBuilder.UpdateData(
                table: "states",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Florida");
        }
    }
}
