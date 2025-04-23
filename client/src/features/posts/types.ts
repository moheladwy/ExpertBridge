import { Comment } from "../comments/types";
import { MediaObject, MediaObjectResponse, PresignedUrl } from "../media/types";
import { AppUser, Author } from "../users/types";

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
