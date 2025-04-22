import { apiSlice } from "../api/apiSlice";
import { ProfileResponse } from "./types";

export const profilesApiSlice = apiSlice.injectEndpoints({
	endpoints: (builder) => ({
		// Get current user profile (single profile response)
		getCurrentUserProfile: builder.query<ProfileResponse, void>({
			query: () => `/profiles`,
			transformResponse: (response: ProfileResponse) => {
				return response;
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
	}),
});

export const { useGetCurrentUserProfileQuery, useGetProfileByIdQuery } =
	profilesApiSlice;
