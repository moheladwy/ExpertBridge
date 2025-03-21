import { AppUser } from "../users/types";


export interface Author {
  id: string;
  userId: string;
  jobTitle?: string;
  bio?: string;
  profilePictureUrl?: string;
  rating: number;
  ratingCount: number;
}

export interface PostTag {
  id: string;
  name: string;
}

export interface Post {
  id: string;
  title: string;
  content: string;
  authorId: string;
  createdAt: string;
  lastModified?: string | null;
  isDeleted: boolean;
  author: Author;
  medias: any[]; // Adjust type if needed
  comments: any[]; // Adjust type if needed
  votes: any[]; // Adjust type if needed
  postTags: PostTag[];
}


export type AddPostRequest = Pick<Post, 'content' | 'title'>;
