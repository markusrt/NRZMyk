using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NRZMyk.Services.Migrations
{
    public partial class Initial_MySql : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AlterDatabase()
            //    .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ClinicalBreakpoints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AntifungalAgent = table.Column<int>(type: "int", nullable: false),
                    AntifungalAgentDetails = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Species = table.Column<int>(type: "int", nullable: false),
                    Standard = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ValidFrom = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    MicBreakpointSusceptible = table.Column<float>(type: "float", nullable: true),
                    MicBreakpointResistent = table.Column<float>(type: "float", nullable: true),
                    TechnicalUncertainty = table.Column<float>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClinicalBreakpoints", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "SentinelEntries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SamplingDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    SenderLaboratoryNumber = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Material = table.Column<int>(type: "int", nullable: false),
                    OtherMaterial = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    HospitalDepartmentType = table.Column<int>(type: "int", nullable: false),
                    HospitalDepartment = table.Column<int>(type: "int", nullable: false),
                    OtherHospitalDepartment = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    SpeciesIdentificationMethod = table.Column<int>(type: "int", nullable: false),
                    PcrDetails = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IdentifiedSpecies = table.Column<int>(type: "int", nullable: false),
                    OtherIdentifiedSpecies = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AgeGroup = table.Column<int>(type: "int", nullable: false),
                    Remark = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Gender = table.Column<int>(type: "int", nullable: false),
                    CryoBoxNumber = table.Column<int>(type: "int", nullable: false),
                    CryoBoxSlot = table.Column<int>(type: "int", nullable: false),
                    YearlySequentialEntryNumber = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    ProtectKey = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CryoDate = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    CryoRemark = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SentinelEntries", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RemoteAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ObjectId = table.Column<Guid>(type: "char(36)", nullable: false),
                    DisplayName = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Email = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Country = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    City = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Postalcode = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Street = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    OrganizationId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RemoteAccounts", x => x.Id);
                    //table.ForeignKey(
                    //    name: "FK_RemoteAccounts_Organizations_OrganizationId",
                    //    column: x => x.OrganizationId,
                    //    principalTable: "Organizations",
                    //    principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "AntimicrobialSensitivityTest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    TestingMethod = table.Column<int>(type: "int", nullable: false),
                    AntifungalAgent = table.Column<int>(type: "int", nullable: false),
                    SentinelEntryId = table.Column<int>(type: "int", nullable: true),
                    ClinicalBreakpointId = table.Column<int>(type: "int", nullable: true),
                    MinimumInhibitoryConcentration = table.Column<float>(type: "float", nullable: false),
                    Resistance = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AntimicrobialSensitivityTest", x => x.Id);
                    //table.ForeignKey(
                    //    name: "FK_AntimicrobialSensitivityTest_ClinicalBreakpoints_ClinicalBre~",
                    //    column: x => x.ClinicalBreakpointId,
                    //    principalTable: "ClinicalBreakpoints",
                    //    principalColumn: "Id");
                    //table.ForeignKey(
                    //    name: "FK_AntimicrobialSensitivityTest_SentinelEntries_SentinelEntryId",
                    //    column: x => x.SentinelEntryId,
                    //    principalTable: "SentinelEntries",
                    //    principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_AntimicrobialSensitivityTest_ClinicalBreakpointId",
                table: "AntimicrobialSensitivityTest",
                column: "ClinicalBreakpointId");

            migrationBuilder.CreateIndex(
                name: "IX_AntimicrobialSensitivityTest_SentinelEntryId",
                table: "AntimicrobialSensitivityTest",
                column: "SentinelEntryId");

            migrationBuilder.CreateIndex(
                name: "IX_ClinicalBreakpoints_Standard_Version_Species_AntifungalAgent~",
                table: "ClinicalBreakpoints",
                columns: new[] { "Standard", "Version", "Species", "AntifungalAgentDetails" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RemoteAccounts_ObjectId",
                table: "RemoteAccounts",
                column: "ObjectId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RemoteAccounts_OrganizationId",
                table: "RemoteAccounts",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_SentinelEntries_CryoBoxNumber_CryoBoxSlot",
                table: "SentinelEntries",
                columns: new[] { "CryoBoxNumber", "CryoBoxSlot" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SentinelEntries_ProtectKey",
                table: "SentinelEntries",
                column: "ProtectKey");

            migrationBuilder.CreateIndex(
                name: "IX_SentinelEntries_Year_YearlySequentialEntryNumber",
                table: "SentinelEntries",
                columns: new[] { "Year", "YearlySequentialEntryNumber" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AntimicrobialSensitivityTest");

            migrationBuilder.DropTable(
                name: "RemoteAccounts");

            migrationBuilder.DropTable(
                name: "ClinicalBreakpoints");

            migrationBuilder.DropTable(
                name: "SentinelEntries");

            migrationBuilder.DropTable(
                name: "Organizations");
        }
    }
}
