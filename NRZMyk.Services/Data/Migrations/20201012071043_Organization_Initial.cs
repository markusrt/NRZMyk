using Microsoft.EntityFrameworkCore.Migrations;

namespace NRZMyk.Services.Data.Migrations
{
    public partial class Organization_Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrganizationId",
                table: "RemoteAccounts",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Organizations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RemoteAccounts_OrganizationId",
                table: "RemoteAccounts",
                column: "OrganizationId");

            migrationBuilder.AddForeignKey(
                name: "FK_RemoteAccounts_Organizations_OrganizationId",
                table: "RemoteAccounts",
                column: "OrganizationId",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RemoteAccounts_Organizations_OrganizationId",
                table: "RemoteAccounts");

            migrationBuilder.DropTable(
                name: "Organizations");

            migrationBuilder.DropIndex(
                name: "IX_RemoteAccounts_OrganizationId",
                table: "RemoteAccounts");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                table: "RemoteAccounts");
        }
    }
}
