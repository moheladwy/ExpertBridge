

export interface Post {
  id: number
  author: string;
  title: string;
  content: string;
  upvotes: number,
  downvotes: number,
  tags: string[],
}
