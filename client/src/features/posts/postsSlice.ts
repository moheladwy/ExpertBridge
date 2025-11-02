import {
	createEntityAdapter,
	createSelector,
	EntityState,
} from "@reduxjs/toolkit";
import { apiSlice } from "../api/apiSlice";
import {
	AddPostRequest,
	Post,
	PostResponse,
	PostsCursorPaginatedResponse,
	PostsInitialPageParam,
	SimilarPostsResponse,
} from "./types";
import { RootState } from "@/app/store";
import { useAppSelector } from "@/app/hooks";

type PostsState = EntityState<Post, string>;
const postsAdapter = createEntityAdapter<Post>({
	sortComparer: (a, b) => b.createdAt.localeCompare(a.createdAt),
});

const initialState: PostsState = postsAdapter.getInitialState();

const postResponseTransformer = (p: PostResponse): Post => ({
	...p,
	createdAt: new Date(p.createdAt).toISOString(),
	lastModified: p.lastModified
		? new Date(p.lastModified).toISOString()
		: null,
});

const postsResponseTransformer = (response: PostResponse[]) => {
	console.log(response);
	const posts: Post[] = response.map(postResponseTransformer);

	return postsAdapter.setAll(initialState, posts);
};

const postsResponseTransformerWithoutAdapter = (
	response: PostResponse[]
): Post[] => {
	console.log(response);
	return response.map(postResponseTransformer);
};

// TODO: Needs optimization
const transformPagesToFlatPosts = (
	pages: PostsCursorPaginatedResponse[] | undefined
) => {
	let allPosts: Post[] = [];

	pages?.forEach((page) => {
		// allPosts = allPosts.concat(page.posts.map(postResponseTransformer));
		allPosts = allPosts.concat(page.posts);
	});

	return allPosts;
};

export const postsApiSlice = apiSlice.injectEndpoints({
	endpoints: (builder) => ({
		getPostsCursor: builder.infiniteQuery<
			PostsCursorPaginatedResponse,
			undefined, // query arg
			PostsInitialPageParam
		>({
			query: ({
				pageParam /*: { pageSize, after, page, embedding }*/,
			}) => {
				return {
					// url: `/posts?${params.toString()}`,
					url: "/posts/feed",
					method: "POST",
					body: pageParam,
				};
			},
			infiniteQueryOptions: {
				initialPageParam: { pageSize: 10, page: 1 },
				getNextPageParam: (
					lastPage,
					allPages,
					lastPageParam,
					allPageParams
				) => {
					if (!lastPage.pageInfo?.hasNextPage) {
						return undefined;
					}

					console.log(lastPageParam);

					return {
						after: lastPage.pageInfo.endCursor,
						pageSize: lastPageParam.pageSize,
						page: lastPageParam.page + 1,
						embedding: lastPage.pageInfo.embedding,
					};
				},
			},
		}),

		getSuggestedPosts: builder.query<SimilarPostsResponse[], number>({
			query: (limit) => ({
				url: `/posts/suggested?limit=${limit}`,
				method: "GET",
			}),
		}),

		// (DEPRECATED)
		getPosts: builder.query<PostsState, void>({
			query: () => "/posts",
			transformResponse: postsResponseTransformer,
			providesTags: (result = initialState, error, arg) => [
				"Post",
				{ type: "Post", id: "LIST" },
				...result.ids.map(
					(id) => ({ type: "Post", id: id.toString() }) as const
				),
			],
		}),

		getPost: builder.query<Post, string>({
			query: (postId) => `/posts/${postId}`,
			providesTags: (result, error, arg) => [{ type: "Post", id: arg }],
			transformResponse: postResponseTransformer,
		}),

		getAllPostsByProfileId: builder.query<Post[], string>({
			query: (profileId) => `/profiles/${profileId}/posts`,
			transformResponse: postsResponseTransformerWithoutAdapter,
			providesTags: (result, error, arg) => [
				{ type: "Post", id: arg },
				{ type: "Post", id: "LIST" },
				// ...result.map((post) => ({ type: "Post", id: post.id.toString() })),
			],
		}),

		getSimilarPosts: builder.query<SimilarPostsResponse[], string>({
			query: (postId) => `/posts/${postId}/similar?limit=5`,
			providesTags: (result, error, arg) => [
				{ type: "SimilarPosts", id: arg },
			],
		}),

		createPost: builder.mutation<Post, AddPostRequest>({
			query: (initialPost) => ({
				url: "/posts",
				method: "POST",
				body: initialPost,
			}),
			// invalidatesTags: [{ type: 'Post', id: 'LIST' }],
			transformResponse: postResponseTransformer,
			onQueryStarted: async (request, lifecycleApi) => {
				try {
					const { data: createdPost } =
						await lifecycleApi.queryFulfilled;

					const getPostsPatchResult = lifecycleApi.dispatch(
						postsApiSlice.util.updateQueryData(
							"getPostsCursor",
							undefined,
							(draft) => {
								draft.pages[0].posts.push(createdPost);
							}
						)
					);

					const getPostPatchResult = lifecycleApi.dispatch(
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

		upvotePost: builder.mutation<Post, Post>({
			query: (post) => ({
				url: `/posts/${post.id}/upvote`,
				method: "PATCH",
			}),
			transformResponse: postResponseTransformer,
			onQueryStarted: async (post, lifecycleApi) => {
				let upvotes = post.upvotes;
				let downvotes = post.downvotes;
				let isUpvoted = post.isUpvoted;
				let isDownvoted = post.isDownvoted;

				// toggle
				if (post.isUpvoted) {
					upvotes -= 1;
					isUpvoted = false;
				} // change opposites
				else if (post.isDownvoted) {
					downvotes -= 1;
					upvotes += 1;
					isDownvoted = false;
					isUpvoted = true;
				} // new vote
				else {
					upvotes += 1;
					isUpvoted = true;
				}

				const getPostsPatchResult = lifecycleApi.dispatch(
					postsApiSlice.util.updateQueryData(
						"getPostsCursor",
						undefined,
						(draft) => {
							// The `draft` is Immer-wrapped and can be 'mutated' like in createSlice
							// const updateCandidate = draft.entities[post.id];
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

				const getPostPatchResult = lifecycleApi.dispatch(
					postsApiSlice.util.updateQueryData(
						"getPost",
						post.id,
						(draft) => {
							// The `draft` is Immer-wrapped and can be 'mutated' like in createSlice
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
					getPostsPatchResult.undo();
					getPostPatchResult.undo();
				}
			},
		}),

		downvotePost: builder.mutation<Post, Post>({
			query: (post) => ({
				url: `/posts/${post.id}/downvote`,
				method: "PATCH",
			}),
			transformResponse: postResponseTransformer,
			onQueryStarted: async (post, lifecycleApi) => {
				let upvotes = post.upvotes;
				let downvotes = post.downvotes;
				let isUpvoted = post.isUpvoted;
				let isDownvoted = post.isDownvoted;

				// toggle
				if (post.isDownvoted) {
					downvotes -= 1;
					isDownvoted = false;
				} // change opposites
				else if (post.isUpvoted) {
					downvotes += 1;
					upvotes -= 1;
					isDownvoted = true;
					isUpvoted = false;
				} // new vote
				else {
					downvotes += 1;
					isDownvoted = true;
				}

				const getPostsPatchResult = lifecycleApi.dispatch(
					postsApiSlice.util.updateQueryData(
						"getPostsCursor",
						undefined,
						(draft) => {
							// The `draft` is Immer-wrapped and can be 'mutated' like in createSlice
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

				const getPostPatchResult = lifecycleApi.dispatch(
					postsApiSlice.util.updateQueryData(
						"getPost",
						post.id,
						(draft) => {
							// The `draft` is Immer-wrapped and can be 'mutated' like in createSlice
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
					getPostsPatchResult.undo();
					getPostPatchResult.undo();
				}
			},
		}),

		updatePost: builder.mutation<
			Post,
			{ postId: string; title?: string; content?: string }
		>({
			query: ({ postId, ...updateData }) => ({
				url: `/posts/${postId}`,
				method: "PATCH",
				body: updateData,
			}),
			onQueryStarted: async (request, lifecycleApi) => {
				const getPostsPatchResult = lifecycleApi.dispatch(
					postsApiSlice.util.updateQueryData(
						"getPostsCursor",
						undefined,
						(draft) => {
							// const updateCandidate = draft.entities[request.postId];
							const updateCandidate = useAppSelector((state) =>
								selectPostById(state, request.postId)
							);
							if (updateCandidate) {
								updateCandidate.title =
									request.title ?? updateCandidate.title;
								updateCandidate.content =
									request.content ?? updateCandidate.content;
							}
						}
					)
				);

				const getPostPatchResult = lifecycleApi.dispatch(
					postsApiSlice.util.updateQueryData(
						"getPost",
						request.postId,
						(draft) => {
							// The `draft` is Immer-wrapped and can be 'mutated' like in createSlice
							if (draft) {
								draft.title = request.title ?? draft.title;
								draft.content =
									request.content ?? draft.content;
							}
						}
					)
				);

				try {
					await lifecycleApi.queryFulfilled;
				} catch {
					getPostsPatchResult.undo();
					getPostPatchResult.undo();
				}
			},
		}),

		deletePost: builder.mutation<void, string>({
			query: (postId) => ({
				url: `/posts/${postId}`,
				method: "DELETE",
			}),
			onQueryStarted: async (postId, lifecycleApi) => {
				try {
					const response = await lifecycleApi.queryFulfilled;

					const getPostsPatchResult = lifecycleApi.dispatch(
						postsApiSlice.util.updateQueryData(
							"getPostsCursor",
							undefined,
							(draft) => {
								// postsAdapter.removeOne(draft, postId);
								const page = draft.pages.find(
									(p) =>
										p.posts.findIndex(
											(post) => post.id === postId
										) !== -1
								);
								if (!page) return;

								page.posts = page.posts.filter(
									(post) => post.id !== postId
								);
							}
						)
					);

					const getPostPatchResult = lifecycleApi.dispatch(
						postsApiSlice.util.updateQueryData(
							"getPost",
							postId,
							(draft) => {
								Object.assign(draft, null);
							}
						)
					);
				} catch {
					console.error("error while deleting post");
				}
			},
		}),
	}),
});

export const {
	useGetPostQuery,
	useGetSimilarPostsQuery,
	useGetAllPostsByProfileIdQuery,
	useGetSuggestedPostsQuery,
	useGetPostsQuery,
	useGetPostsCursorInfiniteQuery,
	useCreatePostMutation,
	useUpvotePostMutation,
	useDownvotePostMutation,
	useUpdatePostMutation,
	useDeletePostMutation,
} = postsApiSlice;

// // returns the query result object (does not initiate a request)
// export const selectPostsResult = postsApiSlice.endpoints.getPosts.select();

// // Creates memoized selector
// const selectPostsData = createSelector(
// 	selectPostsResult,
// 	(postsResult) => postsResult.data ?? initialState
// );

// export const {
// 	selectAll: selectAllPosts,
// 	selectById: selectPostById,
// 	selectIds: selectPostIds,
// } = postsAdapter.getSelectors(selectPostsData);

// Calling `someEndpoint.select(someArg)` generates a new selector that will return
// the query result object for a query with those parameters.
// To generate a selector for a specific query argument, call `select(theQueryArg)`.
// In this case, the 'Posts' query has no params, so we don't pass anything to select()
export const selectPostsResult =
	postsApiSlice.endpoints.getPostsCursor.select(undefined);

const selectPostsData = createSelector(
	selectPostsResult,
	// Fall back to the empty entity state if no response yet.
	(result) => result.data?.pages.flat() ?? []
);

export const selectAllPosts = createSelector(selectPostsResult, (postsResult) =>
	transformPagesToFlatPosts(postsResult?.data?.pages)
);

export const selectPostById = createSelector(
	selectAllPosts,
	(state: RootState, postId: string) => postId,
	(posts, postId) => posts.find((post) => post.id === postId)
);
