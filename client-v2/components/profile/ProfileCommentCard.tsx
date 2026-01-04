"use client";

import Link from "next/link";
import Image from "next/image";
import { cn } from "@/lib/utils";
import type { Comment } from "@/features/comments/types";

interface ProfileCommentCardProps {
	comment: Comment;
	postTitle?: string;
	className?: string;
}

/**
 * Format relative time from ISO date string.
 */
function formatTimeAgo(dateString: string): string {
	const date = new Date(dateString);
	const now = new Date();
	const diffMs = now.getTime() - date.getTime();
	const diffMins = Math.floor(diffMs / 60000);
	const diffHours = Math.floor(diffMs / 3600000);
	const diffDays = Math.floor(diffMs / 86400000);

	if (diffMins < 1) return "Just now";
	if (diffMins < 60) return `${diffMins}m ago`;
	if (diffHours < 24) return `${diffHours}h ago`;
	if (diffDays < 7) return `${diffDays}d ago`;

	return date.toLocaleDateString("en-US", {
		month: "short",
		day: "numeric",
		year: date.getFullYear() !== now.getFullYear() ? "numeric" : undefined,
	});
}

/**
 * Comment card for profile page - displays a user's comment.
 */
export function ProfileCommentCard({
	comment,
	postTitle,
	className,
}: ProfileCommentCardProps) {
	const netVotes = comment.upvotes - comment.downvotes;
	const authorName = [comment.author?.firstName, comment.author?.lastName]
		.filter(Boolean)
		.join(" ") || "Anonymous";

	return (
		<div
			className={cn(
				"group flex flex-col gap-3 p-4 border border-border rounded-xl bg-card",
				"hover:border-primary/50 hover:shadow-lg transition-all duration-300",
				className
			)}
		>
			{/* Post Reference */}
			{postTitle && (
				<div className="mb-2 text-sm text-muted-foreground">
					<span className="font-semibold">On Post: </span>
					<Link
						href={`/feed/${comment.postId}`}
						className="hover:text-primary hover:underline transition-colors"
						dir="auto"
					>
						{postTitle}
					</Link>
				</div>
			)}

			{/* Comment Author */}
			<div className="flex items-center space-x-3">
				<div className="relative h-8 w-8 rounded-full overflow-hidden bg-muted">
					{comment.author?.profilePictureUrl ? (
						<Image
							src={comment.author.profilePictureUrl}
							alt={authorName}
							fill
							className="object-cover"
							sizes="32px"
						/>
					) : (
						<div className="flex h-full w-full items-center justify-center text-xs font-semibold text-muted-foreground">
							{authorName.charAt(0).toUpperCase()}
						</div>
					)}
				</div>
				<div>
					<h4 className="text-sm font-semibold text-card-foreground">
						{authorName}
					</h4>
					<p className="text-xs text-muted-foreground">
						{formatTimeAgo(comment.createdAt)}
					</p>
				</div>
			</div>

			{/* Comment Content */}
			<div className="w-full wrap-break-word">
				<p
					className="text-card-foreground whitespace-pre-wrap leading-relaxed"
					dir="auto"
				>
					{comment.content}
				</p>
			</div>

			{/* Footer */}
			<div className="flex items-center justify-between pt-2">
				{/* Vote Display */}
				<div className="flex items-center gap-1">
					<span
						className={cn(
							"font-medium text-sm px-2 py-1 rounded-full",
							netVotes > 0
								? "bg-green-500/10 text-green-600 border border-green-200"
								: netVotes < 0
									? "bg-destructive/10 text-destructive border border-destructive/20"
									: "bg-muted text-muted-foreground"
						)}
					>
						{netVotes > 0 ? "+" : ""}
						{netVotes} votes
					</span>
				</div>

				{/* View post link */}
				<Link
					href={`/feed/${comment.postId}`}
					className="text-xs font-medium text-primary hover:text-primary/80 transition-colors"
				>
					View Discussion →
				</Link>
			</div>
		</div>
	);
}
