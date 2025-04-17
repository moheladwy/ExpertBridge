import { createEntityAdapter, createSelector, EntityState } from "@reduxjs/toolkit";
import { apiSlice } from "../api/apiSlice";
import { sub } from 'date-fns';
import { AddCommentRequest, AddReplyRequest, Comment } from "./types";
import { request } from "http";


// type CommentsState = EntityState<Comment, string>;
// const commentsAdapter = createEntityAdapter<Comment>({
//   sortComparer: (a, b) => a.createdAt.localeCompare(b.createdAt),
// });
// const initialState: CommentsState = commentsAdapter.getInitialState();

export const commentsApiSlice = apiSlice.injectEndpoints({
  endpoints: (builder) => ({
    getCommentsByPostId: builder.query<Comment[], string>({
      query: (postId) => `/posts/${postId}/comments`,
      // transformResponse: (response: Comment[]) => {
      //   console.log(response);

      //   return commentsAdapter.setAll(initialState, response);
      // },
      providesTags: (result = [], error, arg) => [
        'Comment',
        { type: 'Comment', id: `LIST/${arg}` } as const,
        ...result.map(({ id }) => ({ type: 'Comment', id }) as const),
      ],

    }),

    getComment: builder.query<Comment, string>({
      query: (commentId) => `/comments/${commentId}`,
      providesTags: (result, error, arg) => [
        { type: 'Comment', id: arg },
      ],
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
      onQueryStarted: async (request, lifecycleApi) => {
        try {
          const { data: createdReply } = await lifecycleApi.queryFulfilled;
          const getCommentsByPostPatchResult = lifecycleApi.dispatch(
            commentsApiSlice.util.updateQueryData('getCommentsByPostId', request.postId, (draft) => {
              const parent = draft.find(c => c.id == request.parentCommentId);
              if (parent) {
                parent.replies = (parent.replies || []).concat(createdReply);
              }
            }),
          );

          const getCommentPatchResult = lifecycleApi.dispatch(
            commentsApiSlice.util.updateQueryData('getComment', request.parentCommentId, (draft) => {
              draft.replies = (draft.replies || []).concat(createdReply);
            }),
          );
        }
        catch {
          console.error('Reply creation failed');
        }
      }
    }),


  }),
});

export const {
  useGetCommentQuery,
  useGetCommentsByPostIdQuery,
  useCreateCommentMutation,
  useCreateReplyMutation,
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

