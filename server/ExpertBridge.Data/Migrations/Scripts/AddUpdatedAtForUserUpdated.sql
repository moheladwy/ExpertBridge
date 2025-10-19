START TRANSACTION;
ALTER TABLE "Posts"
    ADD "UpdatedAt" timestamp with time zone;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250629012029_AddUpdatedAtForUserUpdated', '9.0.4');

COMMIT;

