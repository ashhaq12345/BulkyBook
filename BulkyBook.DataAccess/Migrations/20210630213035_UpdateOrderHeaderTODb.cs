using Microsoft.EntityFrameworkCore.Migrations;

namespace BulkyBook.DataAccess.Migrations
{
    public partial class UpdateOrderHeaderTODb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OrderStatus",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "OrderTotal",
                table: "OrderHeaders",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderStatus",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "OrderTotal",
                table: "OrderHeaders");
        }
    }
}
