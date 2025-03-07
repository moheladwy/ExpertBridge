import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';

export const emptyApiSlice = createApi({
  reducerPath: 'api',
  baseQuery: fetchBaseQuery({
    baseUrl: import.meta.env.BASE_URL,
  }),

  tagTypes: [
    'Login',
    'User',
  ],
  keepUnusedDataFor: 60,

  // @ts-expect-error - We don't need to define endpoints here
  endpoints: builder => ({

  }),
});
