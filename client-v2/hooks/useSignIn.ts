"use client";

import { useState, useCallback } from "react";
import { signInWithEmailAndPassword } from "firebase/auth";
import { auth } from "@/lib/firebase";
import { getFirebaseErrorMessage } from "@/lib/validations/auth";
import { useLazyGetCurrentUserProfileQuery } from "@/features/profiles/profilesSlice";

interface UseSignInReturn {
	signIn: (email: string, password: string) => Promise<boolean>;
	loading: boolean;
	error: string | null;
	clearError: () => void;
}

/**
 * Hook for email/password sign in.
 *
 * Handles:
 * - Firebase authentication
 * - Email verification check
 * - Profile prefetch for cache population
 * - Error message mapping
 *
 * @example
 * ```tsx
 * const { signIn, loading, error } = useSignIn();
 *
 * const handleSubmit = async (data) => {
 *   const success = await signIn(data.email, data.password);
 *   if (success) router.push("/home");
 * };
 * ```
 */
export function useSignIn(): UseSignInReturn {
	const [loading, setLoading] = useState(false);
	const [error, setError] = useState<string | null>(null);
	const [prefetchProfile] = useLazyGetCurrentUserProfileQuery();

	const clearError = useCallback(() => {
		setError(null);
	}, []);

	const signIn = useCallback(
		async (email: string, password: string): Promise<boolean> => {
			setLoading(true);
			setError(null);

			try {
				const userCredential = await signInWithEmailAndPassword(
					auth,
					email,
					password
				);

				// Check email verification
				if (!userCredential.user.emailVerified) {
					// Sign out unverified user
					await auth.signOut();
					setError(
						"Please verify your email before signing in. Check your inbox for the verification link."
					);
					return false;
				}

				// Prefetch profile to populate RTK Query cache
				// This ensures AppInitializer has data immediately
				await prefetchProfile();

				return true;
			} catch (err) {
				setError(getFirebaseErrorMessage(err));
				return false;
			} finally {
				setLoading(false);
			}
		},
		[prefetchProfile]
	);

	return { signIn, loading, error, clearError };
}
