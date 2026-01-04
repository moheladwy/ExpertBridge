"use client";

import { cn } from "@/lib/utils";
import { Card, CardContent } from "@/components/ui/card";
import { Separator } from "@/components/ui/separator";
import { ProfileAvatar } from "./ProfileAvatar";
import { ProfileInfo } from "./ProfileInfo";
import { ProfileSkills } from "./ProfileSkills";
import { ProfileStats } from "./ProfileStats";
import type { ProfileResponse } from "@/features/profiles/types";

interface ProfileSidebarProps {
	profile: ProfileResponse | null | undefined;
	postsCount?: number;
	commentsCount?: number;
	isLoading?: boolean;
	onEditProfile?: () => void;
	className?: string;
}

/**
 * Profile sidebar combining avatar, info, skills, and stats.
 * Sticky on desktop, stacks on mobile.
 */
export function ProfileSidebar({
	profile,
	postsCount = 0,
	commentsCount = 0,
	isLoading = false,
	onEditProfile,
	className,
}: ProfileSidebarProps) {
	return (
		<Card className={cn("", className)}>
			<CardContent className="space-y-6">
				{/* Avatar */}
				<div className="flex justify-center">
					<ProfileAvatar
						imageUrl={profile?.profilePictureUrl}
						firstName={profile?.firstName}
						lastName={profile?.lastName}
						size="lg"
						isLoading={isLoading}
					/>
				</div>

				<Separator />

				{/* Info */}
				<ProfileInfo
					profile={profile}
					isLoading={isLoading}
					onEditProfile={onEditProfile}
				/>

				<Separator />

				{/* Skills */}
				<ProfileSkills skills={profile?.skills} isLoading={isLoading} />

				<Separator />

				{/* Stats */}
				<ProfileStats
					profile={profile}
					postsCount={postsCount}
					commentsCount={commentsCount}
					isLoading={isLoading}
				/>
			</CardContent>
		</Card>
	);
}
