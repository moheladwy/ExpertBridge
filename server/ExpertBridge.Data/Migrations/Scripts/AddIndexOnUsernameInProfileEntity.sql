START TRANSACTION;
ALTER TABLE "Profiles" ALTER COLUMN "Username" TYPE character varying(256);

CREATE UNIQUE INDEX "IX_Profiles_Username" ON "Profiles" ("Username") WHERE ("IsDeleted") = false;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250517213239_AddIndexOnUsernameInProfileEntity', '9.0.4');

COMMIT;

