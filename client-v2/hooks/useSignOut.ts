"use client";

import { useState, useCallback } from "react";
import { signOut as firebaseSignOut } from "firebase/auth";
import { auth } from "@/lib/firebase";
import { useAppDispatch } from "@/lib/redux/hooks";
import { apiSlice } from "@/features/api/apiSlice";

interface UseSignOutReturn {
	signOut: () => Promise<boolean>;
	loading: boolean;
	error: string | null;
}

/**
 * Hook for signing out users.
 *
 * Handles:
 * - Firebase sign out
 * - RTK Query cache reset (clears all cached data)
 *
 * @example
 * ```tsx
 * const { signOut, loading } = useSignOut();
 *
 * const handleLogout = async () => {
 *   const success = await signOut();
 *   if (success) router.push("/");
 * };
 * ```
 */
export function useSignOut(): UseSignOutReturn {
	const [loading, setLoading] = useState(false);
	const [error, setError] = useState<string | null>(null);
	const dispatch = useAppDispatch();

	const signOut = useCallback(async (): Promise<boolean> => {
		setLoading(true);
		setError(null);

		try {
			// Sign out from Firebase
			await firebaseSignOut(auth);

			// Reset all RTK Query cached data
			dispatch(apiSlice.util.resetApiState());

			return true;
		} catch (err) {
			const message =
				err instanceof Error ? err.message : "Failed to sign out";
			setError(message);
			return false;
		} finally {
			setLoading(false);
		}
	}, [dispatch]);

	return { signOut, loading, error };
}
