import { createApi, fetchBaseQuery } from '@reduxjs/toolkit/query/react';
import config from '@/lib/util/config';
import { auth } from '@/lib/firebase';

export const emptyApiSlice = createApi({
  reducerPath: 'api',
  baseQuery: fetchBaseQuery({
    // baseUrl: config.API_HTTPS_BASE_URL,
    // baseUrl: 'https://api.expertbridge.duckdns.org/api',
    baseUrl: 'http://localhost:3500',

    prepareHeaders: async (headers, query) => {
      const token = await auth.currentUser?.getIdToken();
  
      if (token) {
        headers.set('Authorization', `Bearer ${token}`);
      }
  
      return headers;
    },
  }),

  tagTypes: [
    'CurrentUser',
    'Post',
  ],
  keepUnusedDataFor: 60,

  endpoints: builder => ({

  }),
});
