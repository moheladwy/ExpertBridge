import { MediaObjectResponse, PresignedUrl } from "../media/types";
import { Tag } from "../tags/types";
import { Author } from "../users/types";

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
};

export type PostsInitialPageParam = {
	after?: number;
	pageSize: number;
	page: number;
	embedding?: string;
};

export type PostsQueryParamLimit = number;

export interface Post {
	id: string;
	title: string;
	content: string;
	author: Author;
	language?: string;
	createdAt: string;
	lastModified?: string | null;
	upvotes: number;
	downvotes: number;
	isUpvoted: boolean;
	isDownvoted: boolean;
	medias: MediaObjectResponse[]; // Adjust later if media structure is known
	relevanceScore?: number;
	comments: number;
	tags: Tag[];
}

export interface PostResponse {
	id: string;
	title: string;
	content: string;
	author: Author;
	language?: string;
	createdAt: Date;
	lastModified?: Date | null;
	upvotes: number;
	downvotes: number;
	isUpvoted: boolean;
	isDownvoted: boolean;
	medias: MediaObjectResponse[]; // Adjust later if media structure is known
	relevanceScore?: number;
	comments: number;
	tags: Tag[];
}

export interface SimilarPostsResponse {
	postId: string;
	title: string;
	content: string;
	authorName: string;
	createdAt?: Date;
	relevanceScore: number;
}

export interface AddPostRequest extends Pick<Post, "content" | "title"> {
	media?: PresignedUrl[];
}
