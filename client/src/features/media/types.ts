
export interface PresignedUrl {
  url: string;
  key: string;
  type: 'video' | 'image' | 'pdf';
}

export interface MediaObject {
  file: File;
  url?: string | null;
  type: 'video' | 'image' | 'pdf';
};

export interface MediaObjectResponse {
  id: string;
  name?: string | null;
  url: string;
  type: string;
};

export interface UploadMediaRequest {
  mediaList: MediaObject[];
};
