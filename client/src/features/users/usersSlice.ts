import { emptyApiSlice } from "../../app/apiSlice";
import { AppUser, CreateUserRequest, UpdateUserRequest } from "./types";

export const usersApiSlice = emptyApiSlice.injectEndpoints({
  endpoints: (builder) => ({
    updateUser: builder.mutation<AppUser, UpdateUserRequest | CreateUserRequest>({
      query: (user) => ({
        url: '/users',
        method: 'PUT',
        body: user,
      }),
      invalidatesTags: ['User'],
    })
  }),
});

export const {
  useUpdateUserMutation,
} = usersApiSlice;
