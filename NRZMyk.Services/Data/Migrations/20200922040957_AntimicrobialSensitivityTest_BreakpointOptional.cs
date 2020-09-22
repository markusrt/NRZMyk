using Microsoft.EntityFrameworkCore.Migrations;

namespace NRZMyk.Services.Data.Migrations
{
    public partial class AntimicrobialSensitivityTest_BreakpointOptional : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AntimicrobialSensitivityTest_ClinicalBreakpoints_ClinicalBreakpointId",
                table: "AntimicrobialSensitivityTest");

            migrationBuilder.AlterColumn<int>(
                name: "ClinicalBreakpointId",
                table: "AntimicrobialSensitivityTest",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_AntimicrobialSensitivityTest_ClinicalBreakpoints_ClinicalBreakpointId",
                table: "AntimicrobialSensitivityTest",
                column: "ClinicalBreakpointId",
                principalTable: "ClinicalBreakpoints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AntimicrobialSensitivityTest_ClinicalBreakpoints_ClinicalBreakpointId",
                table: "AntimicrobialSensitivityTest");

            migrationBuilder.AlterColumn<int>(
                name: "ClinicalBreakpointId",
                table: "AntimicrobialSensitivityTest",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AntimicrobialSensitivityTest_ClinicalBreakpoints_ClinicalBreakpointId",
                table: "AntimicrobialSensitivityTest",
                column: "ClinicalBreakpointId",
                principalTable: "ClinicalBreakpoints",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
