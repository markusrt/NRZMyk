using Microsoft.EntityFrameworkCore.Migrations;

namespace NRZMyk.Services.Data.Migrations
{
    public partial class RemoveYearFromCryoBoxIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SentinelEntries_Year_CryoBoxNumber_CryoBoxSlot",
                table: "SentinelEntries");

            migrationBuilder.CreateIndex(
                name: "IX_SentinelEntries_CryoBoxNumber_CryoBoxSlot",
                table: "SentinelEntries",
                columns: new[] { "CryoBoxNumber", "CryoBoxSlot" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SentinelEntries_CryoBoxNumber_CryoBoxSlot",
                table: "SentinelEntries");

            migrationBuilder.CreateIndex(
                name: "IX_SentinelEntries_Year_CryoBoxNumber_CryoBoxSlot",
                table: "SentinelEntries",
                columns: new[] { "Year", "CryoBoxNumber", "CryoBoxSlot" },
                unique: true);
        }
    }
}
