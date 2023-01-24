using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataBaseAPI.Migrations
{
    public partial class userLeftOnReg : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LeftOnRegistration",
                table: "Users",
                type: "int",
                nullable: true,
                computedColumnSql: "DATEDIFF(DAY, RegisteredTo, GETDATE())");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LeftOnRegistration",
                table: "Users");
        }
    }
}
