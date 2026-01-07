"use client";

import Link from "next/link";
import Image from "next/image";
import { cn } from "@/lib/utils";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { ArrowBigUp, ArrowBigDown, MessageCircle } from "lucide-react";
import type { Post } from "@/features/posts/types";
import { formatTimeAgo } from "@/lib/util/date";

interface ProfilePostCardProps {
	post: Post;
	className?: string;
}

/**
 * Post card for profile page - displays a user's post.
 */
export function ProfilePostCard({ post, className }: ProfilePostCardProps) {
	const netVotes = post.upvotes - post.downvotes;
	const authorName =
		[post.author?.firstName, post.author?.lastName]
			.filter(Boolean)
			.join(" ") || "Anonymous";

	return (
		<div
			className={cn(
				"group flex flex-col gap-3 bg-card rounded-xl p-4 border border-border",
				"hover:border-primary/50 hover:shadow-lg transition-all duration-300",
				className
			)}
		>
			{/* Author Info */}
			<div className="flex items-center space-x-3">
				<Link href={`/profile/${post.author?.id}`}>
					<div className="relative h-10 w-10 rounded-full overflow-hidden bg-muted">
						{post.author?.profilePictureUrl ? (
							<Image
								src={post.author.profilePictureUrl}
								alt={authorName}
								fill
								className="object-cover"
								sizes="40px"
							/>
						) : (
							<div className="flex h-full w-full items-center justify-center text-sm font-semibold text-muted-foreground">
								{authorName.charAt(0).toUpperCase()}
							</div>
						)}
					</div>
				</Link>
				<div className="flex w-full justify-between">
					<div>
						<Link href={`/profile/${post.author?.id}`}>
							<h3 className="text-md font-semibold text-card-foreground hover:text-primary transition-colors">
								{authorName}
							</h3>
						</Link>
						<p className="text-xs text-muted-foreground">
							{formatTimeAgo(post.createdAt)}
							{post.lastModified && " (edited)"}
						</p>
					</div>
				</div>
			</div>

			{/* Post Title */}
			<div className="wrap-break-word">
				<h2
					className="text-lg font-bold text-card-foreground whitespace-pre-wrap group-hover:text-primary transition-colors"
					dir="auto"
				>
					{post.title}
				</h2>
			</div>

			{/* Post Content */}
			<div className="wrap-break-word">
				<p
					className="text-muted-foreground whitespace-pre-wrap line-clamp-3 leading-relaxed"
					dir="auto"
				>
					{post.content}
				</p>
			</div>

			{/* Tags */}
			{post.tags?.length > 0 && (
				<div className="flex flex-wrap gap-2">
					{post.tags.map((tag, index) => (
						<Badge
							key={index}
							variant="secondary"
							className="text-xs"
						>
							{tag.englishName || tag.arabicName || "Tag"}
						</Badge>
					))}
				</div>
			)}

			{/* Footer */}
			<div className="flex justify-between items-center mt-2">
				<div className="flex space-x-4">
					{/* Votes Display */}
					<div className="flex items-center gap-1 text-muted-foreground">
						<div className="flex items-center">
							{netVotes >= 0 ? (
								<ArrowBigUp className="text-muted-foreground w-5 h-5" />
							) : (
								<ArrowBigDown className="text-muted-foreground w-5 h-5" />
							)}
							<span
								className={cn(
									"ml-1",
									netVotes < 0
										? "text-destructive"
										: "text-muted-foreground"
								)}
							>
								{Math.abs(netVotes)}
							</span>
						</div>
					</div>

					{/* Comments */}
					<div className="flex items-center gap-1 text-muted-foreground">
						<MessageCircle className="w-5 h-5" />
						<span>{post.comments}</span>
					</div>
				</div>

				{/* View Button */}
				<Button
					variant="outline"
					size="sm"
					className="text-primary border-primary hover:bg-primary hover:text-primary-foreground"
				>
					<Link href={`/feed/${post.id}`}>View Post</Link>
				</Button>
			</div>
		</div>
	);
}
