"use client";

import { cn } from "@/lib/utils";
import { Skeleton } from "@/components/ui/skeleton";
import { Card, CardContent } from "@/components/ui/card";
import { Separator } from "@/components/ui/separator";

interface ProfilePageSkeletonProps {
	className?: string;
}

/**
 * Two-column skeleton for the profile page.
 */
export function ProfilePageSkeleton({ className }: ProfilePageSkeletonProps) {
	return (
		<div className={cn("mx-auto max-w-6xl px-4 py-6", className)}>
			<div className="flex flex-col md:flex-row gap-6">
				{/* Sidebar Skeleton */}
				<aside className="w-full md:w-72 md:sticky md:top-20 md:self-start">
					<Card>
						<CardContent className="space-y-6">
							{/* Avatar */}
							<div className="flex justify-center">
								<Skeleton className="h-28 w-28 rounded-full" />
							</div>

							<Separator />

							{/* Info */}
							<div className="space-y-3">
								<Skeleton className="h-6 w-40" />
								<Skeleton className="h-4 w-24" />
								<Skeleton className="h-4 w-32" />
								<Skeleton className="h-16 w-full" />
								<Skeleton className="h-9 w-full" />
							</div>

							<Separator />

							{/* Skills */}
							<div className="space-y-3">
								<Skeleton className="h-5 w-16" />
								<div className="flex flex-wrap gap-2">
									<Skeleton className="h-5 w-16" />
									<Skeleton className="h-5 w-20" />
									<Skeleton className="h-5 w-14" />
								</div>
							</div>

							<Separator />

							{/* Stats */}
							<div className="space-y-3">
								<Skeleton className="h-5 w-12" />
								<div className="space-y-2">
									<Skeleton className="h-4 w-32" />
									<Skeleton className="h-4 w-28" />
									<Skeleton className="h-4 w-36" />
									<Skeleton className="h-4 w-40" />
								</div>
							</div>
						</CardContent>
					</Card>
				</aside>

				{/* Main Content Skeleton */}
				<main className="flex-1 min-w-0">
					{/* Tabs */}
					<div className="mb-4">
						<div className="inline-flex h-10 items-center gap-1 rounded-lg bg-muted p-1">
							<Skeleton className="h-8 w-20 rounded-md" />
							<Skeleton className="h-8 w-24 rounded-md" />
							<Skeleton className="h-8 w-20 rounded-md" />
						</div>
					</div>

					{/* Content Cards */}
					<div className="space-y-4">
						{[1, 2, 3].map((i) => (
							<Card key={i}>
								<CardContent className="space-y-4">
									{/* Author row */}
									<div className="flex items-center gap-3">
										<Skeleton className="h-10 w-10 rounded-full" />
										<div className="space-y-1">
											<Skeleton className="h-4 w-32" />
											<Skeleton className="h-3 w-20" />
										</div>
									</div>
									{/* Title */}
									<Skeleton className="h-6 w-3/4" />
									{/* Content */}
									<Skeleton className="h-16 w-full" />
									{/* Tags */}
									<div className="flex gap-2">
										<Skeleton className="h-5 w-16" />
										<Skeleton className="h-5 w-20" />
									</div>
									{/* Footer */}
									<div className="flex justify-between pt-2">
										<div className="flex gap-4">
											<Skeleton className="h-5 w-12" />
											<Skeleton className="h-5 w-12" />
										</div>
										<Skeleton className="h-8 w-24" />
									</div>
								</CardContent>
							</Card>
						))}
					</div>
				</main>
			</div>
		</div>
	);
}
