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
