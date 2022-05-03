using Microsoft.EntityFrameworkCore.Migrations;

namespace APIGestao.Migrations
{
    public partial class inserindoIDEmpresa : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IDEmpresa",
                table: "Despesas",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IDEmpresa",
                table: "Despesas");
        }
    }
}
