CREATE TABLE "TimerTags" (
    "Id" uuid NOT NULL,
    "PublicId" text NOT NULL,
    "CreateDate" timestamp without time zone NOT NULL,
    "UpdateDate" timestamp without time zone NOT NULL,
    "ProfileId" uuid NULL,
    "Name" text NOT NULL,
    CONSTRAINT "PK_TimerTags" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_TimerTags_UserProfiles_ProfileId" FOREIGN KEY ("ProfileId") REFERENCES "UserProfiles" ("Id") ON DELETE RESTRICT
);

CREATE INDEX "IX_TimerTags_ProfileId" ON "TimerTags" ("ProfileId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20200909034255_Tags', '3.1.6');

CREATE TABLE "TagToTimerTasks" (
    "Id" uuid NOT NULL,
    "TagId" uuid NOT NULL,
    "TaskId" uuid NOT NULL,
    CONSTRAINT "PK_TagToTimerTasks" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_TagToTimerTasks_TimerTags_TagId" FOREIGN KEY ("TagId") REFERENCES "TimerTags" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_TagToTimerTasks_Tasks_TaskId" FOREIGN KEY ("TaskId") REFERENCES "Tasks" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_TagToTimerTasks_TagId" ON "TagToTimerTasks" ("TagId");

CREATE INDEX "IX_TagToTimerTasks_TaskId" ON "TagToTimerTasks" ("TaskId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20200910045555_TagsRel', '3.1.6');


