import { emptyApiSlice } from "../api/apiSlice";
import { AppUser, CreateUserRequest, UpdateUserRequest } from "./types";

export const usersApiSlice = emptyApiSlice.injectEndpoints({
  endpoints: (builder) => ({

    getCurrentUser: builder.query<AppUser, string | null | undefined>({
      query: (email) => `/user/get-by-email/${email}`,
      transformResponse: (response: AppUser) => {
        console.log(response);
        return response;
      },
      providesTags: ['CurrentUser'],
    }),

    updateUser: builder.mutation<AppUser, UpdateUserRequest | CreateUserRequest>({
      query: (user) => ({
        url: '/user/update',
        method: 'PUT',
        body: user,
      }),
      invalidatesTags: ['CurrentUser'],
    }),
  }),
});

export const {
  useGetCurrentUserQuery,
  useUpdateUserMutation,
} = usersApiSlice;
