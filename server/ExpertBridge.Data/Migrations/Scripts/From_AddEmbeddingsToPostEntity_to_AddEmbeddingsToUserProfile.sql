-- 20250429062455_SmallTweaksToModel Migration.
START TRANSACTION;
ALTER TABLE "Profiles"
    ALTER COLUMN "CreatedAt" DROP NOT NULL;

ALTER TABLE "ProfileExperiences"
    ALTER COLUMN "CreatedAt" DROP NOT NULL;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250429062455_SmallTweaksToModel', '9.0.4');
COMMIT;

-- 20250429192911_ModifyTagEntity Migration.
START TRANSACTION;
ALTER TABLE "Tags" RENAME COLUMN "Name" TO "EnglishName";

ALTER
INDEX "IX_Tags_Name" RENAME TO "IX_Tags_EnglishName";

ALTER TABLE "Posts" RENAME COLUMN "Embeddings" TO "Embedding";

ALTER TABLE "Tags"
    ADD "ArabicName" text NOT NULL DEFAULT '';

ALTER TABLE "Tags"
    ADD "CreatedAt" timestamp with time zone;

ALTER TABLE "Tags"
    ADD "LastModified" timestamp with time zone;

ALTER TABLE "Posts"
    ADD "Language" text;

CREATE UNIQUE INDEX "IX_Tags_ArabicName" ON "Tags" ("ArabicName");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250429192911_ModifyTagEntity', '9.0.4');

COMMIT;

-- 20250430123708_AddEmbeddingToUserProfile Migration.
START TRANSACTION;
DROP TABLE "ProfileTags";

ALTER TABLE "Profiles"
    ADD "UserInterestEmbedding" vector(1024);

CREATE TABLE "UserInterests"
(
    "ProfileId" character varying(450) NOT NULL,
    "TagId"     character varying(450) NOT NULL,
    CONSTRAINT "PK_UserInterests" PRIMARY KEY ("ProfileId", "TagId"),
    CONSTRAINT "FK_UserInterests_Profiles_ProfileId" FOREIGN KEY ("ProfileId") REFERENCES "Profiles" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_UserInterests_Tags_TagId" FOREIGN KEY ("TagId") REFERENCES "Tags" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_UserInterests_TagId" ON "UserInterests" ("TagId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250430123708_AddEmbeddingToUserProfile', '9.0.4');

COMMIT;

