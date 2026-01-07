import { createSelector } from "@reduxjs/toolkit";
import { apiSlice } from "../api/apiSlice";
import type {
	Post,
	PostResponse,
	PostsCursorPaginatedResponse,
	PostsInitialPageParam,
	CreatePostRequest,
	UpdatePostRequest,
	SimilarPostsResponse,
} from "./types";

// ============================================================================
// Transformers
// ============================================================================

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

/**
 * Flatten all pages into a single array of posts.
 */
const transformPagesToFlatPosts = (
	pages: PostsCursorPaginatedResponse[] | undefined
): Post[] => {
	if (!pages) return [];
	return pages.flatMap((page) => page.posts);
};

// ============================================================================
// API Slice
// ============================================================================

export const postsApiSlice = apiSlice.injectEndpoints({
	endpoints: (builder) => ({
		// ====================================================================
		// Query Endpoints
		// ====================================================================

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

		/**
		 * Get posts with cursor-based pagination for infinite scroll (feed).
		 * Uses POST to send pagination params in body.
		 */
		getPostsCursor: builder.infiniteQuery<
			PostsCursorPaginatedResponse,
			undefined,
			PostsInitialPageParam
		>({
			query: ({ pageParam }) => ({
				url: "/posts/feed",
				method: "POST",
				body: pageParam,
			}),
			infiniteQueryOptions: {
				initialPageParam: { pageSize: 10, page: 1 },
				getNextPageParam: (lastPage, _allPages, lastPageParam) => {
					if (!lastPage.pageInfo?.hasNextPage) {
						return undefined;
					}
					return {
						after: lastPage.pageInfo.endCursor,
						pageSize: lastPageParam.pageSize,
						page: lastPageParam.page + 1,
						embedding: lastPage.pageInfo.embedding,
					};
				},
			},
			providesTags: (result) => {
				const listTag = { type: "Posts" as const, id: "LIST" };
				if (!result?.pages) {
					return [listTag];
				}
				const postTags = result.pages.flatMap((page) =>
					page.posts.map((post) => ({
						type: "Post" as const,
						id: post.id,
					}))
				);
				return [listTag, ...postTags];
			},
		}),

		/**
		 * Get suggested posts for sidebar.
		 */
		getSuggestedPosts: builder.query<SimilarPostsResponse[], number>({
			query: (limit) => `/posts/suggested?limit=${limit}`,
		}),

		/**
		 * Get similar posts to a specific post.
		 */
		getSimilarPosts: builder.query<SimilarPostsResponse[], string>({
			query: (postId) => `/posts/${postId}/similar?limit=5`,
			providesTags: (_result, _error, postId) => [
				{ type: "SimilarPosts", id: postId },
			],
		}),

		// ====================================================================
		// Mutation Endpoints
		// ====================================================================

		/**
		 * Create a new post.
		 */
		createPost: builder.mutation<Post, CreatePostRequest>({
			query: (body) => ({
				url: "/posts",
				method: "POST",
				body,
			}),
			transformResponse: postResponseTransformer,
			invalidatesTags: [{ type: "Posts", id: "LIST" }],
			onQueryStarted: async (_request, lifecycleApi) => {
				try {
					const { data: createdPost } =
						await lifecycleApi.queryFulfilled;

					// Add the new post to the feed cache
					lifecycleApi.dispatch(
						postsApiSlice.util.updateQueryData(
							"getPostsCursor",
							undefined,
							(draft) => {
								// Guard against missing or uninitialized cache
								if (!draft || !Array.isArray(draft.pages)) {
									draft.pages = [
										{
											posts: [createdPost],
											pageInfo: { hasNextPage: false },
										},
									];
								} else if (draft.pages[0]) {
									draft.pages[0].posts.unshift(createdPost);
								} else {
									draft.pages.unshift({
										posts: [createdPost],
										pageInfo: { hasNextPage: false },
									});
								}
							}
						)
					);

					// Also cache the individual post
					lifecycleApi.dispatch(
						postsApiSlice.util.upsertQueryData(
							"getPost",
							createdPost.id,
							createdPost
						)
					);
				} catch {
					console.error("Post creation failed");
				}
			},
		}),

		/**
		 * Update an existing post.
		 */
		updatePost: builder.mutation<Post, UpdatePostRequest>({
			query: ({ postId, ...updateData }) => ({
				url: `/posts/${postId}`,
				method: "PATCH",
				body: updateData,
			}),
			transformResponse: postResponseTransformer,
			invalidatesTags: (_result, _error, { postId }) => [
				{ type: "Posts", id: "LIST" },
				{ type: "Post", id: postId },
			],
			onQueryStarted: async (request, lifecycleApi) => {
				// Optimistically update the feed cache
				const feedPatchResult = lifecycleApi.dispatch(
					postsApiSlice.util.updateQueryData(
						"getPostsCursor",
						undefined,
						(draft) => {
							const posts = transformPagesToFlatPosts(
								draft.pages
							);
							const updateCandidate = posts.find(
								(p) => p.id === request.postId
							);
							if (updateCandidate) {
								if (request.title !== undefined) {
									updateCandidate.title = request.title;
								}
								if (request.content !== undefined) {
									updateCandidate.content = request.content;
								}
							}
						}
					)
				);

				// Optimistically update the individual post cache
				const postPatchResult = lifecycleApi.dispatch(
					postsApiSlice.util.updateQueryData(
						"getPost",
						request.postId,
						(draft) => {
							if (draft) {
								if (request.title !== undefined) {
									draft.title = request.title;
								}
								if (request.content !== undefined) {
									draft.content = request.content;
								}
							}
						}
					)
				);

				try {
					await lifecycleApi.queryFulfilled;
				} catch {
					// Rollback on error
					feedPatchResult.undo();
					postPatchResult.undo();
				}
			},
		}),

		/**
		 * Delete a post.
		 */
		deletePost: builder.mutation<void, string>({
			query: (postId) => ({
				url: `/posts/${postId}`,
				method: "DELETE",
			}),
			invalidatesTags: (_result, _error, postId) => [
				{ type: "Posts", id: "LIST" },
				{ type: "Post", id: postId },
			],
			onQueryStarted: async (postId, lifecycleApi) => {
				try {
					await lifecycleApi.queryFulfilled;

					// Remove the post from the feed cache
					lifecycleApi.dispatch(
						postsApiSlice.util.updateQueryData(
							"getPostsCursor",
							undefined,
							(draft) => {
								const page = draft.pages.find((p) =>
									p.posts.some((post) => post.id === postId)
								);
								if (page) {
									page.posts = page.posts.filter(
										(post) => post.id !== postId
									);
								}
							}
						)
					);

					// Invalidate the individual post cache entry
					lifecycleApi.dispatch(
						postsApiSlice.util.invalidateTags([
							{ type: "Post", id: postId },
						])
					);
				} catch {
					console.error("Error while deleting post");
				}
			},
		}),

		/**
		 * Upvote a post with optimistic updates.
		 */
		upvotePost: builder.mutation<Post, Post>({
			query: (post) => ({
				url: `/posts/${post.id}/upvote`,
				method: "PATCH",
			}),
			transformResponse: postResponseTransformer,
			onQueryStarted: async (post, lifecycleApi) => {
				// Calculate optimistic state
				let upvotes = post.upvotes;
				let downvotes = post.downvotes;
				let isUpvoted = post.isUpvoted;
				let isDownvoted = post.isDownvoted;

				// Toggle off if already upvoted
				if (post.isUpvoted) {
					upvotes -= 1;
					isUpvoted = false;
				}
				// Switch from downvote to upvote
				else if (post.isDownvoted) {
					downvotes -= 1;
					upvotes += 1;
					isDownvoted = false;
					isUpvoted = true;
				}
				// New upvote
				else {
					upvotes += 1;
					isUpvoted = true;
				}

				// Optimistically update the feed cache
				const feedPatchResult = lifecycleApi.dispatch(
					postsApiSlice.util.updateQueryData(
						"getPostsCursor",
						undefined,
						(draft) => {
							const posts = transformPagesToFlatPosts(
								draft.pages
							);
							const updateCandidate = posts.find(
								(p) => p.id === post.id
							);
							if (updateCandidate) {
								updateCandidate.upvotes = upvotes;
								updateCandidate.downvotes = downvotes;
								updateCandidate.isUpvoted = isUpvoted;
								updateCandidate.isDownvoted = isDownvoted;
							}
						}
					)
				);

				// Optimistically update the individual post cache
				const postPatchResult = lifecycleApi.dispatch(
					postsApiSlice.util.updateQueryData(
						"getPost",
						post.id,
						(draft) => {
							if (draft) {
								draft.upvotes = upvotes;
								draft.downvotes = downvotes;
								draft.isUpvoted = isUpvoted;
								draft.isDownvoted = isDownvoted;
							}
						}
					)
				);

				try {
					await lifecycleApi.queryFulfilled;
				} catch {
					// Rollback on error
					feedPatchResult.undo();
					postPatchResult.undo();
				}
			},
		}),

		/**
		 * Downvote a post with optimistic updates.
		 */
		downvotePost: builder.mutation<Post, Post>({
			query: (post) => ({
				url: `/posts/${post.id}/downvote`,
				method: "PATCH",
			}),
			transformResponse: postResponseTransformer,
			onQueryStarted: async (post, lifecycleApi) => {
				// Calculate optimistic state
				let upvotes = post.upvotes;
				let downvotes = post.downvotes;
				let isUpvoted = post.isUpvoted;
				let isDownvoted = post.isDownvoted;

				// Toggle off if already downvoted
				if (post.isDownvoted) {
					downvotes -= 1;
					isDownvoted = false;
				}
				// Switch from upvote to downvote
				else if (post.isUpvoted) {
					downvotes += 1;
					upvotes -= 1;
					isDownvoted = true;
					isUpvoted = false;
				}
				// New downvote
				else {
					downvotes += 1;
					isDownvoted = true;
				}

				// Optimistically update the feed cache
				const feedPatchResult = lifecycleApi.dispatch(
					postsApiSlice.util.updateQueryData(
						"getPostsCursor",
						undefined,
						(draft) => {
							const posts = transformPagesToFlatPosts(
								draft.pages
							);
							const updateCandidate = posts.find(
								(p) => p.id === post.id
							);
							if (updateCandidate) {
								updateCandidate.upvotes = upvotes;
								updateCandidate.downvotes = downvotes;
								updateCandidate.isUpvoted = isUpvoted;
								updateCandidate.isDownvoted = isDownvoted;
							}
						}
					)
				);

				// Optimistically update the individual post cache
				const postPatchResult = lifecycleApi.dispatch(
					postsApiSlice.util.updateQueryData(
						"getPost",
						post.id,
						(draft) => {
							if (draft) {
								draft.upvotes = upvotes;
								draft.downvotes = downvotes;
								draft.isUpvoted = isUpvoted;
								draft.isDownvoted = isDownvoted;
							}
						}
					)
				);

				try {
					await lifecycleApi.queryFulfilled;
				} catch {
					// Rollback on error
					feedPatchResult.undo();
					postPatchResult.undo();
				}
			},
		}),
	}),
});

// ============================================================================
// Exported Hooks
// ============================================================================

export const {
	// Queries
	useGetPostsByUserIdQuery,
	useGetPostQuery,
	useGetPostsCursorInfiniteQuery,
	useGetSuggestedPostsQuery,
	useGetSimilarPostsQuery,
	// Mutations
	useCreatePostMutation,
	useUpdatePostMutation,
	useDeletePostMutation,
	useUpvotePostMutation,
	useDownvotePostMutation,
} = postsApiSlice;

// ============================================================================
// Selectors
// ============================================================================

/**
 * Select the posts cursor query result.
 */
export const selectPostsResult =
	postsApiSlice.endpoints.getPostsCursor.select(undefined);

/**
 * Select all posts from the feed (flattened from all pages).
 */
export const selectAllPosts = createSelector(selectPostsResult, (postsResult) =>
	transformPagesToFlatPosts(postsResult?.data?.pages)
);

/**
 * Select a specific post by ID from the feed cache.
 */
export const selectPostById = createSelector(
	[selectAllPosts, (_state: unknown, postId: string) => postId],
	(posts, postId) => posts.find((post) => post.id === postId)
);
