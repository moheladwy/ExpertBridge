"use client";

import { useEffect } from "react";
import { useRouter, usePathname } from "next/navigation";
import { useAuth } from "@/lib/firebase/AuthProvider";

interface ProtectedRouteProps {
	children: React.ReactNode;
}

/**
 * Wrapper component that protects routes from unauthenticated access.
 * Redirects to sign-in page with return URL if user is not logged in.
 *
 * @example
 * ```tsx
 * // In a protected page
 * export default function ProfilePage() {
 *   return (
 *     <ProtectedRoute>
 *       <ProfileContent />
 *     </ProtectedRoute>
 *   );
 * }
 * ```
 */
export function ProtectedRoute({ children }: ProtectedRouteProps) {
	const router = useRouter();
	const pathname = usePathname();
	const { user, loading } = useAuth();

	useEffect(() => {
		if (!loading && !user) {
			const redirectUrl = `/auth/signin?redirect=${encodeURIComponent(pathname)}`;
			router.push(redirectUrl);
		}
	}, [user, loading, router, pathname]);

	// Show loading spinner while checking auth
	if (loading) {
		return (
			<div className="flex min-h-[60vh] items-center justify-center">
				<div className="h-8 w-8 animate-spin rounded-full border-4 border-primary border-t-transparent" />
			</div>
		);
	}

	// Don't render content if not authenticated
	if (!user) {
		return null;
	}

	return <>{children}</>;
}
