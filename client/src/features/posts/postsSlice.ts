import { createEntityAdapter, createSelector, EntityState } from "@reduxjs/toolkit";
import { emptyApiSlice } from "../api/apiSlice";
import { AddPostRequest, Post } from "./types";
import { sub } from 'date-fns';


type PostsState = EntityState<Post, number>;
const postsAdapter = createEntityAdapter<Post>({
  sortComparer: (a, b) => b.createdAt.localeCompare(a.createdAt),
});
const initialState: PostsState = postsAdapter.getInitialState();

export const postsApiSlice = emptyApiSlice.injectEndpoints({
  endpoints: (builder) => ({
    getPosts: builder.query<PostsState, void>({
      query: () => '/posts',
      transformResponse: (response) => {
        console.log(response);
        const min = 1;
        const loadedPosts = response.map(post => {
          // if (!post.date) post.date = sub(new Date(), { minutes: min++ }).toISOString();
          // if (!post.upvotes) post.upvotes = 0;
          // if (!post.downvotes) post.downvotes = 0;
          // if (!post.tags) post.tags = [];
          return post;
        })

        return postsAdapter.setAll(initialState, loadedPosts);
      },
      providesTags: (result = initialState, error, arg) => [
        'Post', { type: 'Post', id: 'LIST' },
        ...result.ids.map(id => ({ type: 'Post', id: id.toString() }) as const),
      ],

    }),

    getPost: builder.query<Post, string>({
      query: () => '',
      providesTags: [],
    }),

    addNewPost: builder.mutation<Post, AddPostRequest>({
      query: initialPost => ({
        url: '/posts',
        method: 'POST',
        body: {
          ...initialPost,
          upvotes: 0,
          downvotes: 0,
          date: new Date().toISOString(),
        }
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
  useAddNewPostMutation,
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

