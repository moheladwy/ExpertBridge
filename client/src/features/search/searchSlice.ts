import { createEntityAdapter, EntityState } from "@reduxjs/toolkit";
import { Post, PostResponse } from "@/features/posts/types";
import { apiSlice } from "@/features/api/apiSlice";
import { SEARCH_ENDPOINTS } from "@/lib/api/endpoints";
import {
  SearchJobPostsRequest,
  SearchUsersResponse,
} from "@/features/search/types";
import { JobPosting, JobPostingResponse } from "@/features/jobPostings/types";

type SearchPostsState = EntityState<Post, string>;

type JobPostingsState = EntityState<JobPosting, string>;
const jobPostingsAdapter = createEntityAdapter<JobPosting>({
  sortComparer: (a, b) => b.createdAt.localeCompare(a.createdAt),
});

const jobPostsInitialState: JobPostingsState =
  jobPostingsAdapter.getInitialState();

const jobPostingResponseTransformer = (p: JobPostingResponse): JobPosting => ({
  ...p,
  createdAt: new Date(p.createdAt).toISOString(),
  lastModified: p.lastModified ? new Date(p.lastModified).toISOString() : null,
});

const postsAdapter = createEntityAdapter<Post>({
  sortComparer: (a, b) => b.createdAt.localeCompare(a.createdAt),
});

const jobPostingsResponseTransformer = (response: JobPostingResponse[]) => {
  console.log(response);
  const postings: JobPosting[] = response.map(jobPostingResponseTransformer);

  return jobPostingsAdapter.setAll(jobPostsInitialState, postings);
};

const initialState: SearchPostsState = postsAdapter.getInitialState();

const searchLimit = 100;

const postResponseTransformer = (p: PostResponse): Post => ({
  ...p,
  createdAt: new Date(p.createdAt).toISOString(),
  lastModified: p.lastModified ? new Date(p.lastModified).toISOString() : null,
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
    }),
    searchUsers: builder.query<SearchUsersResponse[], string>({
      query: (searchTerm) => ({
        url: SEARCH_ENDPOINTS.SEARCH_USERS,
        params: {
          query: searchTerm,
          limit: searchLimit,
        },
      }),
    }),
    searchJobPosts: builder.query<JobPosting[], SearchJobPostsRequest>({
      query: (request) => ({
        url: SEARCH_ENDPOINTS.SEARCH_JOB_POSTS,
        params: {
          query: request.query,
          limit: request.limit,
          area: request.area,
          minBudget: request.minBudget,
          maxBudget: request.maxBudget,
          isRemote: request.isRemote,
        },
        transformResponse: jobPostingsResponseTransformer,
      }),
    }),
  }),
});

export const {
  useSearchPostsQuery,
  useSearchUsersQuery,
  useSearchJobPostsQuery,
} = searchApiSlice;
