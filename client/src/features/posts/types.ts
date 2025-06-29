import { Comment } from "../comments/types";
import { MediaObject, MediaObjectResponse, PresignedUrl } from "../media/types";
import { AppUser, Author } from "../users/types";

export interface PostTag {
	id: string;
	name: string;
}

export type PostsCursorPaginatedResponse = {
  posts: Post[];
  pageInfo: {
    endCursor?: number;
    hasNextPage: boolean;
    embedding?: string;
  };
}

export type PostsInitialPageParam = {
  after?: number;
  pageSize: number;
  page: number;
  embedding?: string;
}

export type PostsQueryParamLimit = number;

export interface Post {
	id: string;
	title: string;
	content: string;
	author: Author;
	createdAt: string;
	lastModified?: string | null;
	upvotes: number;
	downvotes: number;
	isUpvoted: boolean;
	isDownvoted: boolean;
	medias: MediaObjectResponse[]; // Adjust later if media structure is known
	relevanceScore?: number;
	comments: number;
	postTags: PostTag[];
}

export interface PostResponse {
	id: string;
	title: string;
	content: string;
	author: Author;
	createdAt: Date;
	lastModified?: Date | null;
	upvotes: number;
	downvotes: number;
	isUpvoted: boolean;
	isDownvoted: boolean;
	medias: MediaObjectResponse[]; // Adjust later if media structure is known
	relevanceScore?: number;
	comments: number;
	postTags: PostTag[];
}

export interface SimilarPostsResponse {
	postId: string;
	title: string;
	content: string;
	authorName: string;
	createdAt?: Date;
	similarityScore: number;
}

export interface AddPostRequest extends Pick<Post, "content" | "title"> {
	media?: PresignedUrl[];
}
