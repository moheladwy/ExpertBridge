import { emptyApiSlice } from "../api/apiSlice";
import { FetchBaseQueryError } from "@reduxjs/toolkit/query";
import { LoginRequest, LoginResponse, RegisterRequest as SignUpRequest, RegisterResponse as SignUpResponse } from "./types";
import { createUserWithEmailAndPassword } from "firebase/auth";
import { auth } from "@/lib/firebase";


export const authApiSlice = emptyApiSlice.injectEndpoints({
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

    signUp: builder.mutation<SignUpResponse, SignUpRequest>({
      queryFn: async ({ email, password }) => {
        try {
          const response = await createUserWithEmailAndPassword(auth, email, password);
          console.log('Res: ', response);
          // const token = await response.user.getIdToken();

          const data = { user: response.user };

          // const data: LoginResponse = await response.json();
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
  useSignUpMutation,
} = authApiSlice;
