import { createSlice } from "@reduxjs/toolkit";
import { User } from "firebase/auth";

// export type AuthUser = User;
export interface AuthUser {
  uid: string;
}

const initialState: { authUser: AuthUser | undefined | null } = { authUser: null };

export const authSlice = createSlice({
  name: "auth",
  initialState,
  reducers: {
    saveAuthUser: (state, action: { payload: AuthUser | undefined | null }) => {
      state.authUser = action.payload;
    },
  },
  selectors: {
    selectAuthUser: (state) => state.authUser,
  }
});

// Action creators are generated for each case reducer function
export const { saveAuthUser } = authSlice.actions;

export const { selectAuthUser } = authSlice.selectors;

export default authSlice.reducer;
