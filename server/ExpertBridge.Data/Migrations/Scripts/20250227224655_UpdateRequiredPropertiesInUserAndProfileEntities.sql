START TRANSACTION;
ALTER TABLE "Profiles"
    ALTER COLUMN "JobTitle" DROP NOT NULL;

ALTER TABLE "Profiles"
    ALTER COLUMN "Bio" DROP NOT NULL;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250227224655_UpdateRequiredPropertiesInUserAndProfileEntities', '9.0.2');

COMMIT;

