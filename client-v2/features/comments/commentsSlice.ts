import { apiSlice } from "../api/apiSlice";
import type { Comment, CommentResponse } from "./types";

/**
 * Transform CommentResponse to Comment.
 * Since both types now use string dates, this is primarily for type safety.
 */
const commentResponseTransformer = (c: CommentResponse): Comment => ({
	...c,
	createdAt: c.createdAt,
	lastModified: c.lastModified,
	replies: c.replies?.map(commentResponseTransformer) ?? null,
});

/**
 * Transform array of CommentResponse to Comment[].
 */
const commentsResponseTransformer = (
	response: CommentResponse[]
): Comment[] => {
	return response.map(commentResponseTransformer);
};

export const commentsApiSlice = apiSlice.injectEndpoints({
	endpoints: (builder) => ({
		/**
		 * Get all comments by a specific profile/user ID.
		 * Used on the profile page to display user's comments.
		 */
		getCommentsByUserId: builder.query<Comment[], string>({
			query: (profileId) => `/profiles/${profileId}/comments`,
			transformResponse: commentsResponseTransformer,
			providesTags: (result = [], _error, profileId) => [
				{ type: "Comment", id: `USER-${profileId}` },
				...result.map(({ id }) => ({ type: "Comment" as const, id })),
			],
		}),

		/**
		 * Get all comments for a specific post.
		 */
		getCommentsByPostId: builder.query<Comment[], string>({
			query: (postId) => `/posts/${postId}/comments`,
			transformResponse: commentsResponseTransformer,
			providesTags: (result = [], _error, postId) => [
				{ type: "Comment", id: `POST-${postId}` },
				...result.map(({ id }) => ({ type: "Comment" as const, id })),
			],
		}),
	}),
});

export const { useGetCommentsByUserIdQuery, useGetCommentsByPostIdQuery } =
	commentsApiSlice;
