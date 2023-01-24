using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataBaseAPI.Migrations
{
    public partial class RecomendedResult : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VariableForTrainerMatch",
                table: "Users");

            migrationBuilder.AddColumn<string>(
                name: "RecomendedProgram",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RecomendedProgram",
                table: "Users");

            migrationBuilder.AddColumn<double>(
                name: "VariableForTrainerMatch",
                table: "Users",
                type: "float",
                nullable: true);
        }
    }
}
