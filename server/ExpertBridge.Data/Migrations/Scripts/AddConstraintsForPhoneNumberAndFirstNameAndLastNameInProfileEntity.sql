START TRANSACTION;
ALTER TABLE "Profiles" ALTER COLUMN "PhoneNumber" TYPE character varying(20);

ALTER TABLE "Profiles" ALTER COLUMN "LastName" TYPE character varying(256);
UPDATE "Profiles" SET "LastName" = '' WHERE "LastName" IS NULL;
ALTER TABLE "Profiles" ALTER COLUMN "LastName" SET NOT NULL;
ALTER TABLE "Profiles" ALTER COLUMN "LastName" SET DEFAULT '';

ALTER TABLE "Profiles" ALTER COLUMN "FirstName" TYPE character varying(256);
UPDATE "Profiles" SET "FirstName" = '' WHERE "FirstName" IS NULL;
ALTER TABLE "Profiles" ALTER COLUMN "FirstName" SET NOT NULL;
ALTER TABLE "Profiles" ALTER COLUMN "FirstName" SET DEFAULT '';

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250517215239_AddConstraintsForPhoneNumberAndFirstNameAndLastNameInProfileEntity', '9.0.4');

COMMIT;

