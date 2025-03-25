import { createEntityAdapter, createSelector, EntityState } from "@reduxjs/toolkit";
import { emptyApiSlice } from "../api/apiSlice";
import { sub } from 'date-fns';
import { AddCommentRequest, Comment } from "./types";


type CommentsState = EntityState<Comment, string>;
const commentsAdapter = createEntityAdapter<Comment>({
  sortComparer: (a, b) => b.createdAt.localeCompare(a.createdAt),
});
const initialState: CommentsState = commentsAdapter.getInitialState();

export const commentsApiSlice = emptyApiSlice.injectEndpoints({
  endpoints: (builder) => ({
    getCommentsByPostId: builder.query<CommentsState, string>({
      query: (postId) => '',
      transformResponse: (response: any[]) => {
        console.log(response);
        const min = 1;
        const loadedComments = response.map(comment => {
          // if (!comment.date) comment.date = sub(new Date(), { minutes: min++ }).toISOString();
          // if (!comment.upvotes) comment.upvotes = 0;
          // if (!comment.downvotes) comment.downvotes = 0;
          // if (!comment.tags) comment.tags = [];
          return comment;
        })

        return commentsAdapter.setAll(initialState, loadedComments);
      },
      providesTags: (result = initialState, error, arg) => [
        'Comment',
        { type: 'Comment', id: `LIST/${arg}` },
        ...result.ids.map(id => ({ type: 'Comment', id: id.toString() }) as const),
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
      invalidatesTags: (result, error, arg) => [
        { type: 'Comment', id: `LIST/${arg.postId}` },
      ],
    }),


  }),
});

export const {
  useGetCommentQuery,
  useGetCommentsByPostIdQuery,
  useCreateCommentMutation,
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

