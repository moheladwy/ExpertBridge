START TRANSACTION;
ALTER TABLE "Users" ADD "DeletedAt" timestamp with time zone;

ALTER TABLE "Profiles" ADD "DeletedAt" timestamp with time zone;

ALTER TABLE "Posts" ADD "DeletedAt" timestamp with time zone;

ALTER TABLE "Comments" ADD "DeletedAt" timestamp with time zone;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250415074659_MigrateDBToProd', '9.0.2');

ALTER TABLE "PostVotes" ADD "DeletedAt" timestamp with time zone;

ALTER TABLE "PostVotes" ADD "IsDeleted" boolean NOT NULL DEFAULT FALSE;

ALTER TABLE "PostVotes" ADD "LastModified" timestamp with time zone;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250415215831_AddTimesToPostVote', '9.0.2');

COMMIT;

