import { apiSlice } from "../api/apiSlice";
import { MediaObject, UploadMediaRequest } from "./types";

export const mediaApiSlice = apiSlice.injectEndpoints({
  endpoints: (builder) => ({
    uploadMedia: builder.mutation<string[], UploadMediaRequest>({
      queryFn: (request, baseQueryApi, extra, baseQuery) => {
        if (request.mediaList.length == 0) {
          return { data: [] };
        }

        // Get the presigned urls using the baseQuery (maybe use Promise.all())

        // Upload media to the s3 bucket

        // Return the media objects/urls, or an error if any

        return { data: [] };
      }
    }),
  }),
});

export const {
  useUploadMediaMutation,
} = mediaApiSlice;

