"use client";

import { type ReactNode, useMemo } from "react";
import { useAuth } from "@/lib/firebase/AuthProvider";
import { auth } from "@/lib/firebase";
import { useGetCurrentUserProfileQuery } from "@/features/profiles/profilesSlice";
import { LoadingScreen } from "./LoadingScreen";

interface AppInitializerProps {
	children: ReactNode;
}

/**
 * Wrapper component that shows loading screen during initial app load.
 *
 * Displays loading screen while:
 * 1. Firebase auth state is being determined
 * 2. User profile is being fetched (if logged in)
 *
 * This ensures users don't see a flash of unauthenticated content
 * when they're actually logged in.
 *
 * Uses both React state (useAuth) and direct Firebase state (auth.currentUser)
 * to handle the race condition after sign-in where React state hasn't updated yet.
 *
 * @example
 * ```tsx
 * // In providers.tsx
 * <AuthProvider>
 *   <AppInitializer>
 *     {children}
 *   </AppInitializer>
 * </AuthProvider>
 * ```
 */
export function AppInitializer({ children }: AppInitializerProps) {
	const { user, loading: authLoading } = useAuth();

	// Determine if we should fetch profile - check both:
	// 1. React state (user from useAuth) - for normal reactive updates
	// 2. Firebase direct state (auth.currentUser) - for race condition after sign-in
	const shouldFetchProfile = useMemo(() => {
		return !!user || !!auth.currentUser;
	}, [user]);

	const { isLoading: profileLoading, isSuccess: profileSuccess } =
		useGetCurrentUserProfileQuery(undefined, {
			skip: !shouldFetchProfile,
		});

	// Determine if we should show loading
	// Only show loading during initial load, not during background refetch
	// isLoading = true only on first fetch, isSuccess = true after any successful fetch
	const isProfilePending =
		shouldFetchProfile && profileLoading && !profileSuccess;
	const showLoading = authLoading || isProfilePending;

	if (showLoading) {
		return <LoadingScreen />;
	}

	return <>{children}</>;
}
