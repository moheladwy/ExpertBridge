"use client";

import { cn } from "@/lib/utils";
import { Badge } from "@/components/ui/badge";
import { Skeleton } from "@/components/ui/skeleton";

interface ProfileSkillsProps {
	skills?: string[];
	isLoading?: boolean;
	className?: string;
}

/**
 * Profile skills section displaying skill badges.
 */
export function ProfileSkills({
	skills,
	isLoading = false,
	className,
}: ProfileSkillsProps) {
	if (isLoading) {
		return (
			<div className={cn("space-y-3", className)}>
				<Skeleton className="h-5 w-20" />
				<div className="flex flex-wrap gap-2">
					<Skeleton className="h-5 w-16" />
					<Skeleton className="h-5 w-20" />
					<Skeleton className="h-5 w-14" />
					<Skeleton className="h-5 w-16" />
				</div>
			</div>
		);
	}

	const hasSkills = skills && skills.length > 0;

	return (
		<div className={cn("space-y-3", className)}>
			<h3 className="text-sm font-semibold text-foreground">Skills</h3>

			{hasSkills ? (
				<div className="flex flex-wrap gap-2">
					{skills.map((skill, index) => (
						<Badge key={index} variant="secondary">
							{skill}
						</Badge>
					))}
				</div>
			) : (
				<p className="text-sm text-muted-foreground italic">
					No skills added yet
				</p>
			)}
		</div>
	);
}
