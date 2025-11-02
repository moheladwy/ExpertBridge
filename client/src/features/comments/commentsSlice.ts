import { apiSlice } from "../api/apiSlice";
import {
	AddCommentRequest,
	AddReplyRequest,
	Comment,
	CommentResponse,
	DeleteCommentRequest,
	UpdateCommentRequest,
} from "./types";

const commentResponseTransformer = (c: CommentResponse): Comment => ({
	...c,
	createdAt: new Date(c.createdAt).toISOString(),
	lastModified: c.lastModified
		? new Date(c.lastModified).toISOString()
		: undefined,
	replies: c.replies?.map(
		(r) =>
			({
				...r,
				createdAt: new Date(r.createdAt).toISOString(),
			}) as Comment
	),
});

const commentsResponseTransformer = (response: CommentResponse[]) => {
	return response.map(commentResponseTransformer);
};

export const commentsApiSlice = apiSlice.injectEndpoints({
	endpoints: (builder) => ({
		getCommentsByPostId: builder.query<Comment[], string>({
			query: (postId) => `/posts/${postId}/comments`,
			transformResponse: commentsResponseTransformer,
			providesTags: (result = [], error, arg) => [
				{ type: "Comment", id: `LIST/${arg}` } as const,
				...result.map(({ id }) => ({ type: "Comment", id }) as const),
			],
		}),

		getCommentsByJobPostingId: builder.query<Comment[], string>({
			query: (jobPostingId) => `/jobPostings/${jobPostingId}/comments`,
			transformResponse: commentsResponseTransformer,
			providesTags: (result = [], error, arg) => [
				{ type: "Comment", id: `LIST-JOB/${arg}` } as const,
				...result.map(({ id }) => ({ type: "Comment", id }) as const),
			],
		}),

		getCommentsByUserId: builder.query<Comment[], string>({
			query: (profileId) => `/profiles/${profileId}/comments`,
			transformResponse: commentsResponseTransformer,
			providesTags: (result = [], error, arg) => [
				{ type: "Comment", id: `LIST/${arg}` } as const,
				...result.map(({ id }) => ({ type: "Comment", id }) as const),
			],
		}),

		getComment: builder.query<Comment, string>({
			query: (commentId) => `/comments/${commentId}`,
			providesTags: (result, error, arg) => [
				{ type: "Comment", id: arg },
			],
			transformResponse: commentResponseTransformer,
		}),

		createComment: builder.mutation<Comment, AddCommentRequest>({
			query: (initialComment) => ({
				url: "/comments",
				method: "POST",
				body: initialComment,
			}),
			transformResponse: commentResponseTransformer,
			// invalidatesTags: (result, error, arg) => [
			//   { type: 'Comment', id: `LIST/${arg.postId}` },
			//   { type: 'Comment', id: arg.parentCommentId || '' } as const,
			// ],
			onQueryStarted: async (request, lifecycleApi) => {
				try {
					const { data: createdComment } =
						await lifecycleApi.queryFulfilled;
					const getCommentsByPostPatchResult = lifecycleApi.dispatch(
						commentsApiSlice.util.updateQueryData(
							"getCommentsByPostId",
							request.postId!,
							(draft) => {
								if (!createdComment.postId) return;

								Object.assign(
									draft,
									draft.concat(createdComment)
								);
								console.log(draft);
							}
						)
					);

					const getCommentsByJobPatchResult = lifecycleApi.dispatch(
						commentsApiSlice.util.updateQueryData(
							"getCommentsByJobPostingId",
							request.jobPostingId!,
							(draft) => {
								if (!createdComment.jobPostingId) return;

								Object.assign(
									draft,
									draft.concat(createdComment)
								);
								console.log(draft);
							}
						)
					);

					const getCommentsByUserPatchResult = lifecycleApi.dispatch(
						commentsApiSlice.util.updateQueryData(
							"getCommentsByUserId",
							createdComment.authorId,
							(draft) => {
								Object.assign(
									draft,
									draft.concat(createdComment)
								);
								console.log(draft);
							}
						)
					);

					const getCommentPatchResult = lifecycleApi.dispatch(
						commentsApiSlice.util.upsertQueryData(
							"getComment",
							createdComment.id,
							createdComment
						)
					);
				} catch {
					console.error("Comment creation failed");
				}
			},
		}),

		createReply: builder.mutation<Comment, AddReplyRequest>({
			query: (initialComment) => ({
				url: "/comments",
				method: "POST",
				body: initialComment,
			}),
			transformResponse: commentResponseTransformer,
			onQueryStarted: async (request, lifecycleApi) => {
				try {
					const { data: createdReply } =
						await lifecycleApi.queryFulfilled;
					const getCommentsByPostPatchResult = lifecycleApi.dispatch(
						commentsApiSlice.util.updateQueryData(
							"getCommentsByPostId",
							request.postId!,
							(draft) => {
								if (!createdReply.postId) return;

								const parent = draft.find(
									(c) => c.id === request.parentCommentId
								);
								if (parent) {
									Object.assign(parent, {
										...parent,
										replies: (parent.replies || []).concat(
											createdReply
										),
									});
								}
							}
						)
					);

					const getCommentsByJobPatchResult = lifecycleApi.dispatch(
						commentsApiSlice.util.updateQueryData(
							"getCommentsByJobPostingId",
							request.jobPostingId!,
							(draft) => {
								if (!createdReply.jobPostingId) return;

								const parent = draft.find(
									(c) => c.id === request.parentCommentId
								);
								if (parent) {
									Object.assign(parent, {
										...parent,
										replies: (parent.replies || []).concat(
											createdReply
										),
									});
								}
							}
						)
					);

					const getCommentPatchResult = lifecycleApi.dispatch(
						commentsApiSlice.util.updateQueryData(
							"getComment",
							request.parentCommentId,
							(draft) => {
								Object.assign(draft, {
									...draft,
									replies: (draft.replies || []).concat(
										createdReply
									),
								});
							}
						)
					);
				} catch {
					console.error("Reply creation failed");
				}
			},
		}),

		upvoteComment: builder.mutation<Comment, Comment>({
			query: (comment) => ({
				url: `comments/${comment.id}/upvote`,
				method: "PATCH",
			}),
			transformResponse: commentResponseTransformer,
			onQueryStarted: async (comment, lifecycleApi) => {
				let upvotes = comment.upvotes;
				let downvotes = comment.downvotes;
				let isUpvoted = comment.isUpvoted;
				let isDownvoted = comment.isDownvoted;

				// toggle
				if (comment.isUpvoted) {
					upvotes -= 1;
					isUpvoted = false;
				} // change opposites
				else if (comment.isDownvoted) {
					downvotes -= 1;
					upvotes += 1;
					isDownvoted = false;
					isUpvoted = true;
				} // new vote
				else {
					upvotes += 1;
					isUpvoted = true;
				}

				const getCommentsByPostPatchResult = lifecycleApi.dispatch(
					commentsApiSlice.util.updateQueryData(
						"getCommentsByPostId",
						comment.postId!,
						(draft) => {
							if (!comment.postId) return;

							// The `draft` is Immer-wrapped and can be "mutated" like in createSlice
							const updateCandidate = draft.find(
								(c) => c.id === comment.id
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

				const getCommentsByJobPatchResult = lifecycleApi.dispatch(
					commentsApiSlice.util.updateQueryData(
						"getCommentsByJobPostingId",
						comment.jobPostingId!,
						(draft) => {
							if (!comment.jobPostingId) return;

							// The `draft` is Immer-wrapped and can be "mutated" like in createSlice
							const updateCandidate = draft.find(
								(c) => c.id === comment.id
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

				const getCommentsByUserPatchResult = lifecycleApi.dispatch(
					commentsApiSlice.util.updateQueryData(
						"getCommentsByUserId",
						comment.authorId,
						(draft) => {
							// The `draft` is Immer-wrapped and can be "mutated" like in createSlice
							const updateCandidate = draft.find(
								(c) => c.id === comment.id
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

				const getCommentPatchResult = lifecycleApi.dispatch(
					commentsApiSlice.util.updateQueryData(
						"getComment",
						comment.id,
						(draft) => {
							// The `draft` is Immer-wrapped and can be "mutated" like in createSlice
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
					getCommentsByPostPatchResult.undo();
					getCommentPatchResult.undo();
				}
			},
		}),

		downvoteComment: builder.mutation<Comment, Comment>({
			query: (comment) => ({
				url: `comments/${comment.id}/downvote`,
				method: "PATCH",
			}),
			transformResponse: commentResponseTransformer,
			onQueryStarted: async (comment, lifecycleApi) => {
				let upvotes = comment.upvotes;
				let downvotes = comment.downvotes;
				let isUpvoted = comment.isUpvoted;
				let isDownvoted = comment.isDownvoted;

				// toggle
				if (comment.isDownvoted) {
					downvotes -= 1;
					isDownvoted = false;
				} // change opposites
				else if (comment.isUpvoted) {
					downvotes += 1;
					upvotes -= 1;
					isDownvoted = true;
					isUpvoted = false;
				} // new vote
				else {
					downvotes += 1;
					isDownvoted = true;
				}

				const getCommentsByPostPatchResult = lifecycleApi.dispatch(
					commentsApiSlice.util.updateQueryData(
						"getCommentsByPostId",
						comment.postId!,
						(draft) => {
							if (!comment.postId) return;

							// The `draft` is Immer-wrapped and can be "mutated" like in createSlice
							const updateCandidate = draft.find(
								(c) => c.id === comment.id
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

				const getCommentsByJobPatchResult = lifecycleApi.dispatch(
					commentsApiSlice.util.updateQueryData(
						"getCommentsByJobPostingId",
						comment.jobPostingId!,
						(draft) => {
							if (!comment.jobPostingId) return;

							// The `draft` is Immer-wrapped and can be "mutated" like in createSlice
							const updateCandidate = draft.find(
								(c) => c.id === comment.id
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

				const getCommentsByUserPatchResult = lifecycleApi.dispatch(
					commentsApiSlice.util.updateQueryData(
						"getCommentsByUserId",
						comment.authorId,
						(draft) => {
							// The `draft` is Immer-wrapped and can be "mutated" like in createSlice
							const updateCandidate = draft.find(
								(c) => c.id === comment.id
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

				const getCommentPatchResult = lifecycleApi.dispatch(
					commentsApiSlice.util.updateQueryData(
						"getComment",
						comment.id,
						(draft) => {
							// The `draft` is Immer-wrapped and can be "mutated" like in createSlice
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
					getCommentsByPostPatchResult.undo();
					getCommentPatchResult.undo();
				}
			},
		}),

		updateComment: builder.mutation<Comment, UpdateCommentRequest>({
			query: (request) => ({
				url: `/comments/${request.commentId}`,
				method: "PATCH",
				body: request,
			}),
			onQueryStarted: async (request, lifecycleApi) => {
				const getByPostPatchResult = lifecycleApi.dispatch(
					commentsApiSlice.util.updateQueryData(
						"getCommentsByPostId",
						request.postId!,
						(draft) => {
							if (!request.postId) return;

							const updateCandidate = draft.find(
								(c) => c.id === request.commentId
							);
							if (updateCandidate) {
								updateCandidate.content =
									request.content ?? updateCandidate.content;
							}
						}
					)
				);

				const getByJobPatchResult = lifecycleApi.dispatch(
					commentsApiSlice.util.updateQueryData(
						"getCommentsByJobPostingId",
						request.jobPostingId!,
						(draft) => {
							if (!request.jobPostingId) return;

							const updateCandidate = draft.find(
								(c) => c.id === request.commentId
							);
							if (updateCandidate) {
								updateCandidate.content =
									request.content ?? updateCandidate.content;
							}
						}
					)
				);

				const getByUserPatchResult = lifecycleApi.dispatch(
					commentsApiSlice.util.updateQueryData(
						"getCommentsByUserId",
						request.authorId,
						(draft) => {
							const updateCandidate = draft.find(
								(c) => c.id === request.commentId
							);
							if (updateCandidate) {
								updateCandidate.content =
									request.content ?? updateCandidate.content;
							}
						}
					)
				);

				const getCommentPatchResult = lifecycleApi.dispatch(
					commentsApiSlice.util.updateQueryData(
						"getComment",
						request.commentId,
						(draft) => {
							if (draft) {
								draft.content =
									request.content ?? draft.content;
							}
						}
					)
				);

				// Handle reply edits
				if (request.parentCommentId) {
					const getCommentsByPostPatchResult2 = lifecycleApi.dispatch(
						commentsApiSlice.util.updateQueryData(
							"getCommentsByPostId",
							request.postId!,
							(draft) => {
								if (!request.postId) return;

								const parent = draft.find(
									(c) => c.id === request.parentCommentId
								);
								if (parent?.replies) {
									const reply = parent.replies.find(
										(r) => r.id === request.commentId
									);
									if (reply) {
										reply.content =
											request.content ?? reply.content;
									}
								}
							}
						)
					);

					const getCommentsByJobPatchResult2 = lifecycleApi.dispatch(
						commentsApiSlice.util.updateQueryData(
							"getCommentsByJobPostingId",
							request.jobPostingId!,
							(draft) => {
								if (!request.jobPostingId) return;

								const parent = draft.find(
									(c) => c.id === request.parentCommentId
								);
								if (parent?.replies) {
									const reply = parent.replies.find(
										(r) => r.id === request.commentId
									);
									if (reply) {
										reply.content =
											request.content ?? reply.content;
									}
								}
							}
						)
					);

					const getCommentPatchResult2 = lifecycleApi.dispatch(
						commentsApiSlice.util.updateQueryData(
							"getComment",
							request.parentCommentId,
							(draft) => {
								const parent = draft;
								if (parent?.replies) {
									const reply = parent.replies.find(
										(r) => r.id === request.commentId
									);
									if (reply) {
										reply.content =
											request.content ?? reply.content;
									}
								}
							}
						)
					);

					try {
						await lifecycleApi.queryFulfilled;
					} catch {
						getCommentsByPostPatchResult2.undo();
						getCommentPatchResult2.undo();
					}
				}

				try {
					await lifecycleApi.queryFulfilled;
				} catch {
					getByPostPatchResult.undo();
					getByUserPatchResult.undo();
					getCommentPatchResult.undo();
				}
			},
		}),

		deleteComment: builder.mutation<void, DeleteCommentRequest>({
			query: (request) => ({
				url: `/comments/${request.commentId}`,
				method: "DELETE",
			}),
			invalidatesTags: (_, __, arg) => [
				{ type: "Comment", id: `LIST/${arg.authorId}` },
			],
			onQueryStarted: async (request, lifecycleApi) => {
				try {
					const response = await lifecycleApi.queryFulfilled;

					const getCommentsByPostPatchResult = lifecycleApi.dispatch(
						commentsApiSlice.util.updateQueryData(
							"getCommentsByPostId",
							request.postId!,
							(draft) => {
								if (!request.postId) return;

								Object.assign(
									draft,
									draft.filter(
										(c) => c.id !== request.commentId
									)
								);
							}
						)
					);

					const getCommentsByJobPatchResult = lifecycleApi.dispatch(
						commentsApiSlice.util.updateQueryData(
							"getCommentsByJobPostingId",
							request.jobPostingId!,
							(draft) => {
								if (!request.jobPostingId) return;

								Object.assign(
									draft,
									draft.filter(
										(c) => c.id !== request.commentId
									)
								);
							}
						)
					);

					const getCommentPatchResult = lifecycleApi.dispatch(
						commentsApiSlice.util.updateQueryData(
							"getComment",
							request.commentId,
							(draft) => {
								Object.assign(draft, null);
							}
						)
					);

					// Handle reply deletion
					if (request.parentCommentId) {
						const getCommentsByPostPatchResult =
							lifecycleApi.dispatch(
								commentsApiSlice.util.updateQueryData(
									"getCommentsByPostId",
									request.postId!,
									(draft) => {
										if (!request.postId) return;

										const parent = draft.find(
											(c) =>
												c.id === request.parentCommentId
										);
										if (parent) {
											Object.assign(parent, {
												...parent,
												replies: (
													parent.replies || []
												).filter(
													(r) =>
														r.id !==
														request.commentId
												),
											});
										}
									}
								)
							);

						const getCommentsByJobPatchResult =
							lifecycleApi.dispatch(
								commentsApiSlice.util.updateQueryData(
									"getCommentsByJobPostingId",
									request.jobPostingId!,
									(draft) => {
										if (!request.jobPostingId) return;

										const parent = draft.find(
											(c) =>
												c.id === request.parentCommentId
										);
										if (parent) {
											Object.assign(parent, {
												...parent,
												replies: (
													parent.replies || []
												).filter(
													(r) =>
														r.id !==
														request.commentId
												),
											});
										}
									}
								)
							);

						const getCommentPatchResult = lifecycleApi.dispatch(
							commentsApiSlice.util.updateQueryData(
								"getComment",
								request.parentCommentId,
								(draft) => {
									Object.assign(draft, {
										...draft,
										replies: (draft.replies || []).filter(
											(r) => r.id !== request.commentId
										),
									});
								}
							)
						);
					}
				} catch {
					console.error("error while deleting post");
				}
			},
		}),
	}),
});

export const {
	useGetCommentQuery,
	useGetCommentsByPostIdQuery,
	useGetCommentsByJobPostingIdQuery,
	useGetCommentsByUserIdQuery,
	useCreateCommentMutation,
	useCreateReplyMutation,
	useUpvoteCommentMutation,
	useDownvoteCommentMutation,
	useDeleteCommentMutation,
	useUpdateCommentMutation,
} = commentsApiSlice;
