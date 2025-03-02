using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrendsValley.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddPhotoColumnToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "imgUrl",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Product_Id",
                keyValue: 1,
                columns: new[] { "Product_Details", "Product_Name", "imgUrl" },
                values: new object[] { "Black Short", "Black Short", "test.jpg" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Product_Id",
                keyValue: 2,
                columns: new[] { "Product_Details", "Product_Name", "imgUrl" },
                values: new object[] { "Black T_Shirt", "Black T-shirt", "test1.jpg" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "imgUrl",
                table: "Products");

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Product_Id",
                keyValue: 1,
                columns: new[] { "Product_Details", "Product_Name" },
                values: new object[] { "Nigga Black Short", "MoamenTheNigga" });

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Product_Id",
                keyValue: 2,
                columns: new[] { "Product_Details", "Product_Name" },
                values: new object[] { "Nigga Black T_Shirt", "HaridyTheNigga" });
        }
    }
}
