"use client";

import { useState } from "react";
import { cn } from "@/lib/utils";
import { Tabs, TabsList, TabsTrigger, TabsContent } from "@/components/ui/tabs";
import { ProfilePostCard } from "./ProfilePostCard";
import { ProfileCommentCard } from "./ProfileCommentCard";
import { Skeleton } from "@/components/ui/skeleton";
import { Card, CardContent } from "@/components/ui/card";
import { FileText, MessageSquare, BarChart3 } from "lucide-react";
import type { Post } from "@/features/posts/types";
import type { Comment } from "@/features/comments/types";

interface ProfileContentTabsProps {
	posts: Post[] | undefined;
	comments: Comment[] | undefined;
	isPostsLoading: boolean;
	isCommentsLoading: boolean;
	className?: string;
}

/**
 * Loading skeleton for tab content.
 */
function TabContentSkeleton() {
	return (
		<div className="space-y-4">
			{[1, 2, 3].map((i) => (
				<Card key={i}>
					<CardContent className="space-y-4">
						<div className="flex items-center gap-3">
							<Skeleton className="h-10 w-10 rounded-full" />
							<div className="space-y-1">
								<Skeleton className="h-4 w-32" />
								<Skeleton className="h-3 w-20" />
							</div>
						</div>
						<Skeleton className="h-6 w-3/4" />
						<Skeleton className="h-16 w-full" />
					</CardContent>
				</Card>
			))}
		</div>
	);
}

/**
 * Empty state for tab content.
 */
function EmptyState({
	icon: Icon,
	title,
	description,
}: {
	icon: React.ElementType;
	title: string;
	description: string;
}) {
	return (
		<div className="flex flex-col items-center justify-center py-12 text-center">
			<div className="rounded-full bg-muted p-4 mb-4">
				<Icon className="h-8 w-8 text-muted-foreground" />
			</div>
			<h3 className="text-lg font-semibold text-foreground">{title}</h3>
			<p className="text-sm text-muted-foreground mt-1">{description}</p>
		</div>
	);
}

/**
 * Main content area with tabs for Posts, Comments, and Wrap Up.
 */
export function ProfileContentTabs({
	posts,
	comments,
	isPostsLoading,
	isCommentsLoading,
	className,
}: ProfileContentTabsProps) {
	const [activeTab, setActiveTab] = useState("posts");

	return (
		<div className={cn("", className)}>
			<Tabs value={activeTab} onValueChange={setActiveTab}>
				<TabsList className="w-full justify-between">
					<TabsTrigger value="posts" className="gap-2">
						<FileText className="h-4 w-4" />
						Posts
						{posts && posts.length > 0 && (
							<span className="ml-1 text-xs bg-primary/10 text-primary px-1.5 py-0.5 rounded-full">
								{posts.length}
							</span>
						)}
					</TabsTrigger>
					<TabsTrigger value="comments" className="gap-2">
						<MessageSquare className="h-4 w-4" />
						Comments
						{comments && comments.length > 0 && (
							<span className="ml-1 text-xs bg-primary/10 text-primary px-1.5 py-0.5 rounded-full">
								{comments.length}
							</span>
						)}
					</TabsTrigger>
					<TabsTrigger value="wrapup" className="gap-2">
						<BarChart3 className="h-4 w-4" />
						Wrap Up
					</TabsTrigger>
				</TabsList>

				{/* Posts Tab */}
				<TabsContent value="posts">
					{isPostsLoading ? (
						<TabContentSkeleton />
					) : posts && posts.length > 0 ? (
						<div className="space-y-4">
							{posts.map((post) => (
								<ProfilePostCard key={post.id} post={post} />
							))}
						</div>
					) : (
						<EmptyState
							icon={FileText}
							title="No posts yet"
							description="Posts you create will appear here."
						/>
					)}
				</TabsContent>

				{/* Comments Tab */}
				<TabsContent value="comments">
					{isCommentsLoading ? (
						<TabContentSkeleton />
					) : comments && comments.length > 0 ? (
						<div className="space-y-4">
							{comments.map((comment) => (
								<ProfileCommentCard
									key={comment.id}
									comment={comment}
								/>
							))}
						</div>
					) : (
						<EmptyState
							icon={MessageSquare}
							title="No comments yet"
							description="Comments you make will appear here."
						/>
					)}
				</TabsContent>

				{/* Wrap Up Tab */}
				<TabsContent value="wrapup">
					<EmptyState
						icon={BarChart3}
						title="Coming Soon"
						description="Activity summary and insights will be available here."
					/>
				</TabsContent>
			</Tabs>
		</div>
	);
}
