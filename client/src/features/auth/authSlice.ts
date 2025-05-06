import { createSlice } from "@reduxjs/toolkit";
import { User } from "firebase/auth";
import { ProfileResponse } from "../profiles/types";

// export type AuthUser = User;

const initialState: { currentUser: ProfileResponse | undefined | null } = { currentUser: null };

export const authSlice = createSlice({
  name: "auth",
  initialState,
  reducers: {
    userLoggedIn: (state, action) => {
      state.currentUser = action.payload.user;
      // state.token = action.payload.token;

      // localStorage.setItem("token", action.payload.token);
      localStorage.setItem("user", JSON.stringify(action.payload.user));
    },

    // update the store
    updateUser: (state, action) => {
      state.currentUser = action.payload;
      localStorage.setItem("user", JSON.stringify(action.payload));
    },
    userLoggedOut: (state) => {
      state.currentUser = null;
      localStorage.removeItem("token");
      localStorage.removeItem("user");
    },
  },
});

export const { userLoggedIn, userLoggedOut, updateUser } = authSlice.actions;

export default authSlice.reducer;
