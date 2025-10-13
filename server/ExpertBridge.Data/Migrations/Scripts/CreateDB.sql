CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory"
(
    "MigrationId"
    character
    varying
(
    150
) NOT NULL,
    "ProductVersion" character varying
(
    32
) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY
(
    "MigrationId"
)
    );

START TRANSACTION;
CREATE TABLE "Badges"
(
    "Id"          character varying(450) NOT NULL,
    "Name"        character varying(256) NOT NULL,
    "Description" character varying(500) NOT NULL,
    CONSTRAINT "PK_Badges" PRIMARY KEY ("Id")
);

CREATE TABLE "Chats"
(
    "Id"        character varying(450)   NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "EndedAt"   timestamp with time zone,
    CONSTRAINT "PK_Chats" PRIMARY KEY ("Id")
);

CREATE TABLE "JobCategories"
(
    "Id"          character varying(450) NOT NULL,
    "Name"        character varying(256) NOT NULL,
    "Description" character varying(512) NOT NULL,
    CONSTRAINT "PK_JobCategories" PRIMARY KEY ("Id")
);

CREATE TABLE "JobStatuses"
(
    "Id"     character varying(450) NOT NULL,
    "Status" character varying(128) NOT NULL,
    CONSTRAINT "PK_JobStatuses" PRIMARY KEY ("Id")
);

CREATE TABLE "MediaTypes"
(
    "Id"   character varying(450) NOT NULL,
    "Type" character varying(128) NOT NULL,
    CONSTRAINT "PK_MediaTypes" PRIMARY KEY ("Id")
);

CREATE TABLE "Skills"
(
    "Id"          character varying(450) NOT NULL,
    "Name"        character varying(256) NOT NULL,
    "Description" character varying(256) NOT NULL,
    CONSTRAINT "PK_Skills" PRIMARY KEY ("Id")
);

CREATE TABLE "Tags"
(
    "Id"          character varying(450) NOT NULL,
    "Name"        character varying(256) NOT NULL,
    "Description" character varying(512) NOT NULL,
    CONSTRAINT "PK_Tags" PRIMARY KEY ("Id")
);

CREATE TABLE "Users"
(
    "Id"         character varying(450)   NOT NULL,
    "FirebaseId" character varying(450)   NOT NULL,
    "FirstName"  character varying(256)   NOT NULL,
    "LastName"   character varying(256)   NOT NULL,
    "Email"      character varying(256)   NOT NULL,
    "Username"   character varying(256)   NOT NULL,
    "isBanned"   boolean                  NOT NULL,
    "isDeleted"  boolean                  NOT NULL,
    "CreatedAt"  timestamp with time zone NOT NULL,
    CONSTRAINT "PK_Users" PRIMARY KEY ("Id")
);

CREATE TABLE "Media"
(
    "Id"           character varying(450)   NOT NULL,
    "Name"         character varying(256)   NOT NULL,
    "MediaUrl"     character varying(2048)  NOT NULL,
    "CreatedAt"    timestamp with time zone NOT NULL,
    "LastModified" timestamp with time zone,
    "MediaTypeId"  character varying(450)   NOT NULL,
    CONSTRAINT "PK_Media" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Media_MediaTypes_MediaTypeId" FOREIGN KEY ("MediaTypeId") REFERENCES "MediaTypes" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Profiles"
(
    "Id"       character varying(450)  NOT NULL,
    "UserId"   character varying(450)  NOT NULL,
    "JobTitle" character varying(256)  NOT NULL,
    "Bio"      character varying(5000) NOT NULL,
    "Rating"   integer                 NOT NULL,
    CONSTRAINT "PK_Profiles" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Profiles_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ChatMedias"
(
    "Id"      character varying(450) NOT NULL,
    "ChatId"  character varying(450) NOT NULL,
    "MediaId" character varying(450) NOT NULL,
    CONSTRAINT "PK_ChatMedias" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_ChatMedias_Chats_ChatId" FOREIGN KEY ("ChatId") REFERENCES "Chats" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_ChatMedias_Media_MediaId" FOREIGN KEY ("MediaId") REFERENCES "Media" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Areas"
(
    "Id"          character varying(450) NOT NULL,
    "ProfileId"   character varying(450) NOT NULL,
    "Governorate" character varying(256) NOT NULL,
    "Region"      character varying(256) NOT NULL,
    CONSTRAINT "PK_Areas" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Areas_Profiles_ProfileId" FOREIGN KEY ("ProfileId") REFERENCES "Profiles" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ChatParticipants"
(
    "ChatId"    character varying(450) NOT NULL,
    "ProfileId" character varying(450) NOT NULL,
    CONSTRAINT "PK_ChatParticipants" PRIMARY KEY ("ChatId", "ProfileId"),
    CONSTRAINT "FK_ChatParticipants_Chats_ChatId" FOREIGN KEY ("ChatId") REFERENCES "Chats" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_ChatParticipants_Profiles_ProfileId" FOREIGN KEY ("ProfileId") REFERENCES "Profiles" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Posts"
(
    "Id"           character varying(450)   NOT NULL,
    "Title"        character varying(256)   NOT NULL,
    "Content"      character varying(5000)  NOT NULL,
    "AuthorId"     character varying(450)   NOT NULL,
    "CreatedAt"    timestamp with time zone NOT NULL,
    "LastModified" timestamp with time zone,
    "isDeleted"    boolean                  NOT NULL,
    CONSTRAINT "PK_Posts" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Posts_Profiles_AuthorId" FOREIGN KEY ("AuthorId") REFERENCES "Profiles" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ProfileBadges"
(
    "ProfileId" character varying(450) NOT NULL,
    "BadgeId"   character varying(450) NOT NULL,
    CONSTRAINT "PK_ProfileBadges" PRIMARY KEY ("ProfileId", "BadgeId"),
    CONSTRAINT "FK_ProfileBadges_Badges_BadgeId" FOREIGN KEY ("BadgeId") REFERENCES "Badges" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_ProfileBadges_Profiles_ProfileId" FOREIGN KEY ("ProfileId") REFERENCES "Profiles" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ProfileExperiences"
(
    "Id"          character varying(450)   NOT NULL,
    "ProfileId"   character varying(450)   NOT NULL,
    "Title"       character varying(256)   NOT NULL,
    "Description" character varying(500)   NOT NULL,
    "Company"     character varying(500)   NOT NULL,
    "Location"    character varying(500)   NOT NULL,
    "StartDate"   timestamp with time zone NOT NULL,
    "EndDate"     timestamp with time zone,
    CONSTRAINT "PK_ProfileExperiences" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_ProfileExperiences_Profiles_ProfileId" FOREIGN KEY ("ProfileId") REFERENCES "Profiles" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ProfileMedias"
(
    "Id"        character varying(450) NOT NULL,
    "ProfileId" character varying(450) NOT NULL,
    "MediaId"   character varying(450) NOT NULL,
    CONSTRAINT "PK_ProfileMedias" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_ProfileMedias_Media_MediaId" FOREIGN KEY ("MediaId") REFERENCES "Media" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_ProfileMedias_Profiles_ProfileId" FOREIGN KEY ("ProfileId") REFERENCES "Profiles" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ProfileSkills"
(
    "ProfileId" character varying(450) NOT NULL,
    "SkillId"   character varying(450) NOT NULL,
    CONSTRAINT "PK_ProfileSkills" PRIMARY KEY ("ProfileId", "SkillId"),
    CONSTRAINT "FK_ProfileSkills_Profiles_ProfileId" FOREIGN KEY ("ProfileId") REFERENCES "Profiles" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_ProfileSkills_Skills_SkillId" FOREIGN KEY ("SkillId") REFERENCES "Skills" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ProfileTags"
(
    "ProfileId" character varying(450) NOT NULL,
    "TagId"     character varying(450) NOT NULL,
    CONSTRAINT "PK_ProfileTags" PRIMARY KEY ("ProfileId", "TagId"),
    CONSTRAINT "FK_ProfileTags_Profiles_ProfileId" FOREIGN KEY ("ProfileId") REFERENCES "Profiles" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_ProfileTags_Tags_TagId" FOREIGN KEY ("TagId") REFERENCES "Tags" ("Id") ON DELETE CASCADE
);

CREATE TABLE "JobPostings"
(
    "Id"          character varying(450)   NOT NULL,
    "Title"       character varying(256)   NOT NULL,
    "Description" character varying(5000)  NOT NULL,
    "Cost"        double precision         NOT NULL,
    "CreatedAt"   timestamp with time zone NOT NULL,
    "UpdatedAt"   timestamp with time zone,
    "AuthorId"    character varying(450)   NOT NULL,
    "AreaId"      character varying(450)   NOT NULL,
    "CategoryId"  character varying(450)   NOT NULL,
    CONSTRAINT "PK_JobPostings" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_JobPostings_Areas_AreaId" FOREIGN KEY ("AreaId") REFERENCES "Areas" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_JobPostings_JobCategories_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "JobCategories" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_JobPostings_Profiles_AuthorId" FOREIGN KEY ("AuthorId") REFERENCES "Profiles" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Comments"
(
    "Id"           character varying(450)   NOT NULL,
    "AuthorId"     character varying(450)   NOT NULL,
    "ParentId"     character varying(450),
    "Content"      character varying(5000)  NOT NULL,
    "CreatedAt"    timestamp with time zone NOT NULL,
    "LastModified" timestamp with time zone,
    "isDeleted"    boolean                  NOT NULL,
    "PostId"       character varying(450),
    CONSTRAINT "PK_Comments" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Comments_Comments_ParentId" FOREIGN KEY ("ParentId") REFERENCES "Comments" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Comments_Posts_PostId" FOREIGN KEY ("PostId") REFERENCES "Posts" ("Id"),
    CONSTRAINT "FK_Comments_Profiles_AuthorId" FOREIGN KEY ("AuthorId") REFERENCES "Profiles" ("Id") ON DELETE CASCADE
);

CREATE TABLE "PostMedias"
(
    "Id"      character varying(450) NOT NULL,
    "PostId"  character varying(450) NOT NULL,
    "MediaId" character varying(450) NOT NULL,
    CONSTRAINT "PK_PostMedias" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_PostMedias_Media_MediaId" FOREIGN KEY ("MediaId") REFERENCES "Media" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_PostMedias_Posts_PostId" FOREIGN KEY ("PostId") REFERENCES "Posts" ("Id") ON DELETE CASCADE
);

CREATE TABLE "PostTags"
(
    "PostId" character varying(450) NOT NULL,
    "TagId"  character varying(450) NOT NULL,
    CONSTRAINT "PK_PostTags" PRIMARY KEY ("PostId", "TagId"),
    CONSTRAINT "FK_PostTags_Posts_PostId" FOREIGN KEY ("PostId") REFERENCES "Posts" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_PostTags_Tags_TagId" FOREIGN KEY ("TagId") REFERENCES "Tags" ("Id") ON DELETE CASCADE
);

CREATE TABLE "PostVotes"
(
    "Id"        character varying(450)   NOT NULL,
    "IsUpvote"  boolean                  NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "ProfileId" character varying(450)   NOT NULL,
    "PostId"    character varying(450)   NOT NULL,
    CONSTRAINT "PK_PostVotes" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_PostVotes_Posts_PostId" FOREIGN KEY ("PostId") REFERENCES "Posts" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_PostVotes_Profiles_ProfileId" FOREIGN KEY ("ProfileId") REFERENCES "Profiles" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ProfileExperienceMedias"
(
    "Id"                  character varying(450) NOT NULL,
    "ProfileExperienceId" character varying(450) NOT NULL,
    "MediaId"             character varying(450) NOT NULL,
    CONSTRAINT "PK_ProfileExperienceMedias" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_ProfileExperienceMedias_Media_MediaId" FOREIGN KEY ("MediaId") REFERENCES "Media" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_ProfileExperienceMedias_ProfileExperiences_ProfileExperienc~" FOREIGN KEY ("ProfileExperienceId") REFERENCES "ProfileExperiences" ("Id") ON DELETE CASCADE
);

CREATE TABLE "JobPostingMedias"
(
    "Id"           character varying(450) NOT NULL,
    "JobPostingId" character varying(450) NOT NULL,
    "MediaId"      character varying(450) NOT NULL,
    CONSTRAINT "PK_JobPostingMedias" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_JobPostingMedias_JobPostings_JobPostingId" FOREIGN KEY ("JobPostingId") REFERENCES "JobPostings" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_JobPostingMedias_Media_MediaId" FOREIGN KEY ("MediaId") REFERENCES "Media" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Jobs"
(
    "Id"           character varying(450)   NOT NULL,
    "ActualCost"   double precision         NOT NULL,
    "StartedAt"    timestamp with time zone NOT NULL,
    "EndedAt"      timestamp with time zone,
    "JobStatusId"  character varying(450)   NOT NULL,
    "WorkerId"     character varying(450)   NOT NULL,
    "AuthorId"     character varying(450)   NOT NULL,
    "JobPostingId" character varying(450),
    CONSTRAINT "PK_Jobs" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Jobs_JobPostings_JobPostingId" FOREIGN KEY ("JobPostingId") REFERENCES "JobPostings" ("Id"),
    CONSTRAINT "FK_Jobs_JobStatuses_JobStatusId" FOREIGN KEY ("JobStatusId") REFERENCES "JobStatuses" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Jobs_Profiles_AuthorId" FOREIGN KEY ("AuthorId") REFERENCES "Profiles" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Jobs_Profiles_WorkerId" FOREIGN KEY ("WorkerId") REFERENCES "Profiles" ("Id") ON DELETE CASCADE
);

CREATE TABLE "CommentMedias"
(
    "Id"        character varying(450) NOT NULL,
    "CommentId" character varying(450) NOT NULL,
    "MediaId"   character varying(450) NOT NULL,
    CONSTRAINT "PK_CommentMedias" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_CommentMedias_Comments_CommentId" FOREIGN KEY ("CommentId") REFERENCES "Comments" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_CommentMedias_Media_MediaId" FOREIGN KEY ("MediaId") REFERENCES "Media" ("Id") ON DELETE CASCADE
);

CREATE TABLE "CommentVotes"
(
    "Id"        character varying(450)   NOT NULL,
    "IsUpvote"  boolean                  NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "CommentId" character varying(450)   NOT NULL,
    "ProfileId" character varying(450)   NOT NULL,
    CONSTRAINT "PK_CommentVotes" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_CommentVotes_Comments_CommentId" FOREIGN KEY ("CommentId") REFERENCES "Comments" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_CommentVotes_Profiles_ProfileId" FOREIGN KEY ("ProfileId") REFERENCES "Profiles" ("Id") ON DELETE CASCADE
);

CREATE TABLE "JobReviews"
(
    "Id"           character varying(450)   NOT NULL,
    "Content"      character varying(5000)  NOT NULL,
    "Rating"       integer                  NOT NULL,
    "CreatedAt"    timestamp with time zone NOT NULL,
    "LastModified" timestamp with time zone,
    "IsDeleted"    boolean                  NOT NULL,
    "WorkerId"     character varying(450)   NOT NULL,
    "CustomerId"   character varying(450)   NOT NULL,
    "JobId"        character varying(450)   NOT NULL,
    CONSTRAINT "PK_JobReviews" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_JobReviews_Jobs_JobId" FOREIGN KEY ("JobId") REFERENCES "Jobs" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_JobReviews_Profiles_CustomerId" FOREIGN KEY ("CustomerId") REFERENCES "Profiles" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_JobReviews_Profiles_WorkerId" FOREIGN KEY ("WorkerId") REFERENCES "Profiles" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_Areas_ProfileId" ON "Areas" ("ProfileId");

CREATE UNIQUE INDEX "IX_Badges_Name" ON "Badges" ("Name");

CREATE INDEX "IX_ChatMedias_ChatId" ON "ChatMedias" ("ChatId");

CREATE UNIQUE INDEX "IX_ChatMedias_MediaId" ON "ChatMedias" ("MediaId");

CREATE UNIQUE INDEX "IX_ChatParticipants_ProfileId" ON "ChatParticipants" ("ProfileId");

CREATE UNIQUE INDEX "IX_CommentMedias_CommentId" ON "CommentMedias" ("CommentId");

CREATE UNIQUE INDEX "IX_CommentMedias_MediaId" ON "CommentMedias" ("MediaId");

CREATE INDEX "IX_Comments_AuthorId" ON "Comments" ("AuthorId");

CREATE INDEX "IX_Comments_ParentId" ON "Comments" ("ParentId");

CREATE INDEX "IX_Comments_PostId" ON "Comments" ("PostId");

CREATE INDEX "IX_CommentVotes_CommentId" ON "CommentVotes" ("CommentId");

CREATE UNIQUE INDEX "IX_CommentVotes_ProfileId_CommentId" ON "CommentVotes" ("ProfileId", "CommentId");

CREATE UNIQUE INDEX "IX_JobCategories_Name" ON "JobCategories" ("Name");

CREATE INDEX "IX_JobPostingMedias_JobPostingId" ON "JobPostingMedias" ("JobPostingId");

CREATE UNIQUE INDEX "IX_JobPostingMedias_MediaId" ON "JobPostingMedias" ("MediaId");

CREATE INDEX "IX_JobPostings_AreaId" ON "JobPostings" ("AreaId");

CREATE INDEX "IX_JobPostings_AuthorId" ON "JobPostings" ("AuthorId");

CREATE INDEX "IX_JobPostings_CategoryId" ON "JobPostings" ("CategoryId");

CREATE INDEX "IX_JobPostings_Title" ON "JobPostings" ("Title");

CREATE INDEX "IX_JobReviews_CustomerId" ON "JobReviews" ("CustomerId");

CREATE UNIQUE INDEX "IX_JobReviews_JobId" ON "JobReviews" ("JobId");

CREATE INDEX "IX_JobReviews_WorkerId" ON "JobReviews" ("WorkerId");

CREATE INDEX "IX_Jobs_AuthorId" ON "Jobs" ("AuthorId");

CREATE UNIQUE INDEX "IX_Jobs_JobPostingId" ON "Jobs" ("JobPostingId");

CREATE INDEX "IX_Jobs_JobStatusId" ON "Jobs" ("JobStatusId");

CREATE INDEX "IX_Jobs_WorkerId" ON "Jobs" ("WorkerId");

CREATE INDEX "IX_Media_MediaTypeId" ON "Media" ("MediaTypeId");

CREATE UNIQUE INDEX "IX_Media_MediaUrl" ON "Media" ("MediaUrl");

CREATE UNIQUE INDEX "IX_PostMedias_MediaId" ON "PostMedias" ("MediaId");

CREATE INDEX "IX_PostMedias_PostId" ON "PostMedias" ("PostId");

CREATE INDEX "IX_Posts_AuthorId" ON "Posts" ("AuthorId");

CREATE INDEX "IX_Posts_Title" ON "Posts" ("Title");

CREATE INDEX "IX_PostTags_TagId" ON "PostTags" ("TagId");

CREATE INDEX "IX_PostVotes_PostId" ON "PostVotes" ("PostId");

CREATE UNIQUE INDEX "IX_PostVotes_ProfileId_PostId" ON "PostVotes" ("ProfileId", "PostId");

CREATE INDEX "IX_ProfileBadges_BadgeId" ON "ProfileBadges" ("BadgeId");

CREATE UNIQUE INDEX "IX_ProfileExperienceMedias_MediaId" ON "ProfileExperienceMedias" ("MediaId");

CREATE INDEX "IX_ProfileExperienceMedias_ProfileExperienceId" ON "ProfileExperienceMedias" ("ProfileExperienceId");

CREATE INDEX "IX_ProfileExperiences_ProfileId" ON "ProfileExperiences" ("ProfileId");

CREATE UNIQUE INDEX "IX_ProfileMedias_MediaId" ON "ProfileMedias" ("MediaId");

CREATE INDEX "IX_ProfileMedias_ProfileId" ON "ProfileMedias" ("ProfileId");

CREATE UNIQUE INDEX "IX_Profiles_UserId" ON "Profiles" ("UserId");

CREATE INDEX "IX_ProfileSkills_SkillId" ON "ProfileSkills" ("SkillId");

CREATE INDEX "IX_ProfileTags_TagId" ON "ProfileTags" ("TagId");

CREATE UNIQUE INDEX "IX_Skills_Name" ON "Skills" ("Name");

CREATE UNIQUE INDEX "IX_Tags_Name" ON "Tags" ("Name");

CREATE UNIQUE INDEX "IX_Users_Email" ON "Users" ("Email");

CREATE UNIQUE INDEX "IX_Users_FirebaseId" ON "Users" ("FirebaseId");

CREATE UNIQUE INDEX "IX_Users_Username" ON "Users" ("Username");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250216234149_InitialMigrations', '9.0.2');

ALTER TABLE "Users" RENAME COLUMN "isDeleted" TO "IsDeleted";

ALTER TABLE "Users" RENAME COLUMN "isBanned" TO "IsBanned";

ALTER TABLE "Users"
    ADD "IsEmailVerified" boolean NOT NULL DEFAULT FALSE;

ALTER TABLE "Users"
    ADD "PhoneNumber" character varying(20);

ALTER TABLE "Profiles"
    ADD "ProfilePictureUrl" character varying(2048);

ALTER TABLE "Profiles"
    ADD "RatingCount" integer NOT NULL DEFAULT 0;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250221132034_UpdateUserAndProfileEntities', '9.0.2');

ALTER TABLE "Users"
    ADD "IsOnBoarded" boolean NOT NULL DEFAULT FALSE;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250222123239_AddOnBoardedPropertyInUserEntity', '9.0.2');

ALTER TABLE "Users" RENAME COLUMN "FirebaseId" TO "ProviderId";

ALTER
INDEX "IX_Users_FirebaseId" RENAME TO "IX_Users_ProviderId";

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250222144618_UpdateFirebaseIdProbNameToProviderId', '9.0.2');

ALTER TABLE "Profiles"
    ALTER COLUMN "JobTitle" DROP NOT NULL;

ALTER TABLE "Profiles"
    ALTER COLUMN "Bio" DROP NOT NULL;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250227224655_UpdateRequiredPropertiesInUserAndProfileEntities', '9.0.2');

ALTER TABLE "Profiles" ALTER COLUMN "Rating" TYPE double precision;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250302210454_UpdateRatingPropertyTypeInProfileEntityFromIntToDouble', '9.0.2');

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

ALTER TABLE "Users"
    ADD "DeletedAt" timestamp with time zone;

ALTER TABLE "Profiles"
    ADD "DeletedAt" timestamp with time zone;

ALTER TABLE "Posts"
    ADD "DeletedAt" timestamp with time zone;

ALTER TABLE "Comments"
    ADD "DeletedAt" timestamp with time zone;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250415074659_MigrateDBToProd', '9.0.2');

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

ALTER TABLE "MediaObject" DROP CONSTRAINT "FK_MediaObject_Chats_ChatId";

ALTER TABLE "MediaObject" DROP CONSTRAINT "FK_MediaObject_Comments_CommentId";

ALTER TABLE "MediaObject" DROP CONSTRAINT "FK_MediaObject_JobPostings_JobPostingId";

ALTER TABLE "MediaObject" DROP CONSTRAINT "FK_MediaObject_MediaTypes_MediaTypeId";

ALTER TABLE "MediaObject" DROP CONSTRAINT "FK_MediaObject_Posts_PostId";

ALTER TABLE "MediaObject" DROP CONSTRAINT "FK_MediaObject_ProfileExperiences_ProfileExperienceId";

ALTER TABLE "MediaObject" DROP CONSTRAINT "FK_MediaObject_Profiles_ProfileId";

DROP TABLE "MediaTypes";

DROP INDEX "IX_Users_Email";

DROP INDEX "IX_Users_ProviderId";

DROP INDEX "IX_Users_Username";

ALTER TABLE "MediaObject" DROP CONSTRAINT "PK_MediaObject";

DROP INDEX "IX_MediaObject_ChatId";

DROP INDEX "IX_MediaObject_CommentId";

DROP INDEX "IX_MediaObject_JobPostingId";

DROP INDEX "IX_MediaObject_MediaTypeId";

DROP INDEX "IX_MediaObject_PostId";

DROP INDEX "IX_MediaObject_ProfileExperienceId";

DROP INDEX "IX_MediaObject_Url";

ALTER TABLE "MediaObject" DROP COLUMN "ChatId";

ALTER TABLE "MediaObject" DROP COLUMN "CommentId";

ALTER TABLE "MediaObject" DROP COLUMN "Discriminator";

ALTER TABLE "MediaObject" DROP COLUMN "JobPostingId";

ALTER TABLE "MediaObject" DROP COLUMN "MediaTypeId";

ALTER TABLE "MediaObject" DROP COLUMN "PostId";

ALTER TABLE "MediaObject" DROP COLUMN "ProfileExperienceId";

ALTER TABLE "MediaObject" DROP COLUMN "Url";

ALTER TABLE "MediaObject" RENAME TO "ProfileMedias";

ALTER
INDEX "IX_MediaObject_ProfileId" RENAME TO "IX_ProfileMedias_ProfileId";

UPDATE "ProfileMedias"
SET "ProfileId" = ''
WHERE "ProfileId" IS NULL;
ALTER TABLE "ProfileMedias"
    ALTER COLUMN "ProfileId" SET NOT NULL;
ALTER TABLE "ProfileMedias"
    ALTER COLUMN "ProfileId" SET DEFAULT '';

ALTER TABLE "ProfileMedias"
    ADD "DeletedAt" timestamp with time zone;

ALTER TABLE "ProfileMedias"
    ADD "IsDeleted" boolean NOT NULL DEFAULT FALSE;

ALTER TABLE "ProfileMedias"
    ADD CONSTRAINT "PK_ProfileMedias" PRIMARY KEY ("Id");

CREATE TABLE "ChatMedias"
(
    "Id"           character varying(450)   NOT NULL,
    "ChatId"       character varying(450)   NOT NULL,
    "CreatedAt"    timestamp with time zone NOT NULL,
    "LastModified" timestamp with time zone,
    "Name"         character varying(256)   NOT NULL,
    "Key"          text                     NOT NULL,
    "Type"         text                     NOT NULL,
    "DeletedAt"    timestamp with time zone,
    "IsDeleted"    boolean                  NOT NULL,
    CONSTRAINT "PK_ChatMedias" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_ChatMedias_Chats_ChatId" FOREIGN KEY ("ChatId") REFERENCES "Chats" ("Id") ON DELETE CASCADE
);

CREATE TABLE "CommentMedias"
(
    "Id"           character varying(450)   NOT NULL,
    "CommentId"    character varying(450)   NOT NULL,
    "CreatedAt"    timestamp with time zone NOT NULL,
    "LastModified" timestamp with time zone,
    "Name"         character varying(256)   NOT NULL,
    "Key"          text                     NOT NULL,
    "Type"         text                     NOT NULL,
    "DeletedAt"    timestamp with time zone,
    "IsDeleted"    boolean                  NOT NULL,
    CONSTRAINT "PK_CommentMedias" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_CommentMedias_Comments_CommentId" FOREIGN KEY ("CommentId") REFERENCES "Comments" ("Id") ON DELETE CASCADE
);

CREATE TABLE "JobPostingMedias"
(
    "Id"           character varying(450)   NOT NULL,
    "JobPostingId" character varying(450)   NOT NULL,
    "CreatedAt"    timestamp with time zone NOT NULL,
    "LastModified" timestamp with time zone,
    "Name"         character varying(256)   NOT NULL,
    "Key"          text                     NOT NULL,
    "Type"         text                     NOT NULL,
    "DeletedAt"    timestamp with time zone,
    "IsDeleted"    boolean                  NOT NULL,
    CONSTRAINT "PK_JobPostingMedias" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_JobPostingMedias_JobPostings_JobPostingId" FOREIGN KEY ("JobPostingId") REFERENCES "JobPostings" ("Id") ON DELETE CASCADE
);

CREATE TABLE "PostMedias"
(
    "Id"           character varying(450)   NOT NULL,
    "PostId"       character varying(450)   NOT NULL,
    "CreatedAt"    timestamp with time zone NOT NULL,
    "LastModified" timestamp with time zone,
    "Name"         character varying(256)   NOT NULL,
    "Key"          text                     NOT NULL,
    "Type"         text                     NOT NULL,
    "DeletedAt"    timestamp with time zone,
    "IsDeleted"    boolean                  NOT NULL,
    CONSTRAINT "PK_PostMedias" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_PostMedias_Posts_PostId" FOREIGN KEY ("PostId") REFERENCES "Posts" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ProfileExperienceMedias"
(
    "Id"                  character varying(450)   NOT NULL,
    "ProfileExperienceId" character varying(450)   NOT NULL,
    "CreatedAt"           timestamp with time zone NOT NULL,
    "LastModified"        timestamp with time zone,
    "Name"                character varying(256)   NOT NULL,
    "Key"                 text                     NOT NULL,
    "Type"                text                     NOT NULL,
    "DeletedAt"           timestamp with time zone,
    "IsDeleted"           boolean                  NOT NULL,
    CONSTRAINT "PK_ProfileExperienceMedias" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_ProfileExperienceMedias_ProfileExperiences_ProfileExperienc~" FOREIGN KEY ("ProfileExperienceId") REFERENCES "ProfileExperiences" ("Id") ON DELETE CASCADE
);

CREATE UNIQUE INDEX "IX_Users_Email" ON "Users" ("Email") WHERE ("IsDeleted") = false;

CREATE UNIQUE INDEX "IX_Users_ProviderId" ON "Users" ("ProviderId") WHERE ("IsDeleted") = false;

CREATE UNIQUE INDEX "IX_Users_Username" ON "Users" ("Username") WHERE ("IsDeleted") = false;

CREATE UNIQUE INDEX "IX_ProfileMedias_Key" ON "ProfileMedias" ("Key");

CREATE INDEX "IX_ChatMedias_ChatId" ON "ChatMedias" ("ChatId");

CREATE UNIQUE INDEX "IX_ChatMedias_Key" ON "ChatMedias" ("Key");

CREATE UNIQUE INDEX "IX_CommentMedias_CommentId" ON "CommentMedias" ("CommentId");

CREATE UNIQUE INDEX "IX_CommentMedias_Key" ON "CommentMedias" ("Key");

CREATE INDEX "IX_JobPostingMedias_JobPostingId" ON "JobPostingMedias" ("JobPostingId");

CREATE UNIQUE INDEX "IX_JobPostingMedias_Key" ON "JobPostingMedias" ("Key");

CREATE UNIQUE INDEX "IX_PostMedias_Key" ON "PostMedias" ("Key");

CREATE INDEX "IX_PostMedias_PostId" ON "PostMedias" ("PostId");

CREATE UNIQUE INDEX "IX_ProfileExperienceMedias_Key" ON "ProfileExperienceMedias" ("Key");

CREATE INDEX "IX_ProfileExperienceMedias_ProfileExperienceId" ON "ProfileExperienceMedias" ("ProfileExperienceId");

ALTER TABLE "ProfileMedias"
    ADD CONSTRAINT "FK_ProfileMedias_Profiles_ProfileId" FOREIGN KEY ("ProfileId") REFERENCES "Profiles" ("Id") ON DELETE CASCADE;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250425174728_FixNotDeletedIndexFilter', '9.0.2');

ALTER TABLE "ProfileExperiences"
    ADD "CreatedAt" timestamp with time zone NOT NULL DEFAULT TIMESTAMPTZ '-infinity';

ALTER TABLE "ProfileExperiences"
    ADD "DeletedAt" timestamp with time zone;

ALTER TABLE "ProfileExperiences"
    ADD "IsDeleted" boolean NOT NULL DEFAULT FALSE;

ALTER TABLE "ProfileExperiences"
    ADD "LastModified" timestamp with time zone;

ALTER TABLE "JobPostings"
    ADD "DeletedAt" timestamp with time zone;

ALTER TABLE "JobPostings"
    ADD "IsDeleted" boolean NOT NULL DEFAULT FALSE;

ALTER TABLE "JobPostings"
    ADD "LastModified" timestamp with time zone;

ALTER TABLE "Chats"
    ADD "DeletedAt" timestamp with time zone;

ALTER TABLE "Chats"
    ADD "IsDeleted" boolean NOT NULL DEFAULT FALSE;

ALTER TABLE "Chats"
    ADD "LastModified" timestamp with time zone;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250425230547_AddBaseModelToOtherEntities', '9.0.2');

ALTER TABLE "ProfileMedias"
    ALTER COLUMN "ProfileId" DROP NOT NULL;

ALTER TABLE "ProfileExperienceMedias"
    ALTER COLUMN "ProfileExperienceId" DROP NOT NULL;

ALTER TABLE "PostMedias"
    ALTER COLUMN "PostId" DROP NOT NULL;

ALTER TABLE "JobPostingMedias"
    ALTER COLUMN "JobPostingId" DROP NOT NULL;

ALTER TABLE "CommentMedias"
    ALTER COLUMN "CommentId" DROP NOT NULL;

ALTER TABLE "ChatMedias"
    ALTER COLUMN "ChatId" DROP NOT NULL;

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250426011542_RefactorEntityRelationshipsConfig', '9.0.2');

CREATE TABLE "MediaGrants"
(
    "Id"          integer GENERATED BY DEFAULT AS IDENTITY,
    "Key"         text                     NOT NULL,
    "OnHold"      boolean                  NOT NULL,
    "IsActive"    boolean                  NOT NULL,
    "GrantedAt"   timestamp with time zone NOT NULL,
    "ActivatedAt" timestamp with time zone,
    "IsDeleted"   boolean                  NOT NULL,
    "DeletedAt"   timestamp with time zone,
    CONSTRAINT "PK_MediaGrants" PRIMARY KEY ("Id")
);

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250426215458_AddMediaGrantModel', '9.0.2');

CREATE INDEX "IX_MediaGrants_OnHold_GrantedAt" ON "MediaGrants" ("OnHold", "GrantedAt");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250426215629_AddMediaGrantModelConfig', '9.0.2');

COMMIT;

