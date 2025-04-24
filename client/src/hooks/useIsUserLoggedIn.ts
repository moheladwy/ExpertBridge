import { useAppSelector } from "@/app/hooks";
import { selectAuthUser } from "@/features/auth/authSlice";
import { useGetCurrentUserProfileQuery } from "@/features/profiles/profilesSlice";
import { ProfileResponse } from "@/features/profiles/types";
import { AppUser, UpdateUserRequest } from "@/features/users/types";
import { useUpdateUserMutation } from "@/features/users/usersSlice";
import { auth } from "@/lib/firebase";
import useAuthSubscribtion from "@/lib/firebase/useAuthSubscribtion";
import { SerializedError } from "@reduxjs/toolkit";
import { FetchBaseQueryError } from "@reduxjs/toolkit/query";
import { AuthError, User } from "firebase/auth";
import { useCallback, useEffect, useMemo, useState } from "react";
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
	// const [authUser, authLoading, authError] = useAuthSubscribtion(auth);
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

	const [isLoggedIn, setIsLoggedIn] = useState(false);
	const [loading, setLoading] = useState(false);
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

		if (uid === authUser?.uid) return;

		retryQuery();
		console.log('refetching get current profile...');
	}, [retryQuery, authUser, signOut, uid]);

	// We are checking against the uid becuase firebase sdk
	// returns a new reference each time we ask it for the currentUser.
	// And react hooks check agains reference equality, not deep equality.
	// Thus, we check if the user has changed by checking agains the uid instead.



	// useEffect(() => {
	// 	console.log('---------------------START STATE REPORT---------------------');
	// 	console.log('authUser:', authUser);
	// 	console.log('authUser.uid:', authUser?.uid);
	// 	console.log('authUser.emailVerified:', authUser?.emailVerified);
	// 	console.log('appUser:', appUser);
	// 	console.log('appUser.id:', appUser?.id);
	// 	console.log('isLoggedIn:', isLoggedIn);
	// 	console.log('loading:', loading);
	// 	console.log('error:', error);
	// 	console.log('userLoading:', userLoading);
	// 	console.log('userError:', userError);
	// 	console.log('userErrorMessage:', userErrorMessage);
	// 	console.log('Timestamp:', new Date().toISOString());
	// 	console.log('---------------------END STATE REPORT---------------------');
	// }, [authUser, appUser, isLoggedIn, loading, error, userLoading, userError, userErrorMessage]);



	// useEffect(() => {
	// 	if (authError) {
	// 		console.error(authError);
	// 		setError(authError);
	// 		setLoading(false);
	// 	}
	// }, [authError]);

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

	// useEffect(() => {
	// 	setLoading(userLoading || authLoading);
	// }, [userLoading, authLoading]);
	useEffect(() => {
		setLoading(userLoading);
	}, [userLoading]);

	return [isLoggedIn, loading, error, authUser, appUser];
};

export default useIsUserLoggedIn;
