using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataBaseAPI.Migrations
{
    public partial class numeroTres : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "id",
                table: "Trainers",
                newName: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Trainers",
                newName: "id");
        }
    }
}
