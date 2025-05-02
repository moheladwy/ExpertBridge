import { apiSlice } from "../api/apiSlice";
import { OnboardUserRequest, ProfileResponse } from "./types";

export const profilesApiSlice = apiSlice.injectEndpoints({
	endpoints: (builder) => ({
		// Get current user profile (single profile response)
		getCurrentUserProfile: builder.query<ProfileResponse, void>({
			query: () => `/profiles`,
			transformResponse: (response: ProfileResponse) => {
				return response;
			},
			onQueryStarted: () => {
				console.log('fetching user profile...');
			},
			providesTags: ["CurrentUser"],
		}),

		// Get profile by ID (single profile response)
		getProfileById: builder.query<ProfileResponse, string>({
			query: (profileId) => `/profiles/${profileId}`,
			transformResponse: (response: ProfileResponse) => {
				return response;
			},
			providesTags: (result, error, arg) => [
				{ type: "Profile", id: arg },
			],
		}),

		onboardUser: builder.mutation<ProfileResponse, OnboardUserRequest>({
			query: (request) => ({
				url: '/profiles/onboard',
				method: 'POST',
				body: request
			}),
			onQueryStarted: async (request, lifecycleApi) => {
				const patchResult = lifecycleApi.dispatch(
					profilesApiSlice.util.updateQueryData(
						'getCurrentUserProfile',
						undefined,
						(draft) => {
							draft.isOnboarded = true;
						}
					),
				);

				try {
					await lifecycleApi.queryFulfilled;
				} catch {
					patchResult.undo();
				}
			},
		}),

	}),
});

export const {
	useGetCurrentUserProfileQuery,
	useGetProfileByIdQuery,
	useOnboardUserMutation,
} = profilesApiSlice;
