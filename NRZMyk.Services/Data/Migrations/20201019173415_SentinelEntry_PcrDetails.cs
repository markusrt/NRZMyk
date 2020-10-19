using Microsoft.EntityFrameworkCore.Migrations;

namespace NRZMyk.Services.Data.Migrations
{
    public partial class SentinelEntry_PcrDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PcrDetails",
                table: "SentinelEntries",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PcrDetails",
                table: "SentinelEntries");
        }
    }
}
