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
	profilePictureUrl?: string;
	isOnboarded: boolean;
	commentsUpvotes: number;
	commentsDownvotes: number;
};
