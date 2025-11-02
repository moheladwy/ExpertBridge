import { useAppSelector } from "@/app/hooks";
import { useGetCurrentUserProfileQuery } from "@/features/profiles/profilesSlice";
import { ProfileResponse } from "@/features/profiles/types";
import { auth } from "@/lib/firebase";
import { SerializedError } from "@reduxjs/toolkit";
import { FetchBaseQueryError } from "@reduxjs/toolkit/query";
import { AuthError, User } from "firebase/auth";
import { useEffect, useMemo, useState } from "react";
import { useCurrentAuthUser } from "./useCurrentAuthUser";
import useSignOut from "@/lib/firebase/useSignOut";

export type IsLoggedInError =
	| AuthError
	| FetchBaseQueryError
	| SerializedError
	| undefined;

export type IsUserLoggedInHook = [
	boolean, // isLoggedIn
	boolean, // loading,
	IsLoggedInError,
	User | null | undefined,
	ProfileResponse | null | undefined,
];

const useIsUserLoggedIn = (): IsUserLoggedInHook => {
	const { currentUser } = useAppSelector((state) => state.auth);

	const authUser = useCurrentAuthUser();
	const [signOut] = useSignOut(auth);

	const uid = useMemo(() => authUser?.uid, [authUser]);

	const {
		data: appUser,
		isFetching: userLoading,
		error: userErrorMessage,
		isError: userError,
		refetch: retryQuery,
	} = useGetCurrentUserProfileQuery();

	const [isLoggedIn, setIsLoggedIn] = useState(appUser ? true : false);
	const [loading, setLoading] = useState(userLoading);
	const [error, setError] = useState<IsLoggedInError>(undefined);

	// If a query like this is used (fired) multiple times
	// and it's result is checked always. Always remember to
	// retry so that the error state changes after the next try.
	// This was one of the most interesting bugs I had to debug.
	useEffect(() => {
		if (authUser) {
			if (!authUser.emailVerified) {
				signOut();
			}
		}
	}, [retryQuery, authUser, signOut, uid]);

	// We are checking against the uid becuase firebase sdk
	// returns a new reference each time we ask it for the currentUser.
	// And react hooks check agains reference equality, not deep equality.
	// Thus, we check if the user has changed by checking agains the uid instead.

	useEffect(() => {
		if (userError) {
			console.error(userErrorMessage);
			setError(userErrorMessage);
			setLoading(false);
		}
	}, [userError, userErrorMessage, error]);

	useEffect(() => {
		if (authUser && appUser) {
			setIsLoggedIn(true);
			setLoading(false);
		} else {
			setIsLoggedIn(false);
			setLoading(false);
		}
	}, [authUser, appUser]);

	useEffect(() => {
		setLoading(userLoading);
	}, [userLoading]);

	return [isLoggedIn, userLoading, error, authUser, appUser];
};

export default useIsUserLoggedIn;
