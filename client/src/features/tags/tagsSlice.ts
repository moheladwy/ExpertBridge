import { apiSlice } from "../api/apiSlice";
import { Tag } from "./types";

export const tagsApiSlice = apiSlice.injectEndpoints({
	endpoints: (builder) => ({
		getTags: builder.query<Tag[], void>({
			query: () => "/tags",
			providesTags: ["Tag", { type: "Tag", id: "LIST" }],
		}),
	}),
});

export const { useGetTagsQuery } = tagsApiSlice;
