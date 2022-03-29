using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NRZMyk.Services.Data.Migrations
{
    public partial class SentinelEntry_InternalHospitalDepartmentType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "InternalHospitalDepartmentType",
                table: "SentinelEntries",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InternalHospitalDepartmentType",
                table: "SentinelEntries");
        }
    }
}
