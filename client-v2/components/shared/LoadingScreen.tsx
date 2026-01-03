"use client";

import { LogoIcon } from "@/components/shared/logo";

/**
 * Full-screen branded loading screen.
 * Displays logo with spinning ring animation.
 *
 * Used during:
 * - Initial app load (restoring session)
 * - After sign in (fetching profile)
 * - After sign up + onboarding (redirecting)
 *
 * @example
 * ```tsx
 * // Direct usage
 * if (isLoading) return <LoadingScreen />;
 *
 * // Via context
 * const { showLoading, hideLoading } = useAppLoading();
 * ```
 */
export function LoadingScreen() {
	return (
		<div className="fixed inset-0 z-50 flex items-center justify-center bg-background">
			<div className="relative flex items-center justify-center">
				{/* Spinning ring */}
				<div className="absolute h-24 w-24 animate-spin rounded-full border-4 border-primary border-t-transparent" />

				{/* Logo centered inside spinner */}
				<LogoIcon className="h-10 w-10" />
			</div>
		</div>
	);
}
