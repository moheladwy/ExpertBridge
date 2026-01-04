"use client";

import { useRouter } from "next/navigation";
import { Button } from "@/components/ui/button";
import { Briefcase, LogIn } from "lucide-react";
import { useIsUserLoggedIn } from "@/hooks/useIsUserLoggedIn";

interface HireMeButtonProps {
	profileId: string;
	profileName?: string;
	className?: string;
}

/**
 * Hire Me button for user profiles.
 *
 * - For unauthenticated users: Shows "Sign in to Hire" and redirects to sign-in
 * - For authenticated users: Shows "Hire Me" button (action TBD)
 */
export function HireMeButton({
	profileId,
	profileName,
	className,
}: HireMeButtonProps) {
	const router = useRouter();
	const { isAuthenticated, isLoading } = useIsUserLoggedIn();

	const handleHireClick = () => {
		if (!isAuthenticated) {
			// Redirect to sign-in with return URL
			const returnUrl = encodeURIComponent(`/profile/${profileId}`);
			router.push(`/auth/signin?redirect=${returnUrl}`);
			return;
		}

		// TODO: Implement hire flow (open hire modal, navigate to job creation, etc.)
		console.log(`Hire request for profile: ${profileId}`);
	};

	if (isLoading) {
		return (
			<Button variant="default" size="sm" className={className} disabled>
				<Briefcase className="h-4 w-4 mr-2" />
				Loading...
			</Button>
		);
	}

	if (!isAuthenticated) {
		return (
			<Button
				variant="default"
				size="sm"
				className={className}
				onClick={handleHireClick}
			>
				<LogIn className="h-4 w-4 mr-2" />
				Sign in to Hire
			</Button>
		);
	}

	return (
		<Button
			variant="default"
			size="sm"
			className={className}
			onClick={handleHireClick}
		>
			<Briefcase className="h-4 w-4 mr-2" />
			Hire {profileName || "Me"}
		</Button>
	);
}
