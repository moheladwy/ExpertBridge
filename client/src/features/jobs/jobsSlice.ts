import { apiSlice } from "../api/apiSlice";
import { CreateJobOfferRequest, JobOfferResponse, JobResponse } from "./types";

export const jobsApiSlice = apiSlice.injectEndpoints({
  endpoints: (builder) => ({

    getMyJobs: builder.query<JobResponse[], void>({
      query: () => '/jobs',
      providesTags: ['Job'],
    }),

    getJobById: builder.query<JobResponse, string>({
      query: (jobId) => `/jobs/${jobId}`,
      providesTags: ['Job'],
    }),

    // Get my job offers (offers I've sent)
    getMyJobOffers: builder.query<JobOfferResponse[], void>({
      query: () => '/jobs/offers',
      providesTags: ['JobOffer'],
    }),

    // Create a new job offer - This should be a mutation, not a query
    createJobOffer: builder.mutation<JobOfferResponse, CreateJobOfferRequest>({
      query: (request) => ({
        url: `/jobs/offers`,
        method: 'POST',
        body: request,
      }),
      invalidatesTags: ['JobOffer'],
    }),

    // Get job offers received by the current user
    getReceivedJobOffers: builder.query<JobOfferResponse[], void>({
      query: () => '/jobs/offers/received',
      providesTags: ['JobOffer'],
    }),

    // Get a specific job offer by ID
    getJobOfferById: builder.query<JobOfferResponse, string>({
      query: (id) => `/jobs/offers/${id}`,
      providesTags: (result, error, id) => [{ type: 'JobOffer', id }],
    }),

    // Update job offer status (accept/decline)
    updateJobOfferStatus: builder.mutation<JobOfferResponse, { id: string; status: 'accepted' | 'declined' }>({
      query: ({ id, status }) => ({
        url: `/jobs/offers/${id}/status`,
        method: 'PATCH',
        body: { status },
      }),
      invalidatesTags: (result, error, { id }) => [{ type: 'JobOffer', id }],
    }),

    // Delete a job offer
    deleteJobOffer: builder.mutation<void, string>({
      query: (id) => ({
        url: `/jobs/offers/${id}`,
        method: 'DELETE',
      }),
      invalidatesTags: (result, error, id) => [{ type: 'JobOffer', id }],
    }),
  }),
});

export const {
  useGetMyJobsQuery,
  useGetJobByIdQuery,
  useCreateJobOfferMutation,
  useGetMyJobOffersQuery,
  useGetReceivedJobOffersQuery,
  useGetJobOfferByIdQuery,
  useUpdateJobOfferStatusMutation,
  useDeleteJobOfferMutation,
} = jobsApiSlice;
