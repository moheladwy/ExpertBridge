export interface AppUser {
	email: string;
}

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
	isOnboarded?: boolean;
	token?: string;
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
	rating?: number;
}
