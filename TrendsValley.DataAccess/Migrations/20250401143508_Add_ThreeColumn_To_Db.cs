using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrendsValley.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Add_ThreeColumn_To_Db : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentMehtod",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PreferredCarriers",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Currency",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PaymentMehtod",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "PreferredCarriers",
                table: "AspNetUsers");
        }
    }
}
