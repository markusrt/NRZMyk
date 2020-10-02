using Microsoft.EntityFrameworkCore.Migrations;

namespace NRZMyk.Services.Data.Migrations
{
    public partial class Sentinel_Entry_LabAndCryoNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CryoBoxNumber",
                table: "SentinelEntries",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "CryoBoxSlot",
                table: "SentinelEntries",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "SentinelEntries",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "YearlySequentialEntryNumber",
                table: "SentinelEntries",
                nullable: false,
                defaultValue: 0);
           
            migrationBuilder.Sql("UPDATE SentinelEntries SET Year = YEAR(GETDATE())");
            migrationBuilder.Sql("UPDATE SentinelEntries SET YearlySequentialEntryNumber = Id");
            migrationBuilder.Sql("UPDATE SentinelEntries SET CryoBoxNumber = 1");
            migrationBuilder.Sql("UPDATE SentinelEntries SET CryoBoxSlot = Id");

            migrationBuilder.CreateIndex(
                name: "IX_SentinelEntries_CryoBoxNumber_CryoBoxSlot",
                table: "SentinelEntries",
                columns: new[] { "CryoBoxNumber", "CryoBoxSlot" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SentinelEntries_Year_YearlySequentialEntryNumber",
                table: "SentinelEntries",
                columns: new[] { "Year", "YearlySequentialEntryNumber" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SentinelEntries_CryoBoxNumber_CryoBoxSlot",
                table: "SentinelEntries");

            migrationBuilder.DropIndex(
                name: "IX_SentinelEntries_Year_YearlySequentialEntryNumber",
                table: "SentinelEntries");

            migrationBuilder.DropColumn(
                name: "CryoBoxNumber",
                table: "SentinelEntries");

            migrationBuilder.DropColumn(
                name: "CryoBoxSlot",
                table: "SentinelEntries");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "SentinelEntries");

            migrationBuilder.DropColumn(
                name: "YearlySequentialEntryNumber",
                table: "SentinelEntries");
        }
    }
}
