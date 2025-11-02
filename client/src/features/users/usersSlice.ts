import { apiSlice } from "../api/apiSlice";
import { AppUser, UpdateUserRequest } from "./types";

export const usersApiSlice = apiSlice.injectEndpoints({
	endpoints: (builder) => ({
		updateUser: builder.mutation<AppUser | undefined, UpdateUserRequest>({
			query: (user) => ({
				url: "/users",
				method: "PUT",
				body: user,
				headers: {
					Authorization: `Bearer ${user.token}`,
				},
			}),

			invalidatesTags: ["CurrentUser"],
			onQueryStarted: async (request, lifecycleApi) => {
				console.log("mutation ongoing");
			},
		}),
	}),
});

export const { useUpdateUserMutation } = usersApiSlice;
