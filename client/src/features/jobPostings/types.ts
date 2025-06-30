import { Comment } from "../comments/types";
import { MediaObject, MediaObjectResponse, PresignedUrl } from "../media/types";
import { AppUser, Author } from "../users/types";

export interface JobPostingTag {
  id: string;
  name: string;
}

export type JobPostingPaginatedResponse = {
  jobPostings: JobPosting[];
  pageInfo: {
    endCursor?: number;
    hasNextPage: boolean;
    embedding?: string;
  };
}

export type JobPostingsInitialPageParam = {
  after?: number;
  pageSize: number;
  page: number;
  embedding?: string;
}

export type JobPostingsQueryParamLimit = number;

export interface JobPosting {
  id: string;
  title: string;
  content: string;
  author: Author;
  budget: number;
  area: string;
  createdAt: string;
  lastModified?: string | null;
  upvotes: number;
  downvotes: number;
  isUpvoted: boolean;
  isDownvoted: boolean;
  medias: MediaObjectResponse[]; // Adjust later if media structure is known
  relevanceScore?: number;
  comments: number;
  postTags: JobPostingTag[];
}

export interface JobPostingResponse {
  id: string;
  title: string;
  content: string;
  author: Author;
  budget: number;
  area: string;
  createdAt: Date;
  lastModified?: Date | null;
  upvotes: number;
  downvotes: number;
  isUpvoted: boolean;
  isDownvoted: boolean;
  medias: MediaObjectResponse[]; // Adjust later if media structure is known
  relevanceScore?: number;
  comments: number;
  postTags: JobPostingTag[];
}

export interface SimilarJobsResponse {
  jobPostingId: string;
  title: string;
  content: string;
  authorName: string;
  createdAt?: Date;
  relevanceScore: number;
}

export interface CreateJobPostingRequest extends Pick<JobPosting, "content" | "title" | "budget" | "area"> {
  media?: PresignedUrl[];
}
