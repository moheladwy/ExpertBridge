import { emptyApiSlice } from "../api/apiSlice";
import { Post } from "./types";

export const postsApiSlice = emptyApiSlice.injectEndpoints({
  endpoints: (builder) => ({

    getPost: builder.query<Post, string>({
      query: () => '',
      providesTags: [],
    }),
  }),
});

export const {
  useGetPostQuery,
} = postsApiSlice;

