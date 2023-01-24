using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataBaseAPI.Migrations
{
    public partial class userLeftOnReg2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "LeftOnRegistration",
                table: "Users",
                type: "int",
                nullable: true,
                computedColumnSql: "DATEDIFF(DAY, GETDATE(), RegisteredTo)",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComputedColumnSql: "DATEDIFF(DAY, RegisteredTo, GETDATE())");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "LeftOnRegistration",
                table: "Users",
                type: "int",
                nullable: true,
                computedColumnSql: "DATEDIFF(DAY, RegisteredTo, GETDATE())",
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true,
                oldComputedColumnSql: "DATEDIFF(DAY, GETDATE(), RegisteredTo)");
        }
    }
}
