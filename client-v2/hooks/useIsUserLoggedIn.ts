"use client";

import { useAuth } from "@/lib/firebase/AuthProvider";
import { useGetCurrentUserProfileQuery } from "@/features/profiles/profilesSlice";
import type { ProfileResponse } from "@/features/profiles/types";
import type { User } from "firebase/auth";

export interface AuthState {
	/** Firebase user object */
	firebaseUser: User | null;
	/** App profile from backend API */
	profile: ProfileResponse | undefined;
	/** True if Firebase auth is still initializing */
	isAuthLoading: boolean;
	/** True if profile is being fetched */
	isProfileLoading: boolean;
	/** True if user is fully authenticated (Firebase + profile) */
	isAuthenticated: boolean;
	/** True if user needs to complete onboarding */
	needsOnboarding: boolean;
	/** Combined loading state */
	isLoading: boolean;
	/** Error from profile fetch */
	error: unknown;
}

/**
 * Hook to determine if user is fully logged in.
 *
 * Combines:
 * - Firebase Auth state (from AuthProvider context)
 * - App profile state (from RTK Query)
 *
 * This simplified version replaces the 80-line hook that used:
 * - Custom TokenManager
 * - Separate Redux authSlice
 * - Multiple useState/useEffect calls
 *
 * @example
 * ```tsx
 * function MyComponent() {
 *   const { isAuthenticated, isLoading, profile, needsOnboarding } = useIsUserLoggedIn();
 *
 *   if (isLoading) return <Spinner />;
 *   if (!isAuthenticated) return <LoginPrompt />;
 *   if (needsOnboarding) return <OnboardingFlow />;
 *
 *   return <Dashboard profile={profile} />;
 * }
 * ```
 */
export function useIsUserLoggedIn(): AuthState {
	const { user: firebaseUser, loading: isAuthLoading } = useAuth();

	// Only fetch profile if Firebase user exists
	const {
		data: profile,
		isLoading: isProfileLoading,
		error,
	} = useGetCurrentUserProfileQuery(undefined, {
		// Skip query if not authenticated
		skip: !firebaseUser,
	});

	// User is authenticated if both Firebase user and profile exist
	const isAuthenticated = Boolean(firebaseUser && profile);

	// Check if user needs onboarding
	const needsOnboarding = Boolean(profile && !profile.isOnboarded);

	// Combined loading state
	const isLoading =
		isAuthLoading || (Boolean(firebaseUser) && isProfileLoading);

	return {
		firebaseUser,
		profile,
		isAuthLoading,
		isProfileLoading,
		isAuthenticated,
		needsOnboarding,
		isLoading,
		error,
	};
}

/**
 * Hook to get just the current user profile (convenience wrapper).
 */
export function useCurrentProfile(): ProfileResponse | undefined {
	const { profile } = useIsUserLoggedIn();
	return profile;
}
