using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TrendsValley.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class seedDataToCitiesAndStates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "cities",
                columns: new[] { "Id", "name" },
                values: new object[,]
                {
                    { 1, "Arcadia" },
                    { 2, "Brea" },
                    { 3, "Chico" },
                    { 4, "Ajo" },
                    { 5, "Clifton" }
                });

            migrationBuilder.InsertData(
                table: "states",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "California" },
                    { 2, "Florida" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "cities",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "states",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "states",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
