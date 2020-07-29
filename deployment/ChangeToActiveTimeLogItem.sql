ALTER TABLE "TaskState" DROP CONSTRAINT "FK_TaskState_TimeLog_LatestTimeLogItemId";

DROP INDEX "IX_TaskState_LatestTimeLogItemId";

ALTER TABLE "TaskState" DROP COLUMN "LatestTimeLogItemId";

ALTER TABLE "TaskState" ADD "ActiveTimeLogItemId" uuid NULL;

CREATE INDEX "IX_TaskState_ActiveTimeLogItemId" ON "TaskState" ("ActiveTimeLogItemId");

ALTER TABLE "TaskState" ADD CONSTRAINT "FK_TaskState_TimeLog_ActiveTimeLogItemId" FOREIGN KEY ("ActiveTimeLogItemId") REFERENCES "TimeLog" ("Id") ON DELETE RESTRICT;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20200726164837_ChangeToActiveTimeLogItem', '3.1.6');
