START TRANSACTION;
ALTER TABLE "Users" RENAME COLUMN "FirebaseId" TO "ProviderId";

ALTER
INDEX "IX_Users_FirebaseId" RENAME TO "IX_Users_ProviderId";

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250222144618_UpdateFirebaseIdProbNameToProviderId', '9.0.1');

COMMIT;

