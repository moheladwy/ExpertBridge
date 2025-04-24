import { createEntityAdapter, createSelector, EntityState } from "@reduxjs/toolkit";
import { apiSlice } from "../api/apiSlice";
import { sub } from 'date-fns';
import { AddCommentRequest, AddReplyRequest, Comment, CommentResponse } from "./types";
import { request } from "http";

const commentResponseTransformer = (c: CommentResponse): Comment => ({
  ...c,
  createdAt: new Date(c.createdAt).toISOString(),
  replies: c.replies?.map(r => ({
    ...r,
    createdAt: new Date(r.createdAt).toISOString(),
  }) as Comment),
});

const commentsResponseTransformer = (response: CommentResponse[]) => {
  return response.map(commentResponseTransformer);
};

// type CommentsState = EntityState<Comment, string>;
// const commentsAdapter = createEntityAdapter<Comment>({
//   sortComparer: (a, b) => a.createdAt.localeCompare(b.createdAt),
// });
// const initialState: CommentsState = commentsAdapter.getInitialState();

export const commentsApiSlice = apiSlice.injectEndpoints({
  endpoints: (builder) => ({
    getCommentsByPostId: builder.query<Comment[], string>({
      query: (postId) => `/posts/${postId}/comments`,
      transformResponse: commentsResponseTransformer,
      providesTags: (result = [], error, arg) => [
        { type: 'Comment', id: `LIST/${arg}` } as const,
        ...result.map(({ id }) => ({ type: 'Comment', id }) as const),
      ],

    }),

    getCommentsByUserId: builder.query<Comment[], string>({
      query: (userId) => `/users/${userId}/comments`,
      transformResponse: commentsResponseTransformer,
      providesTags: (result = [], error, arg) => [
        { type: 'Comment', id: `LIST/${arg}` } as const,
        ...result.map(({ id }) => ({ type: 'Comment', id }) as const),
      ],
    }),

    getComment: builder.query<Comment, string>({
      query: (commentId) => `/comments/${commentId}`,
      providesTags: (result, error, arg) => [
        { type: 'Comment', id: arg },
      ],
      transformResponse: commentResponseTransformer
    }),

    createComment: builder.mutation<Comment, AddCommentRequest>({
      query: initialComment => ({
        url: '/comments',
        method: 'POST',
        body: initialComment,
      }),
      // invalidatesTags: (result, error, arg) => [
      //   { type: 'Comment', id: `LIST/${arg.postId}` },
      //   { type: 'Comment', id: arg.parentCommentId || '' } as const,
      // ],
      onQueryStarted: async (request, lifecycleApi) => {
        try {
          const { data: createdComment } = await lifecycleApi.queryFulfilled;
          const getCommentsByPostPatchResult = lifecycleApi.dispatch(
            commentsApiSlice.util.updateQueryData('getCommentsByPostId', request.postId, (draft) => {
              Object.assign(draft, draft.concat(createdComment));
              console.log(draft);
            }),
          );
          const getCommentPatchResult = lifecycleApi.dispatch(
            commentsApiSlice.util.upsertQueryData('getComment', createdComment.id, createdComment),
          );
        }
        catch {
          console.error('Comment creation failed');
        }
      }
    }),

    createReply: builder.mutation<Comment, AddReplyRequest>({
      query: initialComment => ({
        url: '/comments',
        method: 'POST',
        body: initialComment,
      }),
      // invalidatesTags: (result, error, arg) => [
      //   { type: 'Comment', id: `LIST/${arg.postId}` },
      //   { type: 'Comment', id: arg.parentCommentId },
      // ],
      onQueryStarted: async (request, lifecycleApi) => {
        try {
          console.log(`Req: ${request}`, request.postId);
          const { data: createdReply } = await lifecycleApi.queryFulfilled;
          const getCommentsByPostPatchResult = lifecycleApi.dispatch(
            commentsApiSlice.util.updateQueryData('getCommentsByPostId', request.postId, (draft) => {
              const parent = draft.find(c => c.id == request.parentCommentId);
              console.log(parent?.postId);
              console.log(draft);
              console.log(`Parent: ${parent}`);
              if (parent) {
                Object.assign(parent, { ...parent, replies: (parent.replies || []).concat(createdReply) });
              }
            }),
          );

          const getCommentPatchResult = lifecycleApi.dispatch(
            commentsApiSlice.util.updateQueryData('getComment', request.parentCommentId, (draft) => {
              Object.assign(draft, { ...draft, replies: (draft.replies || []).concat(createdReply) });
            }),
          );
        }
        catch {
          console.error('Reply creation failed');
        }
      }
    }),

    upvoteComment: builder.mutation<Comment, Comment>({
      query: comment => ({
        url: `comments/${comment.id}/upvote`,
        method: 'PATCH'
      }),
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
          commentsApiSlice.util.updateQueryData('getCommentsByPostId', comment.postId, (draft) => {
            // The `draft` is Immer-wrapped and can be "mutated" like in createSlice
            const updateCandidate = draft.find(c => c.id == comment.id);
            if (updateCandidate) {
              updateCandidate.upvotes = upvotes;
              updateCandidate.downvotes = downvotes;
              updateCandidate.isUpvoted = isUpvoted;
              updateCandidate.isDownvoted = isDownvoted;
            }
          }),
        );

        const getCommentPatchResult = lifecycleApi.dispatch(
          commentsApiSlice.util.updateQueryData('getComment', comment.id, (draft) => {
            // The `draft` is Immer-wrapped and can be "mutated" like in createSlice
            if (draft) {
              draft.upvotes = upvotes;
              draft.downvotes = downvotes;
              draft.isUpvoted = isUpvoted;
              draft.isDownvoted = isDownvoted;
            }
          }),
        );

        try {
          await lifecycleApi.queryFulfilled;
        }
        catch {
          getCommentsByPostPatchResult.undo();
          getCommentPatchResult.undo();
        }
      },
    }),

    downvoteComment: builder.mutation<Comment, Comment>({
      query: comment => ({
        url: `comments/${comment.id}/downvote`,
        method: 'PATCH'
      }),
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
          commentsApiSlice.util.updateQueryData('getCommentsByPostId', comment.postId, (draft) => {
            // The `draft` is Immer-wrapped and can be "mutated" like in createSlice
            const updateCandidate = draft.find(c => c.id == comment.id);
            if (updateCandidate) {
              updateCandidate.upvotes = upvotes;
              updateCandidate.downvotes = downvotes;
              updateCandidate.isUpvoted = isUpvoted;
              updateCandidate.isDownvoted = isDownvoted;
            }
          }),
        );

        const getCommentPatchResult = lifecycleApi.dispatch(
          commentsApiSlice.util.updateQueryData('getComment', comment.id, (draft) => {
            // The `draft` is Immer-wrapped and can be "mutated" like in createSlice
            if (draft) {
              draft.upvotes = upvotes;
              draft.downvotes = downvotes;
              draft.isUpvoted = isUpvoted;
              draft.isDownvoted = isDownvoted;
            }
          }),
        );

        try {
          await lifecycleApi.queryFulfilled;
        }
        catch {
          getCommentsByPostPatchResult.undo();
          getCommentPatchResult.undo();
        }
      },
    }),

  }),
});

export const {
  useGetCommentQuery,
  useGetCommentsByPostIdQuery,
  useGetCommentsByUserIdQuery,
  useCreateCommentMutation,
  useCreateReplyMutation,
  useUpvoteCommentMutation,
  useDownvoteCommentMutation,
} = commentsApiSlice;


// returns the query result object (does not initiate a request)
// export const selectCommentsResult = commentsApiSlice.endpoints.getCommentsByPostId.select();

// // Creates memoized selector
// const selectCommentsData = createSelector(
//   selectCommentsResult,
//   commentsResult => commentsResult.data ?? initialState,
// );

// export const {
//   selectAll: selectAllComments,
//   selectById: selectCommentById,
//   selectIds: selectCommentIds,
// } = commentsAdapter.getSelectors(selectCommentsData);

