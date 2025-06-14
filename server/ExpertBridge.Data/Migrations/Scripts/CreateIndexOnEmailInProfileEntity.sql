START TRANSACTION;
ALTER TABLE "Profiles" ALTER COLUMN "Email" TYPE character varying(256);

-- it's not possible to create a unique index on the column with duplicate email values,
-- so we need to delete the duplicate email values first
UPDATE public."Profiles"
set "IsDeleted" = true
WHERE "Id" = '335c721c-aff9-4a30-88ec-10ccdc1bb6e1';

CREATE UNIQUE INDEX "IX_Profiles_Email" ON "Profiles" ("Email") WHERE ("IsDeleted") = false;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250517214125_CreateIndexOnEmailInProfileEntity', '9.0.4');

COMMIT;

