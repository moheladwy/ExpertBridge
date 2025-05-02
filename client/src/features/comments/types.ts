import { MediaObjectResponse, PresignedUrl } from "../media/types";
import { Author } from "../users/types";

export interface Comment {
  id: string;
  author: Author;
  authorId: string;
  postId: string;
  parentCommentId?: string | null;
  content: string;
  createdAt: string;
  upvotes: number;
  downvotes: number;
  isUpvoted: boolean;
  isDownvoted: boolean;
  medias: MediaObjectResponse[];
  replies?: Comment[] | null; // Only one level of replies
}

export interface CommentResponse {
  id: string;
  author: Author;
  authorId: string;
  postId: string;
  parentCommentId?: string | null;
  content: string;
  createdAt: Date;
  upvotes: number;
  downvotes: number;
  isUpvoted: boolean;
  isDownvoted: boolean;
  replies?: CommentResponse[] | null; // Only one level of replies
}

export interface AddCommentRequest {
  content: string;
  postId: string;
  parentCommentId?: string | null;
  media?: PresignedUrl[];
}

export interface AddReplyRequest {
  content: string;
  postId: string;
  parentCommentId: string;
}

export interface DeleteCommentRequest {
  commentId: string;
  postId: string;
  authorId: string;
  parentCommentId?: string | null;
}

export interface UpdateCommentRequest {
  commentId: string;
  content: string;
  authorId: string;
  parentCommentId?: string | null;
  postId: string;
}
