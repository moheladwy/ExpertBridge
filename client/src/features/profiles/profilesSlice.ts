import { apiSlice } from "../api/apiSlice";
import { userLoggedIn } from "../auth/authSlice";
import {
  OnboardUserRequest,
  ProfileResponse,
  UpdateProfileRequest,
} from "./types";

export const profilesApiSlice = apiSlice.injectEndpoints({
  endpoints: (builder) => ({
    // Get current user profile (single profile response)
    getCurrentUserProfile: builder.query<ProfileResponse, void>({
      query: () => `/profiles`,
      providesTags: ["CurrentUser"],
      transformResponse: (response: ProfileResponse) => {
        return response;
      },
      onQueryStarted: () => {
        console.log("fetching user profile...");
      },
      onCacheEntryAdded: async (arg, lifecycleApi) => {
        const { data: user } = await lifecycleApi.cacheDataLoaded;

        lifecycleApi.dispatch(userLoggedIn({ currentUser: user }));
      },
    }),

    // Get profile by ID (single profile response)
    getProfileById: builder.query<ProfileResponse, string>({
      query: (profileId) => `/profiles/${profileId}`,
      transformResponse: (response: ProfileResponse) => {
        return response;
      },
      providesTags: (result, error, arg) => [{ type: "Profile", id: arg }],
    }),

    onboardUser: builder.mutation<ProfileResponse, OnboardUserRequest>({
      query: (request) => ({
        url: "/profiles/onboard",
        method: "POST",
        body: request,
      }),
      invalidatesTags: ["CurrentUser", { type: "Profile", id: "LIST" }],
      onQueryStarted: async (request, lifecycleApi) => {
        const patchResult = lifecycleApi.dispatch(
          profilesApiSlice.util.updateQueryData(
            "getCurrentUserProfile",
            undefined,
            (draft) => {
              draft.isOnboarded = true;
            },
          ),
        );

        try {
          await lifecycleApi.queryFulfilled;
        } catch {
          patchResult.undo();
        }
      },
    }),

    isUsernameAvailable: builder.mutation<boolean, string>({
      query: (username) => ({
        url: `/profiles/is-username-available/${username}`,
        method: "GET",
      }),
    }),

    updateProfile: builder.mutation<ProfileResponse, UpdateProfileRequest>({
      query: (request) => ({
        url: `/profiles`,
        method: "PUT",
        body: request,
      }),
      onQueryStarted: async (request, lifecycleApi) => {
        const patchResult = lifecycleApi.dispatch(
          profilesApiSlice.util.updateQueryData(
            "getCurrentUserProfile",
            undefined,
            (draft: ProfileResponse) => {
              Object.assign(draft, request);
            },
          ),
        );

        try {
          await lifecycleApi.queryFulfilled;
        } catch {
          patchResult.undo();
        }
      },
    }),

    getCurrentUserSkills: builder.query<string[], void>({
      query: () => `/profiles/skills`,
      providesTags: ["CurrentUserSkills"],
    }),

    getProfileSkills: builder.query<string[], string>({
      query: (id) => `/profiles/${id}/skills`,
      providesTags: (result, error, arg) => [
        { type: "ProfileSkills", id: arg },
      ],
    }),

    getSuggestedExperts: builder.query<ProfileResponse[], number>({
      query: (limit) => `/profiles/suggested?limit=${limit}`,
    }),

    getTopReputationProfiles: builder.query<ProfileResponse[], number>({
      query: (limit) => `/profiles/top-reputation?limit=${limit}`,
    }),
  }),
});

export const {
  useGetCurrentUserProfileQuery,
  useGetProfileByIdQuery,
  useOnboardUserMutation,
  useIsUsernameAvailableMutation,
  useUpdateProfileMutation,
  useGetCurrentUserSkillsQuery,
  useGetProfileSkillsQuery,
  useGetSuggestedExpertsQuery,
  useGetTopReputationProfilesQuery,
} = profilesApiSlice;
