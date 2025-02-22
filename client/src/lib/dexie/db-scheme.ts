import { Dexie } from 'dexie';
import type {
  User, Profile, Area, Badge, Chat, Comment, Job, JobCategory,
  JobPosting, JobReview, JobStatus, Media, MediaType, Post,
  ProfileExperience, Skill, Tag, PostVote, CommentVote,
  ChatMedia, CommentMedia, JobPostingMedia, PostMedia,
  ProfileMedia, ProfileExperienceMedia, PostTag, ProfileBadge,
  ProfileSkill, ProfileTag, ChatParticipant,
  Governorates
} from '@/lib/api/interfaces';

const db_name = import.meta.env.VITE_INDEXED_DB_NAME as string;
const db_version = Number(import.meta.env.VITE_INDEXED_DB_VERSION);

class ExpertBridgeDatabase extends Dexie {
  // Main entities
  users!: Dexie.Table<User, string>;
  profiles!: Dexie.Table<Profile, string>;
  areas!: Dexie.Table<Area, string>;
  badges!: Dexie.Table<Badge, string>;
  chats!: Dexie.Table<Chat, string>;
  chatParticipants!: Dexie.Table<ChatParticipant, string>;
  comments!: Dexie.Table<Comment, string>;
  jobs!: Dexie.Table<Job, string>;
  jobCategories!: Dexie.Table<JobCategory, string>;
  jobPostings!: Dexie.Table<JobPosting, string>;
  jobReviews!: Dexie.Table<JobReview, string>;
  jobStatuses!: Dexie.Table<JobStatus, string>;
  media!: Dexie.Table<Media, string>;
  mediaTypes!: Dexie.Table<MediaType, string>;
  posts!: Dexie.Table<Post, string>;
  profileExperiences!: Dexie.Table<ProfileExperience, string>;
  skills!: Dexie.Table<Skill, string>;
  tags!: Dexie.Table<Tag, string>;
  governments!: Dexie.Table<Governorates, string>;

  // Votes
  postVotes!: Dexie.Table<PostVote, string>;
  commentVotes!: Dexie.Table<CommentVote, string>;

  // Media relationships
  chatMedia!: Dexie.Table<ChatMedia, string>;
  commentMedia!: Dexie.Table<CommentMedia, string>;
  jobPostingMedia!: Dexie.Table<JobPostingMedia, string>;
  postMedia!: Dexie.Table<PostMedia, string>;
  profileMedia!: Dexie.Table<ProfileMedia, string>;
  profileExperienceMedia!: Dexie.Table<ProfileExperienceMedia, string>;

  // Many-to-Many relationships
  postTags!: Dexie.Table<PostTag, [string, string]>;
  profileBadges!: Dexie.Table<ProfileBadge, [string, string]>;
  profileSkills!: Dexie.Table<ProfileSkill, [string, string]>;
  profileTags!: Dexie.Table<ProfileTag, [string, string]>;

  constructor() {
    super(db_name);

    this.version(db_version).stores({
      // Main entities
      users: `
        id, firebaseId, firstName, lastName, email, username, phoneNumber,
        isBanned, isDeleted, isEmailVerified, createdAt
      `,
      profiles: `
        id, userId, jobTitle, bio, profilePictureUrl, rating, ratingCount
      `,
      areas: 'id, profileId, governorateId, region',
      badges: 'id, name, description',
      chats: 'id, createdAt, endedAt',
      chatParticipants: '[chatId+profileId], chatId, profileId',
      comments: `
        id, authorId, parentId, content, createdAt, lastModified, isDeleted
      `,
      jobs: `
        id, actualCost, startedAt, endedAt, jobStatusId, workerId, 
        authorId, jobPostingId
      `,
      jobCategories: 'id, name, description',
      jobPostings: `
        id, title, description, cost, createdAt, updatedAt, 
        authorId, areaId, categoryId
      `,
      jobReviews: `
        id, content, rating, createdAt, lastModified, isDeleted,
        workerId, customerId, jobId
      `,
      jobStatuses: 'id, status',
      media: `
        id, name, mediaUrl, createdAt, lastModified, mediaTypeId
      `,
      mediaTypes: 'id, type',
      posts: `
        id, title, content, authorId, createdAt, lastModified, isDeleted
      `,
      profileExperiences: `
        id, profileId, title, description, company, location, 
        startDate, endDate
      `,
      skills: 'id, name, description',
      tags: 'id, name, description',
      governments: 'id, name',

      // Votes
      postVotes: `
        id, isUpvote, createdAt, profileId, postId,
        [profileId+postId]
      `,
      commentVotes: `
        id, isUpvote, createdAt, commentId, profileId,
        [profileId+commentId]
      `,

      // Media relationships
      chatMedia: '[chatId+mediaId], id, chatId, mediaId',
      commentMedia: '[commentId+mediaId], id, commentId, mediaId',
      jobPostingMedia: '[jobPostingId+mediaId], id, jobPostingId, mediaId',
      postMedia: '[postId+mediaId], id, postId, mediaId',
      profileMedia: '[profileId+mediaId], id, profileId, mediaId',
      profileExperienceMedia: '[profileExperienceId+mediaId], id, profileExperienceId, mediaId',

      // Many-to-Many relationships
      postTags: '[postId+tagId], postId, tagId',
      profileBadges: '[profileId+badgeId], profileId, badgeId',
      profileSkills: '[profileId+skillId], profileId, skillId',
      profileTags: '[profileId+tagId], profileId, tagId'
    });
  }
}

const db = new ExpertBridgeDatabase();

export default db;