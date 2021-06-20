using Microsoft.EntityFrameworkCore.Migrations;

namespace MyFarmer.Data.Migrations
{
    public partial class ChangeFoodServiceTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsChecked",
                table: "Servises");

            migrationBuilder.DropColumn(
                name: "IsChecked",
                table: "Foods");

            migrationBuilder.AddColumn<bool>(
                name: "IsChecked",
                table: "FarmsServices",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsChecked",
                table: "FarmsFoods",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsChecked",
                table: "FarmsServices");

            migrationBuilder.DropColumn(
                name: "IsChecked",
                table: "FarmsFoods");

            migrationBuilder.AddColumn<bool>(
                name: "IsChecked",
                table: "Servises",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsChecked",
                table: "Foods",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
    }
}
