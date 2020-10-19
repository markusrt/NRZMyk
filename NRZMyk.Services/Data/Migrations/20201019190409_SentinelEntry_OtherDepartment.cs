using Microsoft.EntityFrameworkCore.Migrations;

namespace NRZMyk.Services.Data.Migrations
{
    public partial class SentinelEntry_OtherDepartment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "OtherHospitalDepartment",
                table: "SentinelEntries",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OtherHospitalDepartment",
                table: "SentinelEntries");
        }
    }
}
