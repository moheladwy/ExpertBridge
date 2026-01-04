/**
 * Comment types for the ExpertBridge application.
 */

import { Author, MediaObject } from "../posts/types";

export interface Comment {
	id: string;
	author: Author;
	authorId: string;
	postId?: string;
	jobPostingId?: string;
	parentCommentId?: string | null;
	content: string;
	createdAt: string;
	lastModified?: string;
	upvotes: number;
	downvotes: number;
	isUpvoted: boolean;
	isDownvoted: boolean;
	medias: MediaObject[];
	replies?: Comment[] | null;
}

export interface CommentResponse {
	id: string;
	author: Author;
	authorId: string;
	postId?: string;
	jobPostingId?: string;
	parentCommentId?: string | null;
	content: string;
	createdAt: Date;
	lastModified?: Date;
	upvotes: number;
	downvotes: number;
	isUpvoted: boolean;
	isDownvoted: boolean;
	medias: MediaObject[];
	replies?: CommentResponse[] | null;
}
