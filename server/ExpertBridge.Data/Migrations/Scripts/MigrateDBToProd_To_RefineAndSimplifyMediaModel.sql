START TRANSACTION;
ALTER TABLE "PostVotes"
    ADD "DeletedAt" timestamp with time zone;

ALTER TABLE "PostVotes"
    ADD "IsDeleted" boolean NOT NULL DEFAULT FALSE;

ALTER TABLE "PostVotes"
    ADD "LastModified" timestamp with time zone;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250415215831_AddTimesToPostVote', '9.0.2');

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250417115204_IamJustTryingToFigureOutWhatsWrong', '9.0.2');

ALTER TABLE "PostVotes" DROP COLUMN "DeletedAt";

ALTER TABLE "PostVotes" DROP COLUMN "IsDeleted";

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250417122241_IamJustTryingToFigureOutWhatsWrongAgain', '9.0.2');

ALTER TABLE "CommentVotes"
    ADD "LastModified" timestamp with time zone;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250418090222_AddBaseModelToCommentVote', '9.0.2');

ALTER TABLE "Media" DROP CONSTRAINT "FK_Media_MediaTypes_MediaTypeId";

DROP TABLE "ChatMedias";

DROP TABLE "CommentMedias";

DROP TABLE "JobPostingMedias";

DROP TABLE "PostMedias";

DROP TABLE "ProfileExperienceMedias";

DROP TABLE "ProfileMedias";

ALTER TABLE "Media" DROP CONSTRAINT "PK_Media";

ALTER TABLE "Media" RENAME TO "MediaObject";

ALTER TABLE "MediaObject" RENAME COLUMN "MediaUrl" TO "Url";

ALTER
INDEX "IX_Media_MediaUrl" RENAME TO "IX_MediaObject_Url";

ALTER
INDEX "IX_Media_MediaTypeId" RENAME TO "IX_MediaObject_MediaTypeId";

ALTER TABLE "MediaObject"
    ALTER COLUMN "MediaTypeId" DROP NOT NULL;

ALTER TABLE "MediaObject"
    ADD "ChatId" character varying(450);

ALTER TABLE "MediaObject"
    ADD "CommentId" character varying(450);

ALTER TABLE "MediaObject"
    ADD "Discriminator" character varying(34) NOT NULL DEFAULT '';

ALTER TABLE "MediaObject"
    ADD "JobPostingId" character varying(450);

ALTER TABLE "MediaObject"
    ADD "Key" text NOT NULL DEFAULT '';

ALTER TABLE "MediaObject"
    ADD "PostId" character varying(450);

ALTER TABLE "MediaObject"
    ADD "ProfileExperienceId" character varying(450);

ALTER TABLE "MediaObject"
    ADD "ProfileId" character varying(450);

ALTER TABLE "MediaObject"
    ADD "Type" text NOT NULL DEFAULT '';

ALTER TABLE "MediaObject"
    ADD CONSTRAINT "PK_MediaObject" PRIMARY KEY ("Id");

CREATE INDEX "IX_MediaObject_ChatId" ON "MediaObject" ("ChatId");

CREATE UNIQUE INDEX "IX_MediaObject_CommentId" ON "MediaObject" ("CommentId");

CREATE INDEX "IX_MediaObject_JobPostingId" ON "MediaObject" ("JobPostingId");

CREATE INDEX "IX_MediaObject_PostId" ON "MediaObject" ("PostId");

CREATE INDEX "IX_MediaObject_ProfileExperienceId" ON "MediaObject" ("ProfileExperienceId");

CREATE INDEX "IX_MediaObject_ProfileId" ON "MediaObject" ("ProfileId");

ALTER TABLE "MediaObject"
    ADD CONSTRAINT "FK_MediaObject_Chats_ChatId" FOREIGN KEY ("ChatId") REFERENCES "Chats" ("Id") ON DELETE CASCADE;

ALTER TABLE "MediaObject"
    ADD CONSTRAINT "FK_MediaObject_Comments_CommentId" FOREIGN KEY ("CommentId") REFERENCES "Comments" ("Id") ON DELETE CASCADE;

ALTER TABLE "MediaObject"
    ADD CONSTRAINT "FK_MediaObject_JobPostings_JobPostingId" FOREIGN KEY ("JobPostingId") REFERENCES "JobPostings" ("Id") ON DELETE CASCADE;

ALTER TABLE "MediaObject"
    ADD CONSTRAINT "FK_MediaObject_MediaTypes_MediaTypeId" FOREIGN KEY ("MediaTypeId") REFERENCES "MediaTypes" ("Id");

ALTER TABLE "MediaObject"
    ADD CONSTRAINT "FK_MediaObject_Posts_PostId" FOREIGN KEY ("PostId") REFERENCES "Posts" ("Id") ON DELETE CASCADE;

ALTER TABLE "MediaObject"
    ADD CONSTRAINT "FK_MediaObject_ProfileExperiences_ProfileExperienceId" FOREIGN KEY ("ProfileExperienceId") REFERENCES "ProfileExperiences" ("Id") ON DELETE CASCADE;

ALTER TABLE "MediaObject"
    ADD CONSTRAINT "FK_MediaObject_Profiles_ProfileId" FOREIGN KEY ("ProfileId") REFERENCES "Profiles" ("Id") ON DELETE CASCADE;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250420151749_RefineAndSimplifyMediaModel', '9.0.2');

COMMIT;

