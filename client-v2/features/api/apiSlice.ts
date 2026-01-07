import { createApi, fetchBaseQuery } from "@reduxjs/toolkit/query/react";
import { auth } from "@/lib/firebase";
import { config } from "@/lib/util/config";

/**
 * Base API slice for RTK Query.
 *
 * Token handling is simplified - uses Firebase's built-in token caching
 * via getIdToken(). This replaces the 456-line TokenManager with a simple
 * inline call that:
 * - Returns cached token if valid (Firebase caches for ~1 hour)
 * - Auto-refreshes if expired
 * - Returns null if not authenticated
 *
 * All feature API slices should use injectEndpoints() to extend this.
 */
export const apiSlice = createApi({
	reducerPath: "api",
	baseQuery: fetchBaseQuery({
		baseUrl: config.apiUrl,
		prepareHeaders: async (headers) => {
			// Use Firebase's built-in token caching
			// getIdToken() returns cached token if valid, refreshes if expired
			const user = auth.currentUser;

			if (user) {
				try {
					const token = await user.getIdToken();
					headers.set("Authorization", `Bearer ${token}`);
				} catch (error) {
					// Token fetch failed - likely signed out
					if (config.isDebugEnabled) {
						console.error("Failed to get auth token:", error);
					}
				}
			}

			headers.set("Content-Type", "application/json");
			return headers;
		},
	}),
	tagTypes: [
		"Profile",
		"Post",
		"Posts",
		"Comment",
		"Job",
		"Notification",
		"SimilarPosts",
	],
	endpoints: () => ({}),
});
