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


  }),
});

export const {
  useGetPostQuery,
  useGetPostsQuery,
  useCreatePostMutation,
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

