"use client";

import { cn } from "@/lib/utils";
import { Skeleton } from "@/components/ui/skeleton";
import { Separator } from "@/components/ui/separator";
import { Calendar, FileText, MessageSquare, Star } from "lucide-react";
import type { ProfileResponse } from "@/features/profiles/types";
import { formatShortDate } from "@/lib/util/date";

interface ProfileStatsProps {
	profile: ProfileResponse | null | undefined;
	postsCount?: number;
	commentsCount?: number;
	isLoading?: boolean;
	className?: string;
}

interface StatItemProps {
	icon: React.ReactNode;
	label: string;
	value: string | number;
}

function StatItem({ icon, label, value }: StatItemProps) {
	return (
		<div className="flex items-center gap-2 text-sm">
			<span className="text-muted-foreground">{icon}</span>
			<span className="text-muted-foreground">{label}:</span>
			<span className="font-medium text-foreground">{value}</span>
		</div>
	);
}

/**
 * Profile stats section displaying reputation, posts count, comments count, and join date.
 */
export function ProfileStats({
	profile,
	postsCount = 0,
	commentsCount = 0,
	isLoading = false,
	className,
}: ProfileStatsProps) {
	if (isLoading) {
		return (
			<div className={cn("space-y-3", className)}>
				<Skeleton className="h-5 w-20" />
				<div className="space-y-2">
					<Skeleton className="h-4 w-32" />
					<Skeleton className="h-4 w-28" />
					<Skeleton className="h-4 w-36" />
					<Skeleton className="h-4 w-40" />
				</div>
			</div>
		);
	}

	return (
		<div className={cn("space-y-3", className)}>
			<h3 className="text-sm font-semibold text-foreground">Stats</h3>
			<Separator />

			<div className="space-y-2">
				<StatItem
					icon={<Star className="h-4 w-4" />}
					label="Reputation"
					value={profile?.reputation ?? 0}
				/>
				<StatItem
					icon={<FileText className="h-4 w-4" />}
					label="Posts"
					value={postsCount}
				/>
				<StatItem
					icon={<MessageSquare className="h-4 w-4" />}
					label="Comments"
					value={commentsCount}
				/>
				<StatItem
					icon={<Calendar className="h-4 w-4" />}
					label="Joined"
					value={formatShortDate(profile?.createdAt)}
				/>
			</div>
		</div>
	);
}
