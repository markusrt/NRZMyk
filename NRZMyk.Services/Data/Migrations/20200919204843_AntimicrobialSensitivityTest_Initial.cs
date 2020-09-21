using Microsoft.EntityFrameworkCore.Migrations;

namespace NRZMyk.Services.Data.Migrations
{
    public partial class AntimicrobialSensitivityTest_Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AntimicrobialSensitivityTest",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestingMethod = table.Column<int>(nullable: false),
                    AntifungalAgent = table.Column<int>(nullable: false),
                    SentinelEntryId = table.Column<int>(nullable: true),
                    ClinicalBreakpointId = table.Column<int>(nullable: false),
                    MinimumInhibitoryConcentration = table.Column<float>(nullable: false),
                    Resistance = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AntimicrobialSensitivityTest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AntimicrobialSensitivityTest_ClinicalBreakpoints_ClinicalBreakpointId",
                        column: x => x.ClinicalBreakpointId,
                        principalTable: "ClinicalBreakpoints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AntimicrobialSensitivityTest_SentinelEntries_SentinelEntryId",
                        column: x => x.SentinelEntryId,
                        principalTable: "SentinelEntries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AntimicrobialSensitivityTest_ClinicalBreakpointId",
                table: "AntimicrobialSensitivityTest",
                column: "ClinicalBreakpointId");

            migrationBuilder.CreateIndex(
                name: "IX_AntimicrobialSensitivityTest_SentinelEntryId",
                table: "AntimicrobialSensitivityTest",
                column: "SentinelEntryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AntimicrobialSensitivityTest");
        }
    }
}
