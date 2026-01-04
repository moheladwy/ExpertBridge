import { apiSlice } from "../api/apiSlice";
import type { Post, PostResponse } from "./types";

/**
 * Transform PostResponse to Post.
 * Since both types now use string dates, this is primarily for type safety.
 */
const postResponseTransformer = (p: PostResponse): Post => ({
	...p,
	createdAt: p.createdAt,
	lastModified: p.lastModified ?? null,
});

/**
 * Transform array of PostResponse to Post[].
 */
const postsResponseTransformer = (response: PostResponse[]): Post[] => {
	return response.map(postResponseTransformer);
};

export const postsApiSlice = apiSlice.injectEndpoints({
	endpoints: (builder) => ({
		/**
		 * Get all posts by a specific profile/user ID.
		 * Used on the profile page to display user's posts.
		 */
		getPostsByUserId: builder.query<Post[], string>({
			query: (profileId) => `/profiles/${profileId}/posts`,
			transformResponse: postsResponseTransformer,
			providesTags: (result = [], _error, profileId) => [
				{ type: "Post", id: `USER-${profileId}` },
				...result.map(({ id }) => ({ type: "Post" as const, id })),
			],
		}),

		/**
		 * Get a single post by ID.
		 */
		getPost: builder.query<Post, string>({
			query: (postId) => `/posts/${postId}`,
			transformResponse: postResponseTransformer,
			providesTags: (_result, _error, postId) => [
				{ type: "Post", id: postId },
			],
		}),
	}),
});

export const { useGetPostsByUserIdQuery, useGetPostQuery } = postsApiSlice;
