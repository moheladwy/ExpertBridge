START TRANSACTION;
ALTER TABLE "Posts"
    ADD "IsSafeContent" boolean NOT NULL DEFAULT FALSE;

ALTER TABLE "JobPostings"
    ADD "IsSafeContent" boolean NOT NULL DEFAULT FALSE;

ALTER TABLE "Comments"
    ADD "IsSafeContent" boolean NOT NULL DEFAULT FALSE;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20251012154018_AddIsSafeFlagToPostsAndComments', '9.0.9');

COMMIT;

