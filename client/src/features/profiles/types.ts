export type ProfileResponse = {
  id: string;
  userId: string;
  rating: number;
  ratingCount: number;
  firstName?: string;
  lastName?: string;
  email: string;
  username?: string;
  phoneNumber?: string;
  isBanned: boolean;
  createdAt: string; // Date is usually serialized as a string in JSON responses
  jobTitle?: string;
  bio?: string;
  profilePictureUrl?: string;
  isOnboarded: boolean;
  commentsUpvotes: number;
  commentsDownvotes: number;
  reputation: number;
  skills: string[];
};

export interface OnboardUserRequest {
  tags: string[];
}

export interface UpdateProfileRequest {
  firstName?: string;
  lastName?: string;
  username?: string;
  phoneNumber?: string;
  jobTitle?: string;
  bio?: string;
  skills: string[];
}

export interface JobApplicant {
	id: string;
	userId: string;
	jobTitle?: string | null;
	profilePictureUrl?: string;
	username?: string;
	firstName: string;
	lastName?: string;
	rating?: number;
	reputation?: number;
	jobsDone?: number;
}

