/**
 * Post types for the ExpertBridge application.
 */

import { PresignedUrl } from "@/features/media/types";

// ============================================================================
// Base Types
// ============================================================================

export interface Tag {
	id: string;
	arabicName: string;
	englishName: string;
	description?: string;
}

export interface Author {
	id: string;
	userId: string;
	firstName?: string;
	lastName?: string;
	username?: string;
	profilePictureUrl?: string;
	jobTitle?: string;
}

export interface MediaObject {
	id: string;
	url: string;
	type: string;
	name?: string;
}

// ============================================================================
// Post Entity
// ============================================================================

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
	medias: MediaObject[];
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
	createdAt: string;
	lastModified?: string | null;
	upvotes: number;
	downvotes: number;
	isUpvoted: boolean;
	isDownvoted: boolean;
	medias: MediaObject[];
	relevanceScore?: number;
	comments: number;
	tags: Tag[];
}

// ============================================================================
// Pagination Types
// ============================================================================

/**
 * Response type for cursor-based pagination of posts (feed).
 */
export interface PostsCursorPaginatedResponse {
	posts: Post[];
	pageInfo: {
		endCursor?: number;
		hasNextPage: boolean;
		embedding?: string;
	};
}

/**
 * Initial page parameters for cursor-based pagination.
 */
export interface PostsInitialPageParam {
	after?: number;
	pageSize: number;
	page: number;
	embedding?: string;
}

// ============================================================================
// Request Types
// ============================================================================

/**
 * Request body for creating a new post.
 */
export interface CreatePostRequest {
	title: string;
	content: string;
	media?: PresignedUrl[];
}

/**
 * Request body for updating an existing post.
 */
export interface UpdatePostRequest {
	postId: string;
	title?: string;
	content?: string;
}

// ============================================================================
// Similar/Suggested Posts
// ============================================================================

/**
 * Response type for similar posts recommendations.
 */
export interface SimilarPostsResponse {
	postId: string;
	title: string;
	content: string;
	authorName: string;
	createdAt: string;
	relevanceScore: number;
}
