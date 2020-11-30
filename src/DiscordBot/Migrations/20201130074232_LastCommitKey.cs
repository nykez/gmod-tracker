using Microsoft.EntityFrameworkCore.Migrations;

namespace gmodwatcher.Migrations
{
    public partial class LastCommitKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LastCommit",
                columns: table => new
                {
                    LastCommit = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LastCommit", x => x.LastCommit);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LastCommit");
        }
    }
}
