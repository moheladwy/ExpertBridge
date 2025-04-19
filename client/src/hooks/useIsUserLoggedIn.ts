import { useAppSelector } from "@/app/hooks";
import { selectAuthUser } from "@/features/auth/authSlice";
import { AppUser, ProfileResponse, UpdateUserRequest } from "@/features/users/types";
import { useGetCurrentUserProfileQuery, useUpdateUserMutation } from "@/features/users/usersSlice";
import { auth } from "@/lib/firebase";
import useAuthSubscribtion from "@/lib/firebase/useAuthSubscribtion";
import { SerializedError } from "@reduxjs/toolkit";
import { FetchBaseQueryError } from "@reduxjs/toolkit/query";
import { AuthError, User } from "firebase/auth";
import { useCallback, useEffect, useMemo, useState } from "react";

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
  const uid = useMemo(() => authUser?.uid, [authUser?.uid]);

  const {
    data: appUser,
    isLoading: userLoading,
    error: userErrorMessage,
    isError: userError,
    refetch: retryQuery,
  } = useGetCurrentUserProfileQuery();

  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<IsLoggedInError>(undefined);

  // If a query like this is used (fired) multiple times
  // and it's result is checked always. Always remember to 
  // retry so that the error state changes after the next try.
  // This was one of the most interesting bugs I had to debug. 
  useEffect(() => {
    if (uid === authUser?.uid) return;
    retryQuery();
  }, [uid, retryQuery, authUser]);

  // We are checking against the uid becuase firebase sdk
  // returns a new reference each time we ask it for the currentUser.
  // And react hooks check agains reference equality, not deep equality.
  // Thus, we check if the user has changed by checking agains the uid instead.

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
