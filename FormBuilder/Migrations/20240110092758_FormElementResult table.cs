using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace FormBuilder.Migrations
{
    public partial class FormElementResulttable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FormElementResults",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FormElementId = table.Column<int>(nullable: false),
                    UserId = table.Column<Guid>(nullable: true),
                    OverallPoint = table.Column<decimal>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormElementResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FormElementResults_FormElements_FormElementId",
                        column: x => x.FormElementId,
                        principalTable: "FormElements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FormElementResults_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FormElementResults_FormElementId",
                table: "FormElementResults",
                column: "FormElementId");

            migrationBuilder.CreateIndex(
                name: "IX_FormElementResults_UserId",
                table: "FormElementResults",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FormElementResults");
        }
    }
}
