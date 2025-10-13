START TRANSACTION;
ALTER TABLE "Comments" DROP CONSTRAINT "FK_Comments_Comments_ParentId";

ALTER TABLE "Comments" DROP CONSTRAINT "FK_Comments_Posts_PostId";

ALTER TABLE "Comments" RENAME COLUMN "ParentId" TO "ParentCommentId";

ALTER
INDEX "IX_Comments_ParentId" RENAME TO "IX_Comments_ParentCommentId";

UPDATE "Comments"
SET "PostId" = ''
WHERE "PostId" IS NULL;
ALTER TABLE "Comments"
    ALTER COLUMN "PostId" SET NOT NULL;
ALTER TABLE "Comments"
    ALTER COLUMN "PostId" SET DEFAULT '';

ALTER TABLE "Comments"
    ADD CONSTRAINT "FK_Comments_Comments_ParentCommentId" FOREIGN KEY ("ParentCommentId") REFERENCES "Comments" ("Id") ON DELETE CASCADE;

ALTER TABLE "Comments"
    ADD CONSTRAINT "FK_Comments_Posts_PostId" FOREIGN KEY ("PostId") REFERENCES "Posts" ("Id") ON DELETE CASCADE;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250318052103_RefineCommentsRelationships', '9.0.2');

ALTER TABLE "Users" RENAME COLUMN "IsOnBoarded" TO "IsOnboarded";

ALTER TABLE "Posts" RENAME COLUMN "isDeleted" TO "IsDeleted";

ALTER TABLE "Comments" RENAME COLUMN "isDeleted" TO "IsDeleted";

ALTER TABLE "Posts"
    ADD "IsProcessed" boolean NOT NULL DEFAULT FALSE;

ALTER TABLE "Posts"
    ADD "IsTagged" boolean NOT NULL DEFAULT FALSE;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250321193023_RenameSomeFields', '9.0.2');

ALTER TABLE "Profiles"
    ADD "CreatedAt" timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';

ALTER TABLE "Profiles"
    ADD "Email" text NOT NULL DEFAULT '';

ALTER TABLE "Profiles"
    ADD "FirstName" text NOT NULL DEFAULT '';

ALTER TABLE "Profiles"
    ADD "IsBanned" boolean NOT NULL DEFAULT FALSE;

ALTER TABLE "Profiles"
    ADD "IsDeleted" boolean NOT NULL DEFAULT FALSE;

ALTER TABLE "Profiles"
    ADD "LastName" text;

ALTER TABLE "Profiles"
    ADD "PhoneNumber" text;

ALTER TABLE "Profiles"
    ADD "Username" text NOT NULL DEFAULT '';

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250321220207_AddUserInfoToProfileEntity', '9.0.2');

ALTER TABLE "Users"
    ADD "LastModified" timestamp with time zone;

ALTER TABLE "Profiles"
    ALTER COLUMN "Username" DROP NOT NULL;

ALTER TABLE "Profiles"
    ALTER COLUMN "FirstName" DROP NOT NULL;

ALTER TABLE "Profiles"
    ADD "LastModified" timestamp with time zone;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250322042200_FurtherTuneTheModelsForUseCases', '9.0.2');

ALTER TABLE "Users"
    ALTER COLUMN "Username" DROP NOT NULL;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250322210005_RemoveNotNullConstrainsOnUser.Username', '9.0.2');

COMMIT;

