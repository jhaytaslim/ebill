using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ebill.Migrations
{
    public partial class settings : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "settings",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    secret = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    iv = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    billerName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_settings", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "settings");
        }
    }
}
