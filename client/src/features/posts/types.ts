import { AppUser } from "../users/types";

export interface Author {
  id: string;
  userId: string;
  jobTitle?: string | null;
  profilePictureUrl?: string;
  username?: string;
  firstName: string;
  lastName?: string;
}

export interface PostTag {
  id: string;
  name: string;
}

// export interface Vote {
//   userId: string;
//   postId: string;
//   value: number; // 1 for upvote, -1 for downvote
// }

// export interface Reply {
//   id: string;
//   author: Author;
//   content: string;
//   createdAt: string;
//   upvotes: number;
//   downvotes: number;
//   isUpvoted: boolean;
//   isDownvoted: boolean;
// }

export interface Comment {
  id: string;
  author: Author;
  content: string;
  createdAt: string;
  upvotes: number;
  downvotes: number;
  isUpvoted: boolean;
  isDownvoted: boolean;
  replies?: Comment[] | null; // Only one level of replies
}

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
  medias: any[]; // Adjust later if media structure is known
  comments: Comment[];
  postTags: PostTag[];
}

export type AddPostRequest = Pick<Post, 'content' | 'title'>;
