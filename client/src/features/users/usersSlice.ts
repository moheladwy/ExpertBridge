import { emptyApiSlice } from "../api/apiSlice";
import { AppUser, CreateUserRequest, ProfileResponse, UpdateUserRequest } from "./types";

export const usersApiSlice = emptyApiSlice.injectEndpoints({
  endpoints: (builder) => ({

    // TODO: Consider getting this to it's own slice (profilesSlice) maybe???
    getCurrentUserProfile: builder.query<ProfileResponse, void>({
      query: () => `/profiles`,
      transformResponse: (response: ProfileResponse) => {
        console.log(response);
        return response;
      },
      providesTags: ['CurrentUser'],
    }),

    updateUser: builder.mutation<AppUser, UpdateUserRequest | CreateUserRequest>({
      query: (user) => ({
        url: '/users',
        method: 'PUT',
        body: user,
      }),
      invalidatesTags: ['CurrentUser'],
      onQueryStarted: () => {
        console.log('mutation ongoing');
      },
    }),
  }),
});

export const {
  useGetCurrentUserProfileQuery,
  useUpdateUserMutation,
} = usersApiSlice;
