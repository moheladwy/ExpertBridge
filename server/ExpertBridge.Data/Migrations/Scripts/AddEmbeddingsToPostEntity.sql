START TRANSACTION;
ALTER TABLE "Posts"
    ADD "Embeddings" vector(1024);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250428023010_AddEmbeddingsToPostEntity', '9.0.4');

COMMIT;

