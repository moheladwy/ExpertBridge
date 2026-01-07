"use client";

import { useEffect, useRef } from "react";
import { useIsUserLoggedIn } from "./useIsUserLoggedIn";

/**
 * Hook that triggers a refetch when the user logs in or out.
 * Useful for refreshing data that depends on authentication state.
 *
 * This hook monitors the user's profile ID and triggers a refetch
 * when it changes (e.g., user logs in, logs out, or switches accounts).
 *
 * @param refetch - Function to call when auth state changes
 *
 * @example
 * ```tsx
 * function Feed() {
 *   const { data, refetch } = useGetPostsQuery();
 *   useRefetchOnLogin(refetch);
 *   // Data will be refetched when user logs in/out
 * }
 * ```
 */
export default function useRefetchOnLogin(refetch: () => void) {
	const { profile, isAuthenticated, firebaseUser } = useIsUserLoggedIn();

	const lastProfileIdRef = useRef<string | undefined>(profile?.id);

	useEffect(() => {
		const currentProfileId = profile?.id;

		// Skip if the ID hasn't changed
		if (lastProfileIdRef.current === currentProfileId) return;

		// Update the ref with the new ID
		lastProfileIdRef.current = currentProfileId;

		// Trigger refetch when auth state changes
		refetch();
	}, [profile, isAuthenticated, firebaseUser, refetch]);
}
