import { useCurrentAuthUser } from "@/hooks/useCurrentAuthUser";
import { type User } from "firebase/auth";
import { useState, useEffect } from "react";

/**
 * Interface for auth ready state
 */
interface AuthReadyState {
	/**
	 * Indicates whether Firebase auth state has been initialized
	 * True once Firebase has determined if a user is logged in or not
	 */
	isAuthReady: boolean;

	/**
	 * Indicates whether a user is currently authenticated
	 * True if a Firebase user exists, false otherwise
	 */
	isAuthenticated: boolean;

	/**
	 * The Firebase user object if authenticated, null otherwise
	 */
	authUser: User | null;
}

/**
 * Hook to check Firebase authentication readiness and state
 *
 * This hook provides a centralized way to:
 * 1. Check if Firebase auth has initialized (isAuthReady)
 * 2. Check if a user is authenticated (isAuthenticated)
 * 3. Access the authenticated user object (authUser)
 *
 * Use this hook to conditionally execute queries or show loading states
 * while waiting for Firebase to determine authentication state.
 *
 * @example
 * ```typescript
 * const { isAuthReady, isAuthenticated, authUser } = useAuthReady();
 *
 * // Don't make API calls until auth is ready
 * const { data } = useGetProfileQuery(undefined, {
 *   skip: !isAuthenticated
 * });
 *
 * // Show loading while auth initializes
 * if (!isAuthReady) {
 *   return <PageLoader />;
 * }
 * ```
 *
 * @returns AuthReadyState object with auth status flags
 */
export const useAuthReady = (): AuthReadyState => {
	const authUser = useCurrentAuthUser();
	const [isAuthReady, setIsAuthReady] = useState(false);

	useEffect(() => {
		// Firebase auth takes ~100-300ms to initialize
		// Once we get first auth state (user or null), we're ready
		const timer = setTimeout(() => {
			setIsAuthReady(true);
		}, 100);

		return () => clearTimeout(timer);
	}, []);

	useEffect(() => {
		// Mark as ready once we receive any auth state
		// authUser will be null for unauthenticated users (not undefined)
		if (authUser !== undefined) {
			setIsAuthReady(true);
		}
	}, [authUser]);

	return {
		isAuthReady,
		isAuthenticated: !!authUser,
		authUser: authUser || null,
	};
};
