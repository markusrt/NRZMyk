using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NRZMyk.Services.Data.Migrations
{
    public partial class SentinelEntry_KryoDateAndComment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CryoDate",
                table: "SentinelEntries",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CryoRemark",
                table: "SentinelEntries",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CryoDate",
                table: "SentinelEntries");

            migrationBuilder.DropColumn(
                name: "CryoRemark",
                table: "SentinelEntries");
        }
    }
}
