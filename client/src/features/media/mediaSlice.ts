import { apiSlice } from "../api/apiSlice";
import { MediaObject, PresignedUrl, UploadMediaRequest } from "./types";

export const mediaApiSlice = apiSlice.injectEndpoints({
  endpoints: (builder) => ({
    uploadMedia: builder.mutation<PresignedUrl[], UploadMediaRequest>({
      queryFn: async (request, baseQueryApi, extra, baseQuery) => {
        if (request.mediaList.length == 0) {
          return { data: [] };
        }

        const metadata = request.mediaList.map((file, i) => {
          const name = file.file.name;
          const size = file.file.size.toString();
          const type = file.file.type;
          const extension = name.substring(name.lastIndexOf('.'));

          return { name, size, type, extension, contentType: type };
        })

        // Get the presigned urls using the baseQuery (maybe use Promise.all())
        const response = await baseQuery({
          url: '/media/generate-urls',
          method: 'POST',
          body: { files: metadata },
        });

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

            const res = await fetch(presignedUrls[i].url, {
              method: 'PUT',
              headers: {
                'x-amz-meta-file-name': metadata[i].name,
                'x-amz-meta-file-size': metadata[i].size,
                'x-amz-meta-file-type': metadata[i].type,
                'x-amz-meta-file-extension': metadata[i].extension,
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
        return { data: presignedUrls };
      }
    }),
  }),
});

export const {
  useUploadMediaMutation,
} = mediaApiSlice;

