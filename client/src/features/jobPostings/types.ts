import { MediaObjectResponse, PresignedUrl } from "../media/types";
import { JobApplicant } from "../profiles/types";
import { Tag } from "../tags/types";
import { Author } from "../users/types";

export interface JobPostingTag {
	id: string;
	name: string;
}

export type JobPostingPaginatedResponse = {
	jobPostings: JobPosting[];
	pageInfo: {
		endCursor?: number;
		hasNextPage: boolean;
		embedding?: string;
	};
};

export type JobPostingsInitialPageParam = {
	after?: number;
	pageSize: number;
	page: number;
	embedding?: string;
};

export type JobPostingsQueryParamLimit = number;

export interface JobPosting {
	id: string;
	title: string;
	content: string;
	author: Author;
	budget: number;
	language?: string;
	area: string;
	createdAt: string;
	lastModified?: string | null;
	upvotes: number;
	downvotes: number;
	isAppliedFor: boolean;
	isUpvoted: boolean;
	isDownvoted: boolean;
	medias: MediaObjectResponse[]; // Adjust later if media structure is known
	relevanceScore?: number;
	comments: number;
	tags: Tag[];
}

export interface JobPostingResponse {
	id: string;
	title: string;
	content: string;
	author: Author;
	budget: number;
	language?: string;
	area: string;
	createdAt: Date;
	lastModified?: Date | null;
	upvotes: number;
	downvotes: number;
	isAppliedFor: boolean;
	isUpvoted: boolean;
	isDownvoted: boolean;
	medias: MediaObjectResponse[]; // Adjust later if media structure is known
	relevanceScore?: number;
	comments: number;
	tags: Tag[];
}

export interface SimilarJobsResponse {
	jobPostingId: string;
	title: string;
	content: string;
	authorName: string;
	createdAt?: Date;
	budget: number;
	area: string;
	relevanceScore: number;
}

export interface CreateJobPostingRequest
	extends Pick<JobPosting, "content" | "title" | "budget" | "area"> {
	media?: PresignedUrl[];
}

export interface ApplyToJobPostingRequest {
	jobPostingId: string;
	coverLetter?: string;
	offeredCost: number;
}

export interface JobApplicationResponse {
	id: string;
	jobPostingId: string;
	applicantId: string;
	coverLetter?: string;
	offeredCost: number;
	appliedAt: string;
	applicant?: JobApplicant;
}

// public required string JobPostingId { get; set; }
// public required string ApplicantId { get; set; }
// public string? CoverLetter { get; set; }
// public required decimal OfferedCost { get; set; }
// public DateTime AppliedAt { get; set; }
// public ApplicantResponse ? Applicant { get; set; }
