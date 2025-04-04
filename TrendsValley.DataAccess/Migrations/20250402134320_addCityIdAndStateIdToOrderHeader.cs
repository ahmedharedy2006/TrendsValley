using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrendsValley.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addCityIdAndStateIdToOrderHeader : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "phoneNumber",
                table: "OrderHeaders");

            migrationBuilder.AddColumn<int>(
                name: "cityId",
                table: "OrderHeaders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "stateId",
                table: "OrderHeaders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_OrderHeaders_cityId",
                table: "OrderHeaders",
                column: "cityId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderHeaders_stateId",
                table: "OrderHeaders",
                column: "stateId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderHeaders_cities_cityId",
                table: "OrderHeaders",
                column: "cityId",
                principalTable: "cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderHeaders_states_stateId",
                table: "OrderHeaders",
                column: "stateId",
                principalTable: "states",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderHeaders_cities_cityId",
                table: "OrderHeaders");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderHeaders_states_stateId",
                table: "OrderHeaders");

            migrationBuilder.DropIndex(
                name: "IX_OrderHeaders_cityId",
                table: "OrderHeaders");

            migrationBuilder.DropIndex(
                name: "IX_OrderHeaders_stateId",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "cityId",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "stateId",
                table: "OrderHeaders");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "phoneNumber",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
