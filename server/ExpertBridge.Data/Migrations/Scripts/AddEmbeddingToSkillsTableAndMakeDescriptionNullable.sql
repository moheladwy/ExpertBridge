START TRANSACTION;
ALTER TABLE "Skills"
    ALTER COLUMN "Description" DROP NOT NULL;

ALTER TABLE "Skills"
    ADD "Embedding" vector(1024);

CREATE INDEX "IX_Skills_Embedding" ON "Skills" USING hnsw ("Embedding" vector_cosine_ops) WITH (ef_construction=128, m=64);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250703205231_AddEmbeddingToSkillsTableAndMakeDescriptionNullable', '9.0.6');

COMMIT;

