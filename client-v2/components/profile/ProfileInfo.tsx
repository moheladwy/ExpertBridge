"use client";

import type { ReactNode } from "react";
import { cn } from "@/lib/utils";
import { Button } from "@/components/ui/button";
import { Skeleton } from "@/components/ui/skeleton";
import { Pencil } from "lucide-react";
import type { ProfileResponse } from "@/features/profiles/types";

interface ProfileInfoProps {
	profile: ProfileResponse | null | undefined;
	isLoading?: boolean;
	onEditProfile?: () => void;
	/** Custom action button (e.g., HireMeButton) - takes precedence over onEditProfile */
	customAction?: ReactNode;
	className?: string;
}

/**
 * Profile info section displaying name, username, job title, and bio.
 */
export function ProfileInfo({
	profile,
	isLoading = false,
	onEditProfile,
	customAction,
	className,
}: ProfileInfoProps) {
	if (isLoading) {
		return (
			<div className={cn("space-y-3", className)}>
				<Skeleton className="h-6 w-40" />
				<Skeleton className="h-4 w-24" />
				<Skeleton className="h-4 w-32" />
				<Skeleton className="h-16 w-full" />
				<Skeleton className="h-9 w-full" />
			</div>
		);
	}

	const fullName = [profile?.firstName, profile?.lastName]
		.filter(Boolean)
		.join(" ");

	return (
		<div className={cn("space-y-3", className)}>
			{/* Full Name */}
			<h2 className="text-xl font-semibold text-foreground">
				{fullName || "Anonymous User"}
			</h2>

			{/* Username */}
			{profile?.username && (
				<p className="text-sm text-muted-foreground">
					@{profile.username}
				</p>
			)}

			{/* Job Title */}
			{profile?.jobTitle && (
				<p className="text-sm font-medium text-foreground/80">
					{profile.jobTitle}
				</p>
			)}

			{/* Bio */}
			{profile?.bio && (
				<p className="text-sm text-muted-foreground leading-relaxed">
					{profile.bio}
				</p>
			)}

			{/* Custom Action Button (e.g., Hire Me) */}
			{customAction && <div className="mt-4 w-full">{customAction}</div>}

			{/* Edit Profile Button (only if no customAction) */}
			{!customAction && onEditProfile && (
				<Button
					variant="outline"
					size="sm"
					className="w-full mt-4"
					onClick={onEditProfile}
				>
					<Pencil className="h-4 w-4 mr-2" />
					Edit Profile
				</Button>
			)}
		</div>
	);
}
