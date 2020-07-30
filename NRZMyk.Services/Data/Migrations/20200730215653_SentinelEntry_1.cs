using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NRZMyk.Services.Data.Migrations
{
    public partial class SentinelEntry_1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SentinelEntries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SamplingDate = table.Column<DateTime>(nullable: true),
                    SenderLaboratoryNumber = table.Column<string>(nullable: true),
                    Material = table.Column<int>(nullable: false),
                    ResidentialTreatment = table.Column<int>(nullable: false),
                    IdentifiedSpecies = table.Column<string>(nullable: true),
                    SpeciesTestingMethod = table.Column<int>(nullable: false),
                    AgeGroup = table.Column<int>(nullable: false),
                    Remark = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SentinelEntries", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SentinelEntries");
        }
    }
}
