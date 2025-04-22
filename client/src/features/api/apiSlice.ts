import {
	BaseQueryFn,
	createApi,
	fetchBaseQuery,
} from "@reduxjs/toolkit/query/react";
import config from "@/lib/util/config";
import { auth } from "@/lib/firebase";

export const apiSlice = createApi({
	reducerPath: "api",
	baseQuery: fetchBaseQuery({
		baseUrl: config.VITE_SERVER_URL,

		prepareHeaders: async (headers) => {
			const token = await auth.currentUser?.getIdToken();
			if (token) {
				headers.set("Authorization", `Bearer ${token}`);
			}

			return headers;
		},
	}),

	tagTypes: ["CurrentUser", "AuthUser", "Post", "Comment", "Profile"],
	keepUnusedDataFor: 600,

	endpoints: (builder) => ({}),
});
