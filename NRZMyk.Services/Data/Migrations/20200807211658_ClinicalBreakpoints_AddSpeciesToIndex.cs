using Microsoft.EntityFrameworkCore.Migrations;

namespace NRZMyk.Services.Data.Migrations
{
    public partial class ClinicalBreakpoints_AddSpeciesToIndex : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ClinicalBreakpoints_Standard_Version_AntifungalAgentDetails",
                table: "ClinicalBreakpoints");

            migrationBuilder.CreateIndex(
                name: "IX_ClinicalBreakpoints_Standard_Version_Species_AntifungalAgentDetails",
                table: "ClinicalBreakpoints",
                columns: new[] { "Standard", "Version", "Species", "AntifungalAgentDetails" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_ClinicalBreakpoints_Standard_Version_Species_AntifungalAgentDetails",
                table: "ClinicalBreakpoints");

            migrationBuilder.CreateIndex(
                name: "IX_ClinicalBreakpoints_Standard_Version_AntifungalAgentDetails",
                table: "ClinicalBreakpoints",
                columns: new[] { "Standard", "Version", "AntifungalAgentDetails" },
                unique: true);
        }
    }
}
