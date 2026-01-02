import { apiSlice } from "../api/apiSlice";
import {
	CreateOrUpdateUserRequest,
	OnboardUserRequest,
	ProfileResponse,
	UpdateProfileRequest,
} from "./types";

/**
 * Profiles API slice for RTK Query.
 *
 * Handles all profile-related API calls including:
 * - Current user profile (used for auth state)
 * - Other user profiles
 * - Profile updates and onboarding
 */
export const profilesApiSlice = apiSlice.injectEndpoints({
	endpoints: (builder) => ({
		/**
		 * Get the current authenticated user's profile.
		 * This is the primary source of "app profile" data.
		 */
		getCurrentUserProfile: builder.query<ProfileResponse, void>({
			query: () => `/profiles`,
			providesTags: ["Profile"],
		}),

		/**
		 * Get a profile by user ID.
		 */
		getProfileById: builder.query<ProfileResponse, string>({
			query: (profileId) => `/profiles/${profileId}`,
			providesTags: (_result, _error, id) => [{ type: "Profile", id }],
		}),

		/**
		 * Complete user onboarding with selected tags.
		 */
		onboardUser: builder.mutation<ProfileResponse, OnboardUserRequest>({
			query: (request) => ({
				url: "/profiles/onboard",
				method: "POST",
				body: request,
			}),
			invalidatesTags: ["Profile"],
			// Optimistic update
			onQueryStarted: async (_request, { dispatch, queryFulfilled }) => {
				const patchResult = dispatch(
					profilesApiSlice.util.updateQueryData(
						"getCurrentUserProfile",
						undefined,
						(draft) => {
							draft.isOnboarded = true;
						}
					)
				);

				try {
					await queryFulfilled;
				} catch {
					patchResult.undo();
				}
			},
		}),

		/**
		 * Check if a username is available.
		 */
		isUsernameAvailable: builder.query<boolean, string>({
			query: (username) => `/profiles/is-username-available/${username}`,
		}),

		/**
		 * Update the current user's profile.
		 */
		updateProfile: builder.mutation<ProfileResponse, UpdateProfileRequest>({
			query: (request) => ({
				url: `/profiles`,
				method: "PUT",
				body: request,
			}),
			invalidatesTags: ["Profile"],
			// Optimistic update
			onQueryStarted: async (request, { dispatch, queryFulfilled }) => {
				const patchResult = dispatch(
					profilesApiSlice.util.updateQueryData(
						"getCurrentUserProfile",
						undefined,
						(draft) => {
							Object.assign(draft, request);
						}
					)
				);

				try {
					await queryFulfilled;
				} catch {
					patchResult.undo();
				}
			},
		}),

		/**
		 * Get the current user's skills.
		 */
		getCurrentUserSkills: builder.query<string[], void>({
			query: () => `/profiles/skills`,
			providesTags: [{ type: "Profile", id: "skills" }],
		}),

		/**
		 * Get skills for a specific profile.
		 */
		getProfileSkills: builder.query<string[], string>({
			query: (id) => `/profiles/${id}/skills`,
			providesTags: (_result, _error, id) => [
				{ type: "Profile", id: `${id}-skills` },
			],
		}),

		/**
		 * Get suggested experts based on current user's interests.
		 */
		getSuggestedExperts: builder.query<ProfileResponse[], number>({
			query: (limit) => `/profiles/suggested?limit=${limit}`,
		}),

		/**
		 * Get profiles with highest reputation.
		 */
		getTopReputationProfiles: builder.query<ProfileResponse[], number>({
			query: (limit) => `/profiles/top-reputation?limit=${limit}`,
		}),

		/**
		 * Create or update a user in the backend.
		 * Used during registration and Google OAuth (upsert pattern).
		 */
		createOrUpdateUser: builder.mutation<
			ProfileResponse,
			CreateOrUpdateUserRequest
		>({
			query: (request) => ({
				url: "/users",
				method: "POST",
				body: request,
			}),
			invalidatesTags: ["Profile"],
		}),
	}),
});

export const {
	useGetCurrentUserProfileQuery,
	useGetProfileByIdQuery,
	useOnboardUserMutation,
	useIsUsernameAvailableQuery,
	useUpdateProfileMutation,
	useGetCurrentUserSkillsQuery,
	useGetProfileSkillsQuery,
	useGetSuggestedExpertsQuery,
	useGetTopReputationProfilesQuery,
	useCreateOrUpdateUserMutation,
} = profilesApiSlice;
