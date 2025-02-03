CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;
CREATE TABLE "Badges" (
    "Id" character varying(450) NOT NULL,
    "Name" character varying(256) NOT NULL,
    "Description" character varying(500) NOT NULL,
    CONSTRAINT "PK_Badges" PRIMARY KEY ("Id")
);

CREATE TABLE "Chats" (
    "Id" character varying(450) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "EndedAt" timestamp with time zone,
    CONSTRAINT "PK_Chats" PRIMARY KEY ("Id")
);

CREATE TABLE "JobCategories" (
    "Id" character varying(450) NOT NULL,
    "Name" character varying(256) NOT NULL,
    "Description" character varying(512) NOT NULL,
    CONSTRAINT "PK_JobCategories" PRIMARY KEY ("Id")
);

CREATE TABLE "JobStatuses" (
    "Id" character varying(450) NOT NULL,
    "Status" character varying(128) NOT NULL,
    CONSTRAINT "PK_JobStatuses" PRIMARY KEY ("Id")
);

CREATE TABLE "MediaTypes" (
    "Id" character varying(450) NOT NULL,
    "Type" character varying(128) NOT NULL,
    CONSTRAINT "PK_MediaTypes" PRIMARY KEY ("Id")
);

CREATE TABLE "Skills" (
    "Id" character varying(450) NOT NULL,
    "Name" character varying(256) NOT NULL,
    "Description" character varying(256) NOT NULL,
    CONSTRAINT "PK_Skills" PRIMARY KEY ("Id")
);

CREATE TABLE "Tags" (
    "Id" character varying(450) NOT NULL,
    "Name" character varying(256) NOT NULL,
    "Description" character varying(512) NOT NULL,
    CONSTRAINT "PK_Tags" PRIMARY KEY ("Id")
);

CREATE TABLE "Users" (
    "Id" character varying(450) NOT NULL,
    "FirebaseId" character varying(450) NOT NULL,
    "FirstName" character varying(256) NOT NULL,
    "LastName" character varying(256) NOT NULL,
    "Email" character varying(256) NOT NULL,
    "Username" character varying(256) NOT NULL,
    "isBanned" boolean NOT NULL,
    "isDeleted" boolean NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    CONSTRAINT "PK_Users" PRIMARY KEY ("Id")
);

CREATE TABLE "Media" (
    "Id" character varying(450) NOT NULL,
    "Name" character varying(256) NOT NULL,
    "MediaUrl" character varying(2048) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "LastModified" timestamp with time zone,
    "MediaTypeId" character varying(450) NOT NULL,
    CONSTRAINT "PK_Media" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Media_MediaTypes_MediaTypeId" FOREIGN KEY ("MediaTypeId") REFERENCES "MediaTypes" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Profiles" (
    "Id" character varying(450) NOT NULL,
    "UserId" character varying(450) NOT NULL,
    "JobTitle" character varying(256) NOT NULL,
    "Bio" character varying(5000) NOT NULL,
    "Rating" integer NOT NULL,
    CONSTRAINT "PK_Profiles" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Profiles_Users_UserId" FOREIGN KEY ("UserId") REFERENCES "Users" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ChatMedias" (
    "Id" character varying(450) NOT NULL,
    "ChatId" character varying(450) NOT NULL,
    "MediaId" character varying(450) NOT NULL,
    CONSTRAINT "PK_ChatMedias" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_ChatMedias_Chats_ChatId" FOREIGN KEY ("ChatId") REFERENCES "Chats" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_ChatMedias_Media_MediaId" FOREIGN KEY ("MediaId") REFERENCES "Media" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Areas" (
    "Id" character varying(450) NOT NULL,
    "ProfileId" character varying(450) NOT NULL,
    "Governorate" character varying(256) NOT NULL,
    "Region" character varying(256) NOT NULL,
    CONSTRAINT "PK_Areas" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Areas_Profiles_ProfileId" FOREIGN KEY ("ProfileId") REFERENCES "Profiles" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ChatParticipants" (
    "ChatId" character varying(450) NOT NULL,
    "ProfileId" character varying(450) NOT NULL,
    CONSTRAINT "PK_ChatParticipants" PRIMARY KEY ("ChatId", "ProfileId"),
    CONSTRAINT "FK_ChatParticipants_Chats_ChatId" FOREIGN KEY ("ChatId") REFERENCES "Chats" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_ChatParticipants_Profiles_ProfileId" FOREIGN KEY ("ProfileId") REFERENCES "Profiles" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Posts" (
    "Id" character varying(450) NOT NULL,
    "Title" character varying(256) NOT NULL,
    "Content" character varying(5000) NOT NULL,
    "AuthorId" character varying(450) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "LastModified" timestamp with time zone,
    "isDeleted" boolean NOT NULL,
    CONSTRAINT "PK_Posts" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Posts_Profiles_AuthorId" FOREIGN KEY ("AuthorId") REFERENCES "Profiles" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ProfileBadges" (
    "ProfileId" character varying(450) NOT NULL,
    "BadgeId" character varying(450) NOT NULL,
    CONSTRAINT "PK_ProfileBadges" PRIMARY KEY ("ProfileId", "BadgeId"),
    CONSTRAINT "FK_ProfileBadges_Badges_BadgeId" FOREIGN KEY ("BadgeId") REFERENCES "Badges" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_ProfileBadges_Profiles_ProfileId" FOREIGN KEY ("ProfileId") REFERENCES "Profiles" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ProfileExperiences" (
    "Id" character varying(450) NOT NULL,
    "ProfileId" character varying(450) NOT NULL,
    "Title" character varying(256) NOT NULL,
    "Description" character varying(500) NOT NULL,
    "Company" character varying(500) NOT NULL,
    "Location" character varying(500) NOT NULL,
    "StartDate" timestamp with time zone NOT NULL,
    "EndDate" timestamp with time zone,
    CONSTRAINT "PK_ProfileExperiences" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_ProfileExperiences_Profiles_ProfileId" FOREIGN KEY ("ProfileId") REFERENCES "Profiles" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ProfileMedias" (
    "Id" character varying(450) NOT NULL,
    "ProfileId" character varying(450) NOT NULL,
    "MediaId" character varying(450) NOT NULL,
    CONSTRAINT "PK_ProfileMedias" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_ProfileMedias_Media_MediaId" FOREIGN KEY ("MediaId") REFERENCES "Media" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_ProfileMedias_Profiles_ProfileId" FOREIGN KEY ("ProfileId") REFERENCES "Profiles" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ProfileSkills" (
    "ProfileId" character varying(450) NOT NULL,
    "SkillId" character varying(450) NOT NULL,
    CONSTRAINT "PK_ProfileSkills" PRIMARY KEY ("ProfileId", "SkillId"),
    CONSTRAINT "FK_ProfileSkills_Profiles_ProfileId" FOREIGN KEY ("ProfileId") REFERENCES "Profiles" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_ProfileSkills_Skills_SkillId" FOREIGN KEY ("SkillId") REFERENCES "Skills" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ProfileTags" (
    "ProfileId" character varying(450) NOT NULL,
    "TagId" character varying(450) NOT NULL,
    CONSTRAINT "PK_ProfileTags" PRIMARY KEY ("ProfileId", "TagId"),
    CONSTRAINT "FK_ProfileTags_Profiles_ProfileId" FOREIGN KEY ("ProfileId") REFERENCES "Profiles" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_ProfileTags_Tags_TagId" FOREIGN KEY ("TagId") REFERENCES "Tags" ("Id") ON DELETE CASCADE
);

CREATE TABLE "JobPostings" (
    "Id" character varying(450) NOT NULL,
    "Title" character varying(256) NOT NULL,
    "Description" character varying(5000) NOT NULL,
    "Cost" double precision NOT NULL,
    "AuthorId" character varying(450) NOT NULL,
    "AreaId" character varying(450) NOT NULL,
    "CategoryId" character varying(450) NOT NULL,
    CONSTRAINT "PK_JobPostings" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_JobPostings_Areas_AreaId" FOREIGN KEY ("AreaId") REFERENCES "Areas" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_JobPostings_JobCategories_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES "JobCategories" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_JobPostings_Profiles_AuthorId" FOREIGN KEY ("AuthorId") REFERENCES "Profiles" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Comments" (
    "Id" character varying(450) NOT NULL,
    "AuthorId" character varying(450) NOT NULL,
    "ParentId" character varying(450),
    "Content" character varying(5000) NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "LastModified" timestamp with time zone,
    "isDeleted" boolean NOT NULL,
    "PostId" character varying(450),
    CONSTRAINT "PK_Comments" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Comments_Comments_ParentId" FOREIGN KEY ("ParentId") REFERENCES "Comments" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Comments_Posts_PostId" FOREIGN KEY ("PostId") REFERENCES "Posts" ("Id"),
    CONSTRAINT "FK_Comments_Profiles_AuthorId" FOREIGN KEY ("AuthorId") REFERENCES "Profiles" ("Id") ON DELETE CASCADE
);

CREATE TABLE "PostMedias" (
    "Id" character varying(450) NOT NULL,
    "PostId" character varying(450) NOT NULL,
    "MediaId" character varying(450) NOT NULL,
    CONSTRAINT "PK_PostMedias" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_PostMedias_Media_MediaId" FOREIGN KEY ("MediaId") REFERENCES "Media" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_PostMedias_Posts_PostId" FOREIGN KEY ("PostId") REFERENCES "Posts" ("Id") ON DELETE CASCADE
);

CREATE TABLE "PostTags" (
    "PostId" character varying(450) NOT NULL,
    "TagId" character varying(450) NOT NULL,
    CONSTRAINT "PK_PostTags" PRIMARY KEY ("PostId", "TagId"),
    CONSTRAINT "FK_PostTags_Posts_PostId" FOREIGN KEY ("PostId") REFERENCES "Posts" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_PostTags_Tags_TagId" FOREIGN KEY ("TagId") REFERENCES "Tags" ("Id") ON DELETE CASCADE
);

CREATE TABLE "PostVotes" (
    "Id" character varying(450) NOT NULL,
    "IsUpvote" boolean NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "ProfileId" character varying(450) NOT NULL,
    "PostId" character varying(450) NOT NULL,
    CONSTRAINT "PK_PostVotes" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_PostVotes_Posts_PostId" FOREIGN KEY ("PostId") REFERENCES "Posts" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_PostVotes_Profiles_ProfileId" FOREIGN KEY ("ProfileId") REFERENCES "Profiles" ("Id") ON DELETE CASCADE
);

CREATE TABLE "ProfileExperienceMedias" (
    "Id" character varying(450) NOT NULL,
    "ProfileExperienceId" character varying(450) NOT NULL,
    "MediaId" character varying(450) NOT NULL,
    CONSTRAINT "PK_ProfileExperienceMedias" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_ProfileExperienceMedias_Media_MediaId" FOREIGN KEY ("MediaId") REFERENCES "Media" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_ProfileExperienceMedias_ProfileExperiences_ProfileExperienc~" FOREIGN KEY ("ProfileExperienceId") REFERENCES "ProfileExperiences" ("Id") ON DELETE CASCADE
);

CREATE TABLE "JobPostingMedias" (
    "Id" character varying(450) NOT NULL,
    "JobPostingId" character varying(450) NOT NULL,
    "MediaId" character varying(450) NOT NULL,
    CONSTRAINT "PK_JobPostingMedias" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_JobPostingMedias_JobPostings_JobPostingId" FOREIGN KEY ("JobPostingId") REFERENCES "JobPostings" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_JobPostingMedias_Media_MediaId" FOREIGN KEY ("MediaId") REFERENCES "Media" ("Id") ON DELETE CASCADE
);

CREATE TABLE "Jobs" (
    "Id" character varying(450) NOT NULL,
    "ActualCost" double precision NOT NULL,
    "StartedAt" timestamp with time zone NOT NULL,
    "EndedAt" timestamp with time zone,
    "JobStatusId" character varying(450) NOT NULL,
    "WorkerId" character varying(450) NOT NULL,
    "AuthorId" character varying(450) NOT NULL,
    "JobPostingId" character varying(450),
    CONSTRAINT "PK_Jobs" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Jobs_JobPostings_JobPostingId" FOREIGN KEY ("JobPostingId") REFERENCES "JobPostings" ("Id"),
    CONSTRAINT "FK_Jobs_JobStatuses_JobStatusId" FOREIGN KEY ("JobStatusId") REFERENCES "JobStatuses" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Jobs_Profiles_AuthorId" FOREIGN KEY ("AuthorId") REFERENCES "Profiles" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Jobs_Profiles_WorkerId" FOREIGN KEY ("WorkerId") REFERENCES "Profiles" ("Id") ON DELETE CASCADE
);

CREATE TABLE "CommentMedias" (
    "Id" character varying(450) NOT NULL,
    "CommentId" character varying(450) NOT NULL,
    "MediaId" character varying(450) NOT NULL,
    CONSTRAINT "PK_CommentMedias" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_CommentMedias_Comments_CommentId" FOREIGN KEY ("CommentId") REFERENCES "Comments" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_CommentMedias_Media_MediaId" FOREIGN KEY ("MediaId") REFERENCES "Media" ("Id") ON DELETE CASCADE
);

CREATE TABLE "CommentVotes" (
    "Id" character varying(450) NOT NULL,
    "IsUpvote" boolean NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "CommentId" character varying(450) NOT NULL,
    "ProfileId" character varying(450) NOT NULL,
    CONSTRAINT "PK_CommentVotes" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_CommentVotes_Comments_CommentId" FOREIGN KEY ("CommentId") REFERENCES "Comments" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_CommentVotes_Profiles_ProfileId" FOREIGN KEY ("ProfileId") REFERENCES "Profiles" ("Id") ON DELETE CASCADE
);

CREATE TABLE "JobReviews" (
    "Id" character varying(450) NOT NULL,
    "Content" character varying(5000) NOT NULL,
    "Rating" integer NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "LastModified" timestamp with time zone,
    "IsDeleted" boolean NOT NULL,
    "WorkerId" character varying(450) NOT NULL,
    "CustomerId" character varying(450) NOT NULL,
    "JobId" character varying(450) NOT NULL,
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

CREATE UNIQUE INDEX "IX_Users_Username" ON "Users" ("Username");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20250203151402_InitialMigration', '9.0.1');

COMMIT;

