"use client";

import { useState, useCallback } from "react";
import { signInWithPopup, GoogleAuthProvider } from "firebase/auth";
import { auth } from "@/lib/firebase";
import { getFirebaseErrorMessage } from "@/lib/validations/auth";
import {
	useCreateOrUpdateUserMutation,
	useLazyGetCurrentUserProfileQuery,
} from "@/features/profiles/profilesSlice";
import { useSignOut } from "./useSignOut";

interface UseSignInWithGoogleReturn {
	signInWithGoogle: () => Promise<boolean>;
	loading: boolean;
	error: string | null;
	clearError: () => void;
}

const googleProvider = new GoogleAuthProvider();

/**
 * Hook for Google OAuth sign in with backend user creation/update.
 *
 * Handles:
 * - Google popup sign in
 * - Backend user creation (upsert pattern)
 * - Profile prefetch for cache population
 * - Rollback on backend failure
 *
 * @example
 * ```tsx
 * const { signInWithGoogle, loading, error } = useSignInWithGoogle();
 *
 * const handleGoogleClick = async () => {
 *   const success = await signInWithGoogle();
 *   if (success) router.push("/home");
 * };
 * ```
 */
export function useSignInWithGoogle(): UseSignInWithGoogleReturn {
	const [loading, setLoading] = useState(false);
	const [error, setError] = useState<string | null>(null);
	const [createOrUpdateUser] = useCreateOrUpdateUserMutation();
	const [prefetchProfile] = useLazyGetCurrentUserProfileQuery();
	const { signOut } = useSignOut();

	const clearError = useCallback(() => {
		setError(null);
	}, []);

	const signInWithGoogle = useCallback(async (): Promise<boolean> => {
		setLoading(true);
		setError(null);

		try {
			// Sign in with Google popup
			const result = await signInWithPopup(auth, googleProvider);
			const user = result.user;

			// Extract name parts from display name
			const nameParts = user.displayName?.split(" ") || ["User"];
			const firstName = nameParts[0];
			const lastName = nameParts.slice(1).join(" ") || undefined;

			// Create or update user in backend
			try {
				// Validate email is present - required for account creation
				if (!user.email) {
					throw new Error("Email is required for account creation");
				}

				await createOrUpdateUser({
					firstName,
					lastName,
					email: user.email,
					username: user.email,
					providerId: user.uid,
					profilePictureUrl: user.photoURL || undefined,
					phoneNumber: user.phoneNumber || undefined,
					isEmailVerified: user.emailVerified,
				}).unwrap();

				// Prefetch profile to populate RTK Query cache
				// This ensures AppInitializer has data immediately
				await prefetchProfile(undefined);
			} catch (backendError) {
				// Rollback: sign out from Firebase if backend fails
				await signOut();
				console.error("Backend user creation failed:", backendError);
				setError("Failed to create your account. Please try again.");
				return false;
			}

			return true;
		} catch (err) {
			// Handle popup closed or other Firebase errors
			const message = getFirebaseErrorMessage(err);

			// Don't show error for user-cancelled actions
			if (message !== "Sign-in cancelled") {
				setError(message);
			}

			return false;
		} finally {
			setLoading(false);
		}
	}, [createOrUpdateUser, prefetchProfile, signOut]);

	return { signInWithGoogle, loading, error, clearError };
}
