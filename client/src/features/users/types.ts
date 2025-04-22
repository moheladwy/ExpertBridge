// public class UpdateUserRequest
// {
//     public string ProviderId { get; set; }
//     public string FirstName { get; set; }
//     public string LastName { get; set; }
//     public string Email { get; set; }
//     public string Username { get; set; }
//     public string? PhoneNumber { get; set; }
// }

export interface AppUser {
	email: string;
}

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
};

export interface CreateUserRequest {
	name: string;
}

export interface UpdateUserRequest {
	providerId: string;
	firstName?: string;
	lastName?: string;
	email: string;
	username?: string;
	phoneNumber?: string | null;
	profilePictureUrl?: string | null;
	isEmailVerified: boolean;
}

export type CreateUserError = string | undefined;

export interface UserFormData {
	firstName: string;
	lastName: string;
	email: string;
	username?: string;
	phoneNumber?: string | null;
}

export interface Author {
	id: string;
	userId: string;
	jobTitle?: string | null;
	profilePictureUrl?: string;
	username?: string;
	firstName: string;
	lastName?: string;
}
