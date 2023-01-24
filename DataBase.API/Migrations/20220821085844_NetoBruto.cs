using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataBaseAPI.Migrations
{
    public partial class NetoBruto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Height",
                table: "Trainers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SportHistory",
                table: "Trainers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Weight",
                table: "Trainers",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Bruto",
                table: "Employees",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Neto",
                table: "Employees",
                type: "float",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Height",
                table: "Trainers");

            migrationBuilder.DropColumn(
                name: "SportHistory",
                table: "Trainers");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "Trainers");

            migrationBuilder.DropColumn(
                name: "Bruto",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "Neto",
                table: "Employees");
        }
    }
}
