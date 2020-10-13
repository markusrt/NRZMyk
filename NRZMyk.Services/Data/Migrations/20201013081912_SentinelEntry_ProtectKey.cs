using Microsoft.EntityFrameworkCore.Migrations;

namespace NRZMyk.Services.Data.Migrations
{
    public partial class SentinelEntry_ProtectKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProtectKey",
                table: "SentinelEntries",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_SentinelEntries_ProtectKey",
                table: "SentinelEntries",
                column: "ProtectKey");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SentinelEntries_ProtectKey",
                table: "SentinelEntries");

            migrationBuilder.DropColumn(
                name: "ProtectKey",
                table: "SentinelEntries");
        }
    }
}
