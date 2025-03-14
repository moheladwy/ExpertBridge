import { AppUser } from "../users/types";


export interface Post {
  id: number;
  userId: number;
  title: string;
  body: string;
  upvotes: number;
  downvotes: number;
  tags: string[];
  date: string;
}

export type AddPostRequest = Pick<Post, 'body' | 'userId' | 'title' | 'tags'>;
