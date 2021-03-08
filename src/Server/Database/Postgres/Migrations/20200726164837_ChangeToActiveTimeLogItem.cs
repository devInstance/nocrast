using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace NoCrast.Server.Database.Postgres.Migrations
{
    public partial class ChangeToActiveTimeLogItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskState_TimeLog_LatestTimeLogItemId",
                table: "TaskState");

            migrationBuilder.DropIndex(
                name: "IX_TaskState_LatestTimeLogItemId",
                table: "TaskState");

            migrationBuilder.DropColumn(
                name: "LatestTimeLogItemId",
                table: "TaskState");

            migrationBuilder.AddColumn<Guid>(
                name: "ActiveTimeLogItemId",
                table: "TaskState",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskState_ActiveTimeLogItemId",
                table: "TaskState",
                column: "ActiveTimeLogItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskState_TimeLog_ActiveTimeLogItemId",
                table: "TaskState",
                column: "ActiveTimeLogItemId",
                principalTable: "TimeLog",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskState_TimeLog_ActiveTimeLogItemId",
                table: "TaskState");

            migrationBuilder.DropIndex(
                name: "IX_TaskState_ActiveTimeLogItemId",
                table: "TaskState");

            migrationBuilder.DropColumn(
                name: "ActiveTimeLogItemId",
                table: "TaskState");

            migrationBuilder.AddColumn<Guid>(
                name: "LatestTimeLogItemId",
                table: "TaskState",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TaskState_LatestTimeLogItemId",
                table: "TaskState",
                column: "LatestTimeLogItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskState_TimeLog_LatestTimeLogItemId",
                table: "TaskState",
                column: "LatestTimeLogItemId",
                principalTable: "TimeLog",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
