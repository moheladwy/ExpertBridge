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
  replies?: Comment[] | null; // Only one level of replies
}

export interface AddCommentRequest {
  content: string;
  postId: string;
  parentCommentId?: string | null;
  mediaUrls?: string[] | null;
}
export interface AddReplyRequest {
  content: string;
  postId: string;
  parentCommentId: string;
}
