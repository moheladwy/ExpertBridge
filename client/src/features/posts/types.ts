import { AppUser } from "../users/types";


export interface Post {
  id: number;
  userId: number;
  title: string;
  content: string;
  upvotes: number;
  downvotes: number;
  tags: string[];
  createdAt: string;
}

export type AddPostRequest = Pick<Post, 'content' | 'userId' | 'title' | 'tags'>;
