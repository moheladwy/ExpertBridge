import { AppUser, ProfileResponse } from "@/features/users/types";
import { useGetCurrentUserProfileQuery } from "@/features/users/usersSlice";
import { auth } from "@/lib/firebase";
import useAuthSubscribtion from "@/lib/firebase/useAuthSubscribtion";
import { SerializedError } from "@reduxjs/toolkit";
import { FetchBaseQueryError } from "@reduxjs/toolkit/query";
import { AuthError, User } from "firebase/auth";
import { useEffect, useState } from "react";

export type IsUserLoggedInHook = [
  boolean, // isLoggedIn
  boolean, // loading,
  AuthError | FetchBaseQueryError | SerializedError | undefined,
  User | null | undefined,
  ProfileResponse | null | undefined
];

export default (): IsUserLoggedInHook => {
  const [authUser, authLoading, authError] = useAuthSubscribtion(auth);

  const {
    data: appUser,
    isLoading: userLoading,
    error: userErrorMessage
  } = useGetCurrentUserProfileQuery();

  const [isLoggedIn, setIsLoggedIn] = useState(false);

  useEffect(() => {
    if (authUser && appUser) {
      setIsLoggedIn(true);
    }
    else {
      setIsLoggedIn(false);
    }
  }, [authUser, appUser]);

  return [
    isLoggedIn,
    userLoading || authLoading,
    authError || userErrorMessage,
    authUser,
    appUser,
  ];
}
