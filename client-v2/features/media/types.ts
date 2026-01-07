export type MediaType =
	| "image/jpeg"
	| "image/png"
	| "image/gif"
	| "video/mp4"
	| "application/pdf";

export interface PresignedUrl {
	url: string;
	key: string;
	type: MediaType;
}

export interface MediaObject {
	file: File;
	url?: string;
	type: string;
}

export interface MediaObjectResponse {
	id: string;
	name?: string;
	url: string;
	type: string;
}

export interface UploadMediaRequest {
	mediaList: MediaObject[];
}
