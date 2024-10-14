using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NRZMyk.Services.Data.Migrations
{
    public partial class Organization_RemoveEmailAndAddLastReminderSent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Organizations");

            migrationBuilder.AddColumn<DateTime>(
                name: "LastReminderSent",
                table: "Organizations",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastReminderSent",
                table: "Organizations");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Organizations",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
