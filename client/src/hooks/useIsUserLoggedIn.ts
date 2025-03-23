import { AppUser, ProfileResponse } from "@/features/users/types";
import { useGetCurrentUserProfileQuery } from "@/features/users/usersSlice";
import { auth } from "@/lib/firebase";
import useAuthSubscribtion from "@/lib/firebase/useAuthSubscribtion";
import { SerializedError } from "@reduxjs/toolkit";
import { FetchBaseQueryError } from "@reduxjs/toolkit/query";
import { AuthError, User } from "firebase/auth";
import { useEffect, useState } from "react";

export type IsLoggedInError = AuthError | FetchBaseQueryError | SerializedError | undefined;

export type IsUserLoggedInHook = [
  boolean, // isLoggedIn
  boolean, // loading,
  IsLoggedInError,
  User | null | undefined,
  ProfileResponse | null | undefined
];

export default (): IsUserLoggedInHook => {
  const [authUser, authLoading, authError] = useAuthSubscribtion(auth);

  const {
    data: appUser,
    isLoading: userLoading,
    error: userErrorMessage,
    isError: userError,
    refetch: retryQuery
  } = useGetCurrentUserProfileQuery();

  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<IsLoggedInError>(undefined);

  useEffect(() => {
    retryQuery();
  }, [authUser, retryQuery]);

  useEffect(() => {
    if (authError || userError) {
      console.error(authError || userErrorMessage);
      setError(authError || userErrorMessage);
      setLoading(false);
    }
  }, [authError, userErrorMessage, userError]);

  useEffect(() => {
    if (authUser && appUser) {
      setIsLoggedIn(true);
      setLoading(false);
    }
    else {
      setIsLoggedIn(false);
      setLoading(false);
    }
  }, [authUser, appUser]);

  useEffect(() => {
    setLoading(userLoading || authLoading);
  }, [userLoading, authLoading]);

  return [
    isLoggedIn,
    loading,
    error,
    authUser,
    appUser,
  ];
}
