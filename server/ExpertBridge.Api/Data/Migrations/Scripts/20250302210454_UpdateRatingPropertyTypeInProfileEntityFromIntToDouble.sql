START TRANSACTION;
ALTER TABLE "Profiles" ALTER COLUMN "Rating" TYPE double precision;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250302210454_UpdateRatingPropertyTypeInProfileEntityFromIntToDouble', '9.0.2');

COMMIT;

