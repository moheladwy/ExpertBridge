import {
	BaseQueryFn,
	createApi,
	FetchArgs,
	fetchBaseQuery,
	retry,
} from "@reduxjs/toolkit/query/react";
import config from "@/lib/util/config";
import { auth } from "@/lib/firebase";

const baseQueryWithRetry = retry(
	async (args: string | FetchArgs, api, extraOptions) => {
		const result = await fetchBaseQuery({
			baseUrl: config.VITE_SERVER_URL,

			prepareHeaders: async (headers) => {
				const token = await auth.currentUser?.getIdToken();
				if (token) {
					headers.set("Authorization", `Bearer ${token}`);
				}

				return headers;
			},
		})(args, api, extraOptions);

		// bail out of re-tries immediately if unauthorized,
		// because we know successive re-retries would be redundant
		if (
			result.error?.status === 404 ||
			result.error?.status === 400 ||
			result.error?.status === 401 ||
			result.error?.status === 429 || // too many requests
			result.error?.status === 500
		) {
			retry.fail(result.error, result.meta);
		}

		return result;
	},
	{
		maxRetries: 4,
	}
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
		"SimilarPosts",
		"SimilarJobs",
		"JobPosting",
		"JobOffer",
		"Job",
	],

	keepUnusedDataFor: 600,

	endpoints: (builder) => ({}),
});
