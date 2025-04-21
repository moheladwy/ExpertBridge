import { apiSlice } from "../api/apiSlice";
import { MediaObject, PresignedUrl, UploadMediaRequest } from "./types";

export const mediaApiSlice = apiSlice.injectEndpoints({
  endpoints: (builder) => ({
    uploadMedia: builder.mutation<PresignedUrl[], UploadMediaRequest>({
      queryFn: async (request, baseQueryApi, extra, baseQuery) => {
        if (request.mediaList.length == 0) {
          return { data: [] };
        }

        // Get the presigned urls using the baseQuery (maybe use Promise.all())
        const response = await baseQuery(`/media/generate-urls?count=${request.mediaList.length}`);
        if (response.error) {
          return { error: response.error };
        }

        console.log('urls', response);

        const presignedUrls = response.data as PresignedUrl[];

        // Upload media to the s3 bucket
        try {
          for (let i = 0; i < request.mediaList.length; ++i) {
            presignedUrls[i].type = request.mediaList[i].type;

            const file = request.mediaList[i].file;
            const fileName = file.name;
            const fileSize = file.size.toString();
            const fileType = file.type;
            const fileExtension = fileName.substring(fileName.lastIndexOf('.'));

            const res = await fetch(presignedUrls[i].url, {
              method: 'PUT',
              headers: {
                'Content-Type': fileType,
                'x-amz-meta-file-name': fileName,
                'x-amz-meta-file-size': fileSize,
                'x-amz-meta-file-type': fileType,
                'x-amz-meta-file-extension': fileExtension,
                'x-amz-meta-file-key': presignedUrls[i].key,
              },
              body: file,
            });

            console.log(res);

            if (!res.ok) {
              const js = await res.json();
              throw new Error(`Media upload failed. ${js}`);
            }
          }
        } catch (error) {
          console.error('WTF IS HAPPENING HERE?');
          return {
            error: {
              status: 'CUSTOM_ERROR',
              error: (error as Error).message || 'Media upload failed',
            },
          };
        }

        // Return the media objects/urls, or an error if any
        return {
          data: presignedUrls.map(pu => ({
            ...pu,
            url: pu.url//.split('?')[0]
          })),
        };
      }
    }),
  }),
});

export const {
  useUploadMediaMutation,
} = mediaApiSlice;

