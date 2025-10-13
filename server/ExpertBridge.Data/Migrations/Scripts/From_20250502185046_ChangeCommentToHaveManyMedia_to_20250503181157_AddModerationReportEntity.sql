START TRANSACTION;
DROP INDEX "IX_CommentMedias_CommentId";

CREATE INDEX "IX_CommentMedias_CommentId" ON "CommentMedias" ("CommentId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250502185046_ChangeCommentToHaveManyMedia', '9.0.4');

ALTER TABLE "Comments"
    ADD "IsProcessed" boolean NOT NULL DEFAULT FALSE;

CREATE INDEX "IX_Posts_IsProcessed" ON "Posts" ("IsProcessed");

CREATE INDEX "IX_Posts_IsTagged" ON "Posts" ("IsTagged");

CREATE INDEX "IX_Comments_IsProcessed" ON "Comments" ("IsProcessed");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250502231504_AddIsProcessedPropToCommentAndIndexOnIt', '9.0.4');

ALTER TABLE "Tags"
    ALTER COLUMN "ArabicName" DROP NOT NULL;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250503135721_ArabicTagNameToNullable', '9.0.4');

CREATE TABLE "ModerationReports"
(
    "Id"             character varying(450) NOT NULL,
    "ContentType"    text                   NOT NULL,
    "ContentId"      text                   NOT NULL,
    "AuthorId"       text                   NOT NULL,
    "Reason"         text                   NOT NULL,
    "IsResolved"     boolean                NOT NULL,
    "IsNegative"     boolean                NOT NULL,
    "IsDeleted"      boolean                NOT NULL,
    "DeletedAt"      timestamp with time zone,
    "Toxicity"       double precision       NOT NULL,
    "SevereToxicity" double precision       NOT NULL,
    "Obscene"        double precision       NOT NULL,
    "Threat"         double precision       NOT NULL,
    "Insult"         double precision       NOT NULL,
    "IdentityAttack" double precision       NOT NULL,
    "SexualExplicit" double precision       NOT NULL,
    "CreatedAt"      timestamp with time zone,
    "LastModified"   timestamp with time zone,
    CONSTRAINT "PK_ModerationReports" PRIMARY KEY ("Id")
);

CREATE INDEX "IX_ModerationReports_ContentId" ON "ModerationReports" ("ContentId");

CREATE INDEX "IX_ModerationReports_IsNegative" ON "ModerationReports" ("IsNegative");

CREATE INDEX "IX_ModerationReports_IsNegative_ContentId" ON "ModerationReports" ("IsNegative", "ContentId");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250503181157_AddModerationReportEntity', '9.0.4');

COMMIT;

