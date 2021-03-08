using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NoCrast.Server.Database.Postgres.Migrations
{
    public partial class TagsRel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TagToTimerTasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TagId = table.Column<Guid>(nullable: false),
                    TaskId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TagToTimerTasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TagToTimerTasks_TimerTags_TagId",
                        column: x => x.TagId,
                        principalTable: "TimerTags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TagToTimerTasks_Tasks_TaskId",
                        column: x => x.TaskId,
                        principalTable: "Tasks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TagToTimerTasks_TagId",
                table: "TagToTimerTasks",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_TagToTimerTasks_TaskId",
                table: "TagToTimerTasks",
                column: "TaskId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TagToTimerTasks");
        }
    }
}
