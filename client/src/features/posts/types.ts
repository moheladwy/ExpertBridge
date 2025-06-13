import { Comment } from "../comments/types";
import { MediaObject, MediaObjectResponse, PresignedUrl } from "../media/types";
import { AppUser, Author } from "../users/types";

export interface PostTag {
  id: string;
  name: string;
}

export type PostsCursorPaginatedResponse = {
  posts: PostResponse[];
  pageInfo: {
    endCursor?: number;
    hasNextPage: boolean;
  };
}

export type PostsInitialPageParam = {
  after?: number;
  pageSize: number;
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
  comments: number;
  postTags: PostTag[];
}


export interface PostResponse {
  id: string;
  title: string;
  content: string;
  author: Author;
  createdAt: Date;
  lastModified?: string | null;
  upvotes: number;
  downvotes: number;
  isUpvoted: boolean;
  isDownvoted: boolean;
  medias: MediaObjectResponse[]; // Adjust later if media structure is known
  comments: number;
  postTags: PostTag[];
}

export interface AddPostRequest extends Pick<Post, 'content' | 'title'> {
  media?: PresignedUrl[];
};
