"use client";

import Image from "next/image";
import { cn } from "@/lib/utils";
import { Skeleton } from "@/components/ui/skeleton";

interface ProfileAvatarProps {
	imageUrl?: string | null;
	firstName?: string | null;
	lastName?: string | null;
	size?: "sm" | "md" | "lg";
	className?: string;
	isLoading?: boolean;
}

/**
 * Get initials from first and last name.
 */
function getInitials(
	firstName?: string | null,
	lastName?: string | null
): string {
	const first = firstName?.charAt(0).toUpperCase() || "";
	const last = lastName?.charAt(0).toUpperCase() || "";
	return first + last || "?";
}

/**
 * Profile avatar component with image or fallback initials.
 */
export function ProfileAvatar({
	imageUrl,
	firstName,
	lastName,
	size = "lg",
	className,
	isLoading = false,
}: ProfileAvatarProps) {
	const sizeClasses = {
		sm: "h-10 w-10 text-sm",
		md: "h-16 w-16 text-xl",
		lg: "h-28 w-28 text-3xl",
	};

	// Dynamic sizes for Next.js Image optimization
	const imageSizes = {
		sm: "40px",
		md: "64px",
		lg: "112px",
	};

	if (isLoading) {
		return (
			<Skeleton
				className={cn("rounded-full", sizeClasses[size], className)}
			/>
		);
	}

	const initials = getInitials(firstName, lastName);

	return (
		<div
			className={cn(
				"relative flex items-center justify-center rounded-full overflow-hidden",
				"bg-muted border-2 border-border",
				sizeClasses[size],
				className
			)}
		>
			{imageUrl ? (
				<Image
					src={imageUrl}
					alt={`${firstName || "User"}'s profile picture`}
					fill
					className="object-cover"
					sizes={imageSizes[size]}
				/>
			) : (
				<span className="font-semibold text-muted-foreground">
					{initials}
				</span>
			)}
		</div>
	);
}
