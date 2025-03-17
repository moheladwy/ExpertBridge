import { BaseQueryFn, createApi, FetchArgs, fetchBaseQuery, FetchBaseQueryArgs, FetchBaseQueryError } from '@reduxjs/toolkit/query/react';
import config from '@/lib/util/config';
import { auth } from '@/lib/firebase';



export const authorizedBaseQuery: BaseQueryFn<
  string | FetchArgs,
  unknown,
  FetchBaseQueryError
> = async (args, api, extraOptions) => {
  const token = await auth.currentUser?.getIdToken();

  const baseQuery = fetchBaseQuery({
    // baseUrl: config.API_HTTPS_BASE_URL, 
    // baseUrl: 'https://api.expertbridge.duckdns.org/api', // deployed api
    // baseUrl: 'http://localhost:3500', // json-server
    baseUrl: 'http://localhost:5027/api', // local api

    prepareHeaders: (headers) => {
      if (token) {
        headers.set('Authorization', `Bearer ${token}`);
      }

      return headers;
    }
  });

  console.log(baseQuery.toString());

  return await baseQuery(args, api, extraOptions);
};

export const emptyApiSlice = createApi({
  reducerPath: 'api',
  baseQuery: fetchBaseQuery({
    // baseUrl: config.API_HTTPS_BASE_URL, 
    // baseUrl: 'https://api.expertbridge.duckdns.org/api', // deployed api
    // baseUrl: 'http://localhost:3500', // json-server
    baseUrl: 'http://localhost:5027/api', // local api

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
