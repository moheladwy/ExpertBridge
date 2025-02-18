import { createApi, fakeBaseQuery } from '@reduxjs/toolkit/query/react';

export const firebaseApiSlice = createApi({
  reducerPath: 'firebaseApi',
  baseQuery: fakeBaseQuery(),

  tagTypes: ['Login'],
  keepUnusedDataFor: 5,

  endpoints: builder => ({

  }),
});
