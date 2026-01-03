"use client";

import { type ReactNode } from "react";
import { useAuth } from "@/lib/firebase/AuthProvider";
import { LoadingScreen } from "./LoadingScreen";

interface AppInitializerProps {
	children: ReactNode;
}

/**
 * Wrapper component that shows loading screen during initial Firebase auth check.
 *
 * Only blocks rendering while Firebase auth state is being determined.
 * Profile loading is handled by individual pages/components that need the data.
 *
 * This ensures users don't see a flash of unauthenticated content
 * when they're actually logged in.
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
	const { loading: authLoading } = useAuth();

	// Only show loading during initial Firebase auth check
	if (authLoading) {
		return <LoadingScreen />;
	}

	return <>{children}</>;
}
