import { useGetCurrentUserProfileQuery } from "@/features/profiles/profilesSlice";
import { ProfileResponse } from "@/features/profiles/types";
import { auth } from "@/lib/firebase";
import { SerializedError } from "@reduxjs/toolkit";
import { FetchBaseQueryError } from "@reduxjs/toolkit/query";
import { AuthError, User } from "firebase/auth";
import { useEffect, useMemo, useState } from "react";
import { useCurrentAuthUser } from "./useCurrentAuthUser";
import useSignOut from "@/lib/firebase/useSignOut";
import { useAuthReady } from "./useAuthReady";

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
	const authUser = useCurrentAuthUser();
	const [signOut] = useSignOut(auth);

	// ✅ NEW: Get auth ready state
	const { isAuthReady, isAuthenticated } = useAuthReady();

	const uid = useMemo(() => authUser?.uid, [authUser]);

	const [error, setError] = useState<IsLoggedInError>(undefined);

	// ✅ UPDATED: Add skip parameter to prevent premature API calls
	const {
		data: appUser,
		isFetching: userLoading,
		error: userErrorMessage,
		isError: userError,
		refetch: retryQuery,
	} = useGetCurrentUserProfileQuery(undefined, {
		skip: !isAuthenticated, // ✅ Only fetch if authenticated
	});



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
			// Use a microtask to avoid synchronous setState in effect
			Promise.resolve().then(() => {
				setError(userErrorMessage);
			});
		}
	}, [userError, userErrorMessage]);

	// Derive isLoggedIn directly instead of using state
	const isLoggedIn = !!(authUser && appUser);

	// Derive loading state directly
	const loading = userError ? false : (!isAuthReady || userLoading);

	return [isLoggedIn, loading, error, authUser, appUser];
};

export default useIsUserLoggedIn;
