import { createEntityAdapter, createSelector, EntityState } from "@reduxjs/toolkit";
import { apiSlice } from "../api/apiSlice";
import { AddPostRequest, Post } from "./types";
import { sub } from 'date-fns';


type PostsState = EntityState<Post, string>;
const postsAdapter = createEntityAdapter<Post>({
  sortComparer: (a, b) => b.createdAt.localeCompare(a.createdAt),
});
const initialState: PostsState = postsAdapter.getInitialState();

export const postsApiSlice = apiSlice.injectEndpoints({
  endpoints: (builder) => ({
    getPosts: builder.query<PostsState, void>({
      query: () => '/posts',
      transformResponse: (response: Post[]) => {
        console.log(response);

        return postsAdapter.setAll(initialState, response);
      },
      providesTags: (result = initialState, error, arg) => [
        'Post',
        { type: 'Post', id: 'LIST' },
        ...result.ids.map(id => ({ type: 'Post', id: id.toString() }) as const),
      ],
    }),

    getPost: builder.query<Post, string>({
      query: (postId) => `/posts/${postId}`,
      providesTags: (result, error, arg) => [
        { type: 'Post', id: arg },
      ],
    }),

    createPost: builder.mutation<Post, AddPostRequest>({
      query: initialPost => ({
        url: '/posts',
        method: 'POST',
        body: initialPost,
      }),
      invalidatesTags: [
        { type: 'Post', id: 'LIST' },
      ],
    }),

    upvotePost: builder.mutation<Post, Post>({
      query: post => ({
        url: `/posts/${post.id}/upvote`,
        method: 'PATCH',
      }),
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
          postsApiSlice.util.updateQueryData('getPosts', undefined, (draft) => {
            // The `draft` is Immer-wrapped and can be "mutated" like in createSlice
            const updateCandidate = draft.entities[post.id];
            if (updateCandidate) {
              updateCandidate.upvotes = upvotes;
              updateCandidate.downvotes = downvotes;
              updateCandidate.isUpvoted = isUpvoted;
              updateCandidate.isDownvoted = isDownvoted;
            }
          }),
        );

        const getPostPatchResult = lifecycleApi.dispatch(
          postsApiSlice.util.updateQueryData('getPost', post.id, (draft) => {
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
          getPostsPatchResult.undo();
          getPostPatchResult.undo();
        }
      }
    }),

    downvotePost: builder.mutation<Post, Post>({
      query: post => ({
        url: `/posts/${post.id}/downvote`,
        method: 'PATCH',
      }),
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
          postsApiSlice.util.updateQueryData('getPosts', undefined, (draft) => {
            // The `draft` is Immer-wrapped and can be "mutated" like in createSlice
            const updateCandidate = draft.entities[post.id];
            if (updateCandidate) {
              updateCandidate.upvotes = upvotes;
              updateCandidate.downvotes = downvotes;
              updateCandidate.isUpvoted = isUpvoted;
              updateCandidate.isDownvoted = isDownvoted;
            }
          }),
        );

        const getPostPatchResult = lifecycleApi.dispatch(
          postsApiSlice.util.updateQueryData('getPost', post.id, (draft) => {
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
          getPostsPatchResult.undo();
          getPostPatchResult.undo();
        }
      }
    }),

    deletePost: builder.mutation<void, string>({
      query: (postId) => ({
        url: `/posts/${postId}`,
        method: 'DELETE',
      }),
      invalidatesTags: (result, extra, arg) => [
        { type: 'Post', id: 'LIST' },
        { type: 'Post', id: arg },
      ]
    }),

  }),
});

export const {
  useGetPostQuery,
  useGetPostsQuery,
  useCreatePostMutation,
  useUpvotePostMutation,
  useDownvotePostMutation,
  useDeletePostMutation,
} = postsApiSlice;


// returns the query result object (does not initiate a request)
export const selectPostsResult = postsApiSlice.endpoints.getPosts.select();

// Creates memoized selector
const selectPostsData = createSelector(
  selectPostsResult,
  postsResult => postsResult.data ?? initialState,
);

export const {
  selectAll: selectAllPosts,
  selectById: selectPostById,
  selectIds: selectPostIds,
} = postsAdapter.getSelectors(selectPostsData)

