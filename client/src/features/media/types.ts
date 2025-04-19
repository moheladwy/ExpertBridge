

export interface MediaObject {
  file: File;
  url?: string | null;
  type: 'video' | 'image' | 'pdf';
};

export interface UploadMediaRequest {
  mediaList: MediaObject[];
};
