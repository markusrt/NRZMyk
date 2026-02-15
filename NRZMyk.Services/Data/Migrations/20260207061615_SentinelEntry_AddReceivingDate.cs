using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NRZMyk.Services.Data.Migrations
{
    public partial class SentinelEntry_AddReceivingDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add column as nullable first
            migrationBuilder.AddColumn<DateTime>(
                name: "ReceivingDate",
                table: "SentinelEntries",
                nullable: true);

            // Initialize ReceivingDate with SamplingDate for existing entries
            migrationBuilder.Sql(
                @"UPDATE SentinelEntries 
                  SET ReceivingDate = COALESCE(SamplingDate, GETDATE())");

            // Make the column non-nullable
            migrationBuilder.AlterColumn<DateTime>(
                name: "ReceivingDate",
                table: "SentinelEntries",
                nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceivingDate",
                table: "SentinelEntries");
        }
    }
}
