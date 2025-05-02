import { apiSlice } from "../api/apiSlice";
import { MediaObject, PresignedUrl, UploadMediaRequest } from "./types";

export const mediaApiSlice = apiSlice.injectEndpoints({
  endpoints: (builder) => ({
    uploadMedia: builder.mutation<PresignedUrl[], UploadMediaRequest>({
      queryFn: async (request, baseQueryApi, extra, baseQuery) => {
        if (request.mediaList.length === 0) {
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
          const results = await Promise.all(
            request.mediaList.map((file, i) =>
              fetch(presignedUrls[i].url, {
                method: 'PUT',
                headers: {
                  'x-amz-meta-file-name': metadata[i].name,
                  'x-amz-meta-file-size': metadata[i].size,
                  'x-amz-meta-file-type': metadata[i].type,
                  'x-amz-meta-file-extension': metadata[i].extension,
                  'x-amz-meta-file-key': presignedUrls[i].key,
                  'x-amz-meta-cache-control': 'public,max-age=31536000',
                  'x-amz-meta-max-size': '157286400',
                  'Cache-Control': 'public,max-age=31536000',
                },
                body: file.file,
              })
            )
          );

          results.forEach(res => {
            if (!res.ok)
              throw new Error('Media upload failed');
          });
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
          data: presignedUrls.map((url, i) => ({
            ...url,
            type: metadata[i].type,
          }) as PresignedUrl),
        };
      }
    }),
  }),
});

export const {
  useUploadMediaMutation,
} = mediaApiSlice;

