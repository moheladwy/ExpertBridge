START TRANSACTION;
CREATE INDEX "IX_Posts_Embedding" ON "Posts" USING hnsw ("Embedding" vector_cosine_ops);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250517122905_AddIndexOnPostEmbedding', '9.0.4');

COMMIT;

