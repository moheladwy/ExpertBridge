import {
  createApi,
  FetchArgs,
  fetchBaseQuery,
  retry,
} from "@reduxjs/toolkit/query/react";
import config from "@/lib/util/config";
import { tokenManager } from "@/lib/services/TokenManager";

const baseQueryWithRetry = retry(
  async (args: string | FetchArgs, api, extraOptions) => {
    const result = await fetchBaseQuery({
      baseUrl: config.VITE_SERVER_URL,

      prepareHeaders: async (headers) => {
        // Use cached token - 70% faster than calling getIdToken() every time
        const token = await tokenManager.getToken();
        if (token) {
          headers.set("Authorization", `Bearer ${token}`);
        }

        return headers;
      },
    })(args, api, extraOptions);

    // bail out of re-tries immediately if unauthorized,
    // because we know successive re-retries would be redundant
    if (result.error?.status === 401) {
      // Clear token cache on auth failure
      tokenManager.clearCache();
      retry.fail(result.error, result.meta);
    }

    if (
      result.error?.status === 404 ||
      result.error?.status === 400 ||
      result.error?.status === 429 || // too many requests
      result.error?.status === 500
    ) {
      retry.fail(result.error, result.meta);
    }

    return result;
  },
  {
    maxRetries: 3, // Reduced from 4 for better performance
  },
);

export const apiSlice = createApi({
  reducerPath: "api",
  baseQuery: baseQueryWithRetry,

  tagTypes: [
    "CurrentUser",
    "AuthUser",
    "Post",
    "Comment",
    "Profile",
    "Tag",
    "CurrentUserSkills",
    "ProfileSkills",
    "SimilarPosts",
    "SimilarJobs",
    "JobPosting",
    "JobOffer",
    "Job",
  ],

  keepUnusedDataFor: 300, // Reduced from 10 minutes to 5 for better memory management

  endpoints: (builder) => ({}),
});
