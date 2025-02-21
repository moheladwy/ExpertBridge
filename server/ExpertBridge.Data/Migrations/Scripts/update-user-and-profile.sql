START TRANSACTION;
ALTER TABLE "Users" RENAME COLUMN "isDeleted" TO "IsDeleted";

ALTER TABLE "Users" RENAME COLUMN "isBanned" TO "IsBanned";

ALTER TABLE "Users" ADD "IsEmailVerified" boolean NOT NULL DEFAULT FALSE;

ALTER TABLE "Users" ADD "PhoneNumber" character varying(20);

ALTER TABLE "Profiles" ADD "ProfilePictureUrl" character varying(2048);

ALTER TABLE "Profiles" ADD "RatingCount" integer NOT NULL DEFAULT 0;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250221132034_UpdateUserAndProfileEntities', '9.0.1');

COMMIT;

