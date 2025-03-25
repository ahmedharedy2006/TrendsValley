using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrendsValley.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class makeProductDetailsViewModelNotMapped : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carts_ProductDetailsViewModel_ProductDetailsViewModelId",
                table: "Carts");

            migrationBuilder.DropTable(
                name: "ProductDetailsViewModel");

            migrationBuilder.DropIndex(
                name: "IX_Carts_ProductDetailsViewModelId",
                table: "Carts");

            migrationBuilder.DropColumn(
                name: "ProductDetailsViewModelId",
                table: "Carts");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductDetailsViewModelId",
                table: "Carts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ProductDetailsViewModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Product_Id = table.Column<int>(type: "int", nullable: false),
                    Brandname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CategoryName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductDetailsViewModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductDetailsViewModel_Products_Product_Id",
                        column: x => x.Product_Id,
                        principalTable: "Products",
                        principalColumn: "Product_Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Carts_ProductDetailsViewModelId",
                table: "Carts",
                column: "ProductDetailsViewModelId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductDetailsViewModel_Product_Id",
                table: "ProductDetailsViewModel",
                column: "Product_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_ProductDetailsViewModel_ProductDetailsViewModelId",
                table: "Carts",
                column: "ProductDetailsViewModelId",
                principalTable: "ProductDetailsViewModel",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
