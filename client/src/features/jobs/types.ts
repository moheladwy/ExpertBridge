import { Author } from "../users/types";

export interface CreateJobOfferRequest {
	title: string;
	description: string;
	budget: number;
	area: string;
	workerId: string;
}

export interface JobOfferResponse {
	id: string;
	title: string;
	description: string;
	budget: number;
	area: string;
	createdAt: string;
	author: Author;
}

export interface JobResponse {
	id: string;
	title: string;
	description: string;
	chatId: string;
	actualCost: number;
	area: string;
	startedAt: string | null; // ISO string or null
	endedAt: string | null;
	isPaid: boolean;
	isCompleted: boolean;
	updatedAt: string | null;

	authorId: string;
	workerId: string;

	author: Author;
	worker: Author;
}
