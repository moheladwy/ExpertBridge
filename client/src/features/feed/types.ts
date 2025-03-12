import { AppUser } from "../users/types";


export interface Post {
  id: number
  author: AppUser;
  title: string;
  content: string;
  upvotes: number,
  downvotes: number,
  tags: string[],
}
