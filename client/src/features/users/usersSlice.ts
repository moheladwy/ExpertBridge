import { apiSlice } from "../api/apiSlice";
import { AppUser, CreateUserRequest, UpdateUserRequest } from "./types";

export const usersApiSlice = apiSlice.injectEndpoints({
	endpoints: (builder) => ({
		updateUser: builder.mutation<
			AppUser | undefined,
			UpdateUserRequest | CreateUserRequest
		>({
			query: (user) => ({
				url: "/users",
				method: "PUT",
				body: user,
			}),

			invalidatesTags: ["CurrentUser"],
			onQueryStarted: () => {
				console.log("mutation ongoing");
			},
		}),
	}),
});

export const { useUpdateUserMutation } = usersApiSlice;
