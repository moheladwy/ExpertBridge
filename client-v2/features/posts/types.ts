/**
 * Post types for the ExpertBridge application.
 */

export interface Tag {
	id: string;
	name: string;
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
	createdAt: Date;
	lastModified?: Date | null;
	upvotes: number;
	downvotes: number;
	isUpvoted: boolean;
	isDownvoted: boolean;
	medias: MediaObject[];
	relevanceScore?: number;
	comments: number;
	tags: Tag[];
}
