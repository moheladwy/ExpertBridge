START TRANSACTION;
CREATE EXTENSION IF NOT EXISTS vector;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250427190207_AddVectorExtensionToDatabase', '9.0.4');

COMMIT;

