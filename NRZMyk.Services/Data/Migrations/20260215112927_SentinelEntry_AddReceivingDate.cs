using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NRZMyk.Services.Data.Migrations
{
    /// <inheritdoc />
    public partial class SentinelEntry_AddReceivingDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ReceivingDate",
                table: "SentinelEntries",
                type: "datetime2",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceivingDate",
                table: "SentinelEntries");
        }
    }
}
