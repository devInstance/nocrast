ALTER TABLE "Projects" ADD "Color" integer NOT NULL DEFAULT 0;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20201027220838_ProjectColor', '3.1.9');
