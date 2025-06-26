START TRANSACTION;
UPDATE "Users" SET "Username" = '' WHERE "Username" IS NULL;
ALTER TABLE "Users" ALTER COLUMN "Username" SET NOT NULL;
ALTER TABLE "Users" ALTER COLUMN "Username" SET DEFAULT '';

UPDATE "Profiles" SET "Username" = '' WHERE "Username" IS NULL;
ALTER TABLE "Profiles" ALTER COLUMN "Username" SET NOT NULL;
ALTER TABLE "Profiles" ALTER COLUMN "Username" SET DEFAULT '';

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250624180354_MakeUsernameNotNullInDb', '9.0.4');

COMMIT;

