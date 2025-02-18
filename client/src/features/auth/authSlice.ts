import { appApiSlice } from "../api/apiSlice";
import { FetchBaseQueryError } from "@reduxjs/toolkit/query";
import { LoginRequest, LoginResponse, RegisterRequest, RegisterResponse } from "./types";


export const authApiSlice = appApiSlice.injectEndpoints({
  endpoints: (builder) => ({
    login: builder.mutation<LoginResponse, LoginRequest>({
      queryFn: async ({ username, password }) => {
        try {
          // Call the sdk login instead here. 
          // Or whatever hybrid solution we are using.
          const response = await fetch("/api/auth/login", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ username, password }),
          });

          if (!response.ok) {
            return { error: { status: response.status, data: await response.json() } as FetchBaseQueryError };
          }

          const data: LoginResponse = await response.json();
          return { data };
        } catch (error) {
          return { error: { status: "CUSTOM_ERROR", error: (error as Error).message } };
        }
      },
    }), 

    register: builder.mutation<RegisterResponse, RegisterRequest>({
      queryFn: async ({ }) => {
        try {
          // Call the sdk login instead here. 
          // Or whatever hybrid solution we are using.
          const response = await fetch("/api/auth/login", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ }),
          });

          if (!response.ok) {
            return { error: { status: response.status, data: await response.json() } as FetchBaseQueryError };
          }

          const data: LoginResponse = await response.json();
          return { data };
        } catch (error) {
          return { error: { status: "CUSTOM_ERROR", error: (error as Error).message } };
        }
      }
    })
  }),
});

export const {
  useLoginMutation,
  useRegisterMutation,
} = authApiSlice;
