import {
  createEntityAdapter,
  createSelector,
  EntityState,
} from "@reduxjs/toolkit";
import { apiSlice } from "../api/apiSlice";
import { RootState } from "@/app/store";
import { useAppSelector } from "@/app/hooks";
import { ApplyToJobPostingRequest, CreateJobPostingRequest, JobApplicationResponse, JobPosting, JobPostingPaginatedResponse, JobPostingResponse, JobPostingsInitialPageParam, SimilarJobsResponse } from "./types";

type JobPostingsState = EntityState<JobPosting, string>;
const jobPostingsAdapter = createEntityAdapter<JobPosting>({
  sortComparer: (a, b) => b.createdAt.localeCompare(a.createdAt),
});

const initialState: JobPostingsState = jobPostingsAdapter.getInitialState();

const jobPostingResponseTransformer = (p: JobPostingResponse): JobPosting => ({
  ...p,
  createdAt: new Date(p.createdAt).toISOString(),
  lastModified: p.lastModified
    ? new Date(p.lastModified).toISOString()
    : null,
});

const jobPostingsResponseTransformer = (response: JobPostingResponse[]) => {
  console.log(response);
  const postings: JobPosting[] = response.map(jobPostingResponseTransformer);

  return jobPostingsAdapter.setAll(initialState, postings);
};

// TODO: Needs optimization
const transformPagesToFlatPosts = (
  pages: JobPostingPaginatedResponse[] | undefined
) => {
  let allPostings: JobPosting[] = [];

  pages?.forEach((page) => {
    // allPosts = allPosts.concat(page.posts.map(postResponseTransformer));
    allPostings = allPostings.concat(page.jobPostings);
  });

  return allPostings;
};

export const jobPostingsApiSlice = apiSlice.injectEndpoints({
  endpoints: (builder) => ({
    getJobsCursor: builder.infiniteQuery<
      JobPostingPaginatedResponse,
      undefined, // query arg
      JobPostingsInitialPageParam
    >({
      query: ({ pageParam/*: { pageSize, after, page, embedding }*/ }) => {
        // const params = new URLSearchParams();
        // params.append('pageSize', String(pageSize));
        // params.append('page', String(page));

        // if (after != null) {
        // 	params.append('after', String(after))
        // }

        // if (embedding != null) {
        // 	params.append('embedding', String(embedding));
        // }

        return {
          // url: `/posts?${params.toString()}`,
          url: '/jobPostings/feed',
          method: 'POST',
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
            embedding: lastPage.pageInfo.embedding
          };
        },
      },
    }),

    getJobPosting: builder.query<JobPosting, string>({
      query: (postingId) => `/jobPostings/${postingId}`,
      providesTags: (result, error, arg) => [{ type: "JobPosting", id: arg }],
      transformResponse: jobPostingResponseTransformer,
    }),

    getSimilarJobs: builder.query<SimilarJobsResponse[], string>({
      query: (postingId) => `/jobPostings/${postingId}/similar?limit=5`,
      providesTags: (result, error, arg) => [
        { type: "SimilarJobs", id: arg },
      ],
    }),

    getSuggestedJobs: builder.query<SimilarJobsResponse[], number>({
      query: (limit) => ({
        url: `/jobPostings/suggested?limit=${limit}`,
        method: 'GET',
      }),
    }),

    createJobPosting: builder.mutation<JobPosting, CreateJobPostingRequest>({
      query: (initialPosting) => ({
        url: "/jobPostings",
        method: "POST",
        body: initialPosting,
      }),
      transformResponse: jobPostingResponseTransformer,
      onQueryStarted: async (request, lifecycleApi) => {
        try {
          const { data: createdPosting } =
            await lifecycleApi.queryFulfilled;

          const getPostsPatchResult = lifecycleApi.dispatch(
            jobPostingsApiSlice.util.updateQueryData(
              "getJobsCursor",
              undefined,
              (draft) => {
                // const posts = Object.values(draft.entities).concat(createdPost);
                // postsAdapter.setAll(draft, posts);
                // postsAdapter.addOne(draft, createdPost);

                console.log(draft);
                draft.pages[0].jobPostings.push(createdPosting);
              }
            )
          );

          const getPostPatchResult = lifecycleApi.dispatch(
            jobPostingsApiSlice.util.upsertQueryData(
              "getJobPosting",
              createdPosting.id,
              createdPosting
            )
          );
        } catch {
          console.error("Job Posting creation failed");
        }
      },
    }),

    upvoteJobPosting: builder.mutation<JobPosting, JobPosting>({
      query: (posting) => ({
        url: `/jobPostings/${posting.id}/upvote`,
        method: "PATCH",
      }),
      transformResponse: jobPostingResponseTransformer,
      onQueryStarted: async (posting, lifecycleApi) => {
        let upvotes = posting.upvotes;
        let downvotes = posting.downvotes;
        let isUpvoted = posting.isUpvoted;
        let isDownvoted = posting.isDownvoted;

        // toggle
        if (posting.isUpvoted) {
          upvotes -= 1;
          isUpvoted = false;
        } // change opposites
        else if (posting.isDownvoted) {
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
          jobPostingsApiSlice.util.updateQueryData(
            "getJobsCursor",
            undefined,
            (draft) => {
              // The `draft` is Immer-wrapped and can be 'mutated' like in createSlice
              // const updateCandidate = draft.entities[post.id];
              const postings = transformPagesToFlatPosts(
                draft.pages
              );
              const updateCandidate = postings.find(
                (p) => p.id === posting.id
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
          jobPostingsApiSlice.util.updateQueryData(
            "getJobPosting",
            posting.id,
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

    downvoteJobPosting: builder.mutation<JobPosting, JobPosting>({
      query: (posting) => ({
        url: `/jobPostings/${posting.id}/downvote`,
        method: "PATCH",
      }),
      transformResponse: jobPostingResponseTransformer,
      onQueryStarted: async (posting, lifecycleApi) => {
        let upvotes = posting.upvotes;
        let downvotes = posting.downvotes;
        let isUpvoted = posting.isUpvoted;
        let isDownvoted = posting.isDownvoted;

        // toggle
        if (posting.isDownvoted) {
          downvotes -= 1;
          isDownvoted = false;
        } // change opposites
        else if (posting.isUpvoted) {
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
          jobPostingsApiSlice.util.updateQueryData(
            "getJobsCursor",
            undefined,
            (draft) => {
              // The `draft` is Immer-wrapped and can be 'mutated' like in createSlice
              const postings = transformPagesToFlatPosts(
                draft.pages
              );
              const updateCandidate = postings.find(
                (p) => p.id === posting.id
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
          jobPostingsApiSlice.util.updateQueryData(
            "getJobPosting",
            posting.id,
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

    updateJobPosting: builder.mutation<
      JobPosting,
      { postingId: string; title?: string; content?: string, budget?: number, area?: string }
    >({
      query: ({ postingId, ...updateData }) => ({
        url: `/jobPostings/${postingId}`,
        method: "PATCH",
        body: updateData,
      }),
      onQueryStarted: async (request, lifecycleApi) => {
        const getPostsPatchResult = lifecycleApi.dispatch(
          jobPostingsApiSlice.util.updateQueryData(
            "getJobsCursor",
            undefined,
            (draft) => {
              const updateCandidate = useAppSelector((state) =>
                selectJobPostingById(state, request.postingId)
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
          jobPostingsApiSlice.util.updateQueryData(
            "getJobPosting",
            request.postingId,
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

    deleteJobPosting: builder.mutation<void, string>({
      query: (postingId) => ({
        url: `/jobPostings/${postingId}`,
        method: "DELETE",
      }),
      onQueryStarted: async (postingId, lifecycleApi) => {
        try {
          const response = await lifecycleApi.queryFulfilled;

          const getPostsPatchResult = lifecycleApi.dispatch(
            jobPostingsApiSlice.util.updateQueryData(
              "getJobsCursor",
              undefined,
              (draft) => {
                // postsAdapter.removeOne(draft, postingId);
                const page = draft.pages.find(
                  (p) =>
                    p.jobPostings.findIndex(
                      (posting) => posting.id === postingId
                    ) !== -1
                );
                if (!page) return;

                page.jobPostings = page.jobPostings.filter(
                  (posting) => posting.id !== postingId
                );
              }
            )
          );

          const getPostPatchResult = lifecycleApi.dispatch(
            jobPostingsApiSlice.util.updateQueryData(
              "getJobPosting",
              postingId,
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

    applyToJobPosting: builder.mutation<JobApplicationResponse, ApplyToJobPostingRequest>({
      query: (request) => ({
        url: `/jobPostings/${request.jobPostingId}/apply`,
        method: 'POST',
        body: request,
      }),
    }),

    getJobApplications: builder.query<JobApplicationResponse[], string>({
      query: (postingId) => `/jobPostings/${postingId}/applications`,
    }),
  }),
});

export const {
  useApplyToJobPostingMutation,
  useGetJobApplicationsQuery,
  useGetJobPostingQuery,
  useGetSimilarJobsQuery,
  useGetSuggestedJobsQuery,
  useGetJobsCursorInfiniteQuery,
  useCreateJobPostingMutation,
  useUpvoteJobPostingMutation,
  useDownvoteJobPostingMutation,
  useUpdateJobPostingMutation,
  useDeleteJobPostingMutation,
} = jobPostingsApiSlice;

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
// 	selectIds: selectpostingIds,
// } = postsAdapter.getSelectors(selectPostsData);

// Calling `someEndpoint.select(someArg)` generates a new selector that will return
// the query result object for a query with those parameters.
// To generate a selector for a specific query argument, call `select(theQueryArg)`.
// In this case, the 'Posts' query has no params, so we don't pass anything to select()
export const selectJobPostingsResult =
  jobPostingsApiSlice.endpoints.getJobsCursor.select(undefined);

const selectJobPostingsData = createSelector(
  selectJobPostingsResult,
  // Fall back to the empty entity state if no response yet.
  (result) => result.data?.pages.flat() ?? []
);

export const selectAllJobPostings = createSelector(selectJobPostingsResult, (postsResult) =>
  transformPagesToFlatPosts(postsResult?.data?.pages)
);

export const selectJobPostingById = createSelector(
  selectAllJobPostings,
  (state: RootState, postingId: string) => postingId,
  (postings, postingId) => postings.find((posting) => posting.id === postingId)
);
