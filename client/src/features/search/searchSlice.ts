import {
	createEntityAdapter,
	EntityState,
} from "@reduxjs/toolkit";
import { Post, PostResponse } from "@/features/posts/types";
import { apiSlice } from "../api/apiSlice";
import { SEARCH_ENDPOINTS } from "@/lib/api/endpoints";

type SearchPostsState = EntityState<Post, string>;
const postsAdapter = createEntityAdapter<Post>({
	sortComparer: (a, b) => b.createdAt.localeCompare(a.createdAt),
});
const initialState: SearchPostsState = postsAdapter.getInitialState();

const searchLimit = 100;

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

export const searchApiSlice = apiSlice.injectEndpoints({
	endpoints: (builder) => ({
		searchPosts: builder.query<SearchPostsState, string>({
			query: (searchTerm) => ({
				url: SEARCH_ENDPOINTS.SEARCH_POSTS,
				params: {
					query: searchTerm,
					limit: searchLimit,
				},
			}),
			transformResponse: postsResponseTransformer,
			providesTags: (result = initialState, error, arg) => [
				"Post",
				{ type: "Post", id: "LIST" },
				...result.ids.map(
					(id) => ({ type: "Post", id: id.toString() }) as const
				),
      ],
		}),
	}),
});

export const { useSearchPostsQuery } = searchApiSlice;
