ALTER TABLE "TimerTags" ADD "Color" integer NOT NULL DEFAULT 0;

ALTER TABLE "TimeLog" ADD "Notes" text NULL;

ALTER TABLE "Tasks" ADD "Descritpion" text NULL;

ALTER TABLE "Tasks" ADD "ProjectId" uuid NULL;

CREATE TABLE "Projects" (
    "Id" uuid NOT NULL,
    "PublicId" text NOT NULL,
    "CreateDate" timestamp without time zone NOT NULL,
    "UpdateDate" timestamp without time zone NOT NULL,
    "ProfileId" uuid NULL,
    "Title" text NOT NULL,
    "Descritpion" text NULL,
    CONSTRAINT "PK_Projects" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Projects_UserProfiles_ProfileId" FOREIGN KEY ("ProfileId") REFERENCES "UserProfiles" ("Id") ON DELETE RESTRICT
);

CREATE INDEX "IX_Tasks_ProjectId" ON "Tasks" ("ProjectId");

CREATE INDEX "IX_Projects_ProfileId" ON "Projects" ("ProfileId");

ALTER TABLE "Tasks" ADD CONSTRAINT "FK_Tasks_Projects_ProjectId" FOREIGN KEY ("ProjectId") REFERENCES "Projects" ("Id") ON DELETE RESTRICT;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20200917222828_ProjectAndMore', '3.1.6');
