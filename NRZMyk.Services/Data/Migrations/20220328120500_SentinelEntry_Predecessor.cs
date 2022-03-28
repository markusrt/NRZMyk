using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NRZMyk.Services.Data.Migrations
{
    public partial class SentinelEntry_Predecessor : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PredecessorEntryId",
                table: "SentinelEntries",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SentinelEntries_PredecessorEntryId",
                table: "SentinelEntries",
                column: "PredecessorEntryId");

            migrationBuilder.AddForeignKey(
                name: "FK_SentinelEntries_SentinelEntries_PredecessorEntryId",
                table: "SentinelEntries",
                column: "PredecessorEntryId",
                principalTable: "SentinelEntries",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SentinelEntries_SentinelEntries_PredecessorEntryId",
                table: "SentinelEntries");

            migrationBuilder.DropIndex(
                name: "IX_SentinelEntries_PredecessorEntryId",
                table: "SentinelEntries");

            migrationBuilder.DropColumn(
                name: "PredecessorEntryId",
                table: "SentinelEntries");
        }
    }
}
