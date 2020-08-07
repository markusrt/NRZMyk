using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NRZMyk.Services.Data.Migrations
{
    public partial class ClinicalBreakpoints_Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClinicalBreakpoints",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AntifungalAgent = table.Column<int>(nullable: false),
                    AntifungalAgentDetails = table.Column<string>(nullable: false),
                    Species = table.Column<int>(nullable: false),
                    Standard = table.Column<int>(nullable: false),
                    Version = table.Column<string>(nullable: false),
                    ValidFrom = table.Column<DateTime>(nullable: false),
                    MicBreakpointSusceptible = table.Column<float>(nullable: true),
                    MicBreakpointResistent = table.Column<float>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClinicalBreakpoints", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClinicalBreakpoints_Standard_Version_AntifungalAgentDetails",
                table: "ClinicalBreakpoints",
                columns: new[] { "Standard", "Version", "AntifungalAgentDetails" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClinicalBreakpoints");
        }
    }
}
