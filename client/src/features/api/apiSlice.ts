import { BaseQueryFn, createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';
import config from '@/lib/util/config';
import { auth } from '@/lib/firebase';


export const emptyApiSlice = createApi({
  reducerPath: 'api',
  baseQuery: fetchBaseQuery({
    // baseUrl: config.API_HTTPS_BASE_URL,
    // baseUrl: 'https://api.expertbridge.duckdns.org/api', // deployed api
    // baseUrl: 'http://localhost:3500', // json-server
    // baseUrl: 'http://69.62.106.202:8080/api', // local api
    baseUrl: 'http://localhost:5027/api',

    prepareHeaders: async (headers) => {
      const token = await auth.currentUser?.getIdToken();
      if (token) {
        headers.set('Authorization', `Bearer ${token}`);
      }

      return headers;
    }
  }),

  tagTypes: [
    'CurrentUser',
    'Post',
  ],
  keepUnusedDataFor: 60,

  endpoints: builder => ({

  }),
});
