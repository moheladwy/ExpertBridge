export interface SearchPostRequest {
  query: string;
  limit?: number;
}

export interface SearchUsersResponse {
  id: string;
  email: string;
  username?: string;
  phoneNumber?: string;
  firstName?: string;
  lastName?: string;
  profilePictureUrl?: string;
  jobTitle?: string;
  bio?: string;
  rank: number;
}

export interface SearchJobPostsRequest {
  query: string;
  limit?: number;
  area?: string;
  minBudget?: number;
  maxBudget?: number;
  isRemote?: boolean;
}
