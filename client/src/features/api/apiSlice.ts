import { createApi, fakeBaseQuery, fetchBaseQuery } from '@reduxjs/toolkit/query/react';

export const appApiSlice = createApi({
  reducerPath: 'api',
  baseQuery: fetchBaseQuery({
    baseUrl: 'PUT_OUR_URL_HERE_FROM_CONFIG',
  }),

  tagTypes: ['Login'],
  keepUnusedDataFor: 60,

  endpoints: builder => ({

  }),
});
