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
/**
 * Get size-specific CSS classes based on size prop.
 */
function getSizeClass(size: "sm" | "md" | "lg"): string {
	switch (size) {
		case "sm":
			return "h-10 w-10 text-sm";
		case "md":
			return "h-16 w-16 text-xl";
		case "lg":
			return "h-28 w-28 text-3xl";
	}
}

/**
 * Get image size string for Next.js Image optimization.
 */
function getImageSize(size: "sm" | "md" | "lg"): string {
	switch (size) {
		case "sm":
			return "40px";
		case "md":
			return "64px";
		case "lg":
			return "112px";
	}
}

export function ProfileAvatar({
	imageUrl,
	firstName,
	lastName,
	size = "lg",
	className,
	isLoading = false,
}: ProfileAvatarProps) {
	const sizeClass = getSizeClass(size);
	const imageSize = getImageSize(size);

	if (isLoading) {
		return (
			<Skeleton className={cn("rounded-full", sizeClass, className)} />
		);
	}

	const initials = getInitials(firstName, lastName);

	return (
		<div
			className={cn(
				"relative flex items-center justify-center rounded-full overflow-hidden",
				"bg-muted border-2 border-border",
				sizeClass,
				className
			)}
		>
			{imageUrl ? (
				<Image
					src={imageUrl}
					alt={`${firstName || "User"}'s profile picture`}
					fill
					className="object-cover"
					sizes={imageSize}
				/>
			) : (
				<span className="font-semibold text-muted-foreground">
					{initials}
				</span>
			)}
		</div>
	);
}
