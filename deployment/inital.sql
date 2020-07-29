CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

CREATE TABLE "AspNetRoles" (
    "Id" uuid NOT NULL,
    "Name" character varying(256) NULL,
    "NormalizedName" character varying(256) NULL,
    "ConcurrencyStamp" text NULL,
    CONSTRAINT "PK_AspNetRoles" PRIMARY KEY ("Id")
);

CREATE TABLE "AspNetUsers" (
    "Id" uuid NOT NULL,
    "UserName" character varying(256) NULL,
    "NormalizedUserName" character varying(256) NULL,
    "Email" character varying(256) NULL,
    "NormalizedEmail" character varying(256) NULL,
    "EmailConfirmed" boolean NOT NULL,
    "PasswordHash" text NULL,
    "SecurityStamp" text NULL,
    "ConcurrencyStamp" text NULL,
    "PhoneNumber" text NULL,
    "PhoneNumberConfirmed" boolean NOT NULL,
    "TwoFactorEnabled" boolean NOT NULL,
    "LockoutEnd" timestamp with time zone NULL,
    "LockoutEnabled" boolean NOT NULL,
    "AccessFailedCount" integer NOT NULL,
    CONSTRAINT "PK_AspNetUsers" PRIMARY KEY ("Id")
);

CREATE TABLE "AspNetRoleClaims" (
    "Id" integer NOT NULL GENERATED BY DEFAULT AS IDENTITY,
    "RoleId" uuid NOT NULL,
    "ClaimType" text NULL,
    "ClaimValue" text NULL,
    CONSTRAINT "PK_AspNetRoleClaims" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_AspNetRoleClaims_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserClaims" (
    "Id" integer NOT NULL GENERATED BY DEFAULT AS IDENTITY,
    "UserId" uuid NOT NULL,
    "ClaimType" text NULL,
    "ClaimValue" text NULL,
    CONSTRAINT "PK_AspNetUserClaims" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_AspNetUserClaims_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserLogins" (
    "LoginProvider" text NOT NULL,
    "ProviderKey" text NOT NULL,
    "ProviderDisplayName" text NULL,
    "UserId" uuid NOT NULL,
    CONSTRAINT "PK_AspNetUserLogins" PRIMARY KEY ("LoginProvider", "ProviderKey"),
    CONSTRAINT "FK_AspNetUserLogins_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserRoles" (
    "UserId" uuid NOT NULL,
    "RoleId" uuid NOT NULL,
    CONSTRAINT "PK_AspNetUserRoles" PRIMARY KEY ("UserId", "RoleId"),
    CONSTRAINT "FK_AspNetUserRoles_AspNetRoles_RoleId" FOREIGN KEY ("RoleId") REFERENCES "AspNetRoles" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_AspNetUserRoles_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE TABLE "AspNetUserTokens" (
    "UserId" uuid NOT NULL,
    "LoginProvider" text NOT NULL,
    "Name" text NOT NULL,
    "Value" text NULL,
    CONSTRAINT "PK_AspNetUserTokens" PRIMARY KEY ("UserId", "LoginProvider", "Name"),
    CONSTRAINT "FK_AspNetUserTokens_AspNetUsers_UserId" FOREIGN KEY ("UserId") REFERENCES "AspNetUsers" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_AspNetRoleClaims_RoleId" ON "AspNetRoleClaims" ("RoleId");

CREATE UNIQUE INDEX "RoleNameIndex" ON "AspNetRoles" ("NormalizedName");

CREATE INDEX "IX_AspNetUserClaims_UserId" ON "AspNetUserClaims" ("UserId");

CREATE INDEX "IX_AspNetUserLogins_UserId" ON "AspNetUserLogins" ("UserId");

CREATE INDEX "IX_AspNetUserRoles_RoleId" ON "AspNetUserRoles" ("RoleId");

CREATE INDEX "EmailIndex" ON "AspNetUsers" ("NormalizedEmail");

CREATE UNIQUE INDEX "UserNameIndex" ON "AspNetUsers" ("NormalizedUserName");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20200612191854_Initial', '3.1.6');

CREATE EXTENSION IF NOT EXISTS "uuid-ossp";

CREATE TABLE "UserProfiles" (
    "Id" uuid NOT NULL,
    "PublicId" text NOT NULL,
    "CreateDate" timestamp without time zone NOT NULL,
    "UpdateDate" timestamp without time zone NOT NULL,
    "Email" text NULL,
    "Name" text NULL,
    "ApplicationUserId" uuid NOT NULL,
    "Status" integer NOT NULL,
    CONSTRAINT "PK_UserProfiles" PRIMARY KEY ("Id")
);

CREATE TABLE "Tasks" (
    "Id" uuid NOT NULL,
    "PublicId" text NOT NULL,
    "CreateDate" timestamp without time zone NOT NULL,
    "UpdateDate" timestamp without time zone NOT NULL,
    "ProfileId" uuid NULL,
    "Title" text NOT NULL,
    CONSTRAINT "PK_Tasks" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Tasks_UserProfiles_ProfileId" FOREIGN KEY ("ProfileId") REFERENCES "UserProfiles" ("Id") ON DELETE RESTRICT
);

CREATE TABLE "TimeLog" (
    "Id" uuid NOT NULL,
    "PublicId" text NOT NULL,
    "CreateDate" timestamp without time zone NOT NULL,
    "UpdateDate" timestamp without time zone NOT NULL,
    "StartTime" timestamp without time zone NOT NULL,
    "ElapsedMilliseconds" bigint NOT NULL,
    "TaskId" uuid NOT NULL,
    CONSTRAINT "PK_TimeLog" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_TimeLog_Tasks_TaskId" FOREIGN KEY ("TaskId") REFERENCES "Tasks" ("Id") ON DELETE CASCADE
);

CREATE TABLE "TaskState" (
    "Id" uuid NOT NULL,
    "TaskId" uuid NOT NULL,
    "IsRunning" boolean NOT NULL,
    "LatestTimeLogItemId" uuid NULL,
    CONSTRAINT "PK_TaskState" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_TaskState_TimeLog_LatestTimeLogItemId" FOREIGN KEY ("LatestTimeLogItemId") REFERENCES "TimeLog" ("Id") ON DELETE RESTRICT,
    CONSTRAINT "FK_TaskState_Tasks_TaskId" FOREIGN KEY ("TaskId") REFERENCES "Tasks" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_Tasks_ProfileId" ON "Tasks" ("ProfileId");

CREATE INDEX "IX_TaskState_LatestTimeLogItemId" ON "TaskState" ("LatestTimeLogItemId");

CREATE UNIQUE INDEX "IX_TaskState_TaskId" ON "TaskState" ("TaskId");

CREATE INDEX "IX_TimeLog_TaskId" ON "TimeLog" ("TaskId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20200720171355_Tasks_And_Logs', '3.1.6');

