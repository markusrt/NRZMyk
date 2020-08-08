using Microsoft.EntityFrameworkCore.Migrations;

namespace NRZMyk.Services.Data.Migrations
{
    public partial class SentinelEntry_HospitalDepartmentAndSpeciesMethod : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResidentialTreatment",
                table: "SentinelEntries");

            migrationBuilder.DropColumn(
                name: "SpeciesTestingMethod",
                table: "SentinelEntries");

            migrationBuilder.AlterColumn<int>(
                name: "IdentifiedSpecies",
                table: "SentinelEntries",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "HospitalDepartment",
                table: "SentinelEntries",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "HospitalDepartmentType",
                table: "SentinelEntries",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SpeciesIdentificationMethod",
                table: "SentinelEntries",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HospitalDepartment",
                table: "SentinelEntries");

            migrationBuilder.DropColumn(
                name: "HospitalDepartmentType",
                table: "SentinelEntries");

            migrationBuilder.DropColumn(
                name: "SpeciesIdentificationMethod",
                table: "SentinelEntries");

            migrationBuilder.AlterColumn<string>(
                name: "IdentifiedSpecies",
                table: "SentinelEntries",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "ResidentialTreatment",
                table: "SentinelEntries",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SpeciesTestingMethod",
                table: "SentinelEntries",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
