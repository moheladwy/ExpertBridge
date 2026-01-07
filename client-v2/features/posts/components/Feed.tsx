"use client";

import React, { useEffect, useRef, useState } from "react";
import { useRouter } from "next/navigation";
import Image from "next/image";
import { TrendingUp, Clock, ThumbsUp, Sparkles } from "lucide-react";

import { Button } from "@/components/ui/button";
import { useGetPostsCursorInfiniteQuery } from "@/features/posts/postsSlice";
import type {
	Post,
	PostsCursorPaginatedResponse,
} from "@/features/posts/types";
import { useIsUserLoggedIn } from "@/hooks/useIsUserLoggedIn";
import useRefetchOnLogin from "@/hooks/useRefetchOnLogin";
import { useCallbackOnIntersection } from "@/hooks/useCallbackOnIntersection";
import { useAuthPrompt } from "@/lib/contexts/AuthPromptContext";

import PostCard from "./PostCard";
import LoadingSkeleton from "./LoadingSkeleton";

// Default profile picture
const DEFAULT_PROFILE_PIC = "/ProfilePic.svg";

// Number of posts to fetch per page
const PAGE_SIZE = 10;

type FilterOption = "Recommended" | "Recent" | "Most Upvoted" | "Trending";

interface FeedProps {
	/** Optional post ID to scroll to on mount */
	startingPostId?: string | null;
}

/**
 * Feed component that displays posts with infinite scroll.
 * Includes filter tabs and a create post prompt.
 */
const Feed = ({ startingPostId = null }: FeedProps) => {
	const router = useRouter();
	const { showAuthPrompt } = useAuthPrompt();
	const { isAuthenticated, profile } = useIsUserLoggedIn();

	// RTK Query infinite query for posts
	const {
		hasNextPage,
		data,
		isFetching,
		isLoading,
		isError,
		fetchNextPage,
		isFetchingNextPage,
		refetch,
	} = useGetPostsCursorInfiniteQuery(undefined, {
		initialPageParam: {
			pageSize: PAGE_SIZE,
			page: 1,
		},
	});

	// Current filter selection
	const [filter, setFilter] = useState<FilterOption>("Recommended");

	// Intersection observer for infinite scroll
	const loadMoreRef = useCallbackOnIntersection(fetchNextPage);

	// Ref for scrolling to a specific post
	const startingPostRef = useRef<HTMLDivElement>(null);
	const [hasCentered, setHasCentered] = useState(false);

	// Scroll to starting post when data loads
	useEffect(() => {
		if (hasCentered || !startingPostId) return;
		const startingElement = startingPostRef.current;
		if (startingElement) {
			startingElement.scrollIntoView({
				behavior: "auto",
				block: "center",
			});
			Promise.resolve().then(() => setHasCentered(true));
		}
	}, [data?.pages, hasCentered, startingPostId]);

	// Refetch when user logs in
	useRefetchOnLogin(refetch);

	// Handle create post click
	const handleCreatePost = () => {
		if (isAuthenticated) {
			router.push("/posts/create");
		} else {
			showAuthPrompt();
		}
	};

	// Get icon for filter option
	const getFilterIcon = (filterName: FilterOption) => {
		switch (filterName) {
			case "Recommended":
				return <Sparkles className="h-4 w-4" />;
			case "Recent":
				return <Clock className="h-4 w-4" />;
			case "Most Upvoted":
				return <ThumbsUp className="h-4 w-4" />;
			case "Trending":
				return <TrendingUp className="h-4 w-4" />;
		}
	};

	// Sort posts based on current filter
	const sortPosts = (posts: Post[]): Post[] => {
		const sorted = [...posts];

		switch (filter) {
			case "Recent":
				sorted.sort(
					(a, b) =>
						new Date(b.createdAt).getTime() -
						new Date(a.createdAt).getTime()
				);
				break;
			case "Most Upvoted":
				sorted.sort(
					(a, b) =>
						b.upvotes - b.downvotes - (a.upvotes - a.downvotes)
				);
				break;
			case "Trending":
				sorted.sort(
					(a, b) =>
						b.upvotes +
						b.downvotes +
						(b.comments || 0) -
						(a.upvotes + a.downvotes + (a.comments || 0))
				);
				break;
			default:
				// "Recommended" - keep original order (relevance score)
				break;
		}

		return sorted;
	};

	const profilePicUrl = profile?.profilePictureUrl || DEFAULT_PROFILE_PIC;

	return (
		<div className="min-h-screen bg-secondary">
			<div className="mx-auto flex max-w-7xl gap-4 p-4">
				{/* Main Feed Content */}
				<main className="mx-auto max-w-4xl flex-1 space-y-4">
					{/* Create Post Section */}
					<div className="overflow-hidden rounded-2xl border border-border bg-card py-4 px-10 shadow-lg">
						<div className="flex cursor-pointer items-center justify-center gap-3">
							{isAuthenticated && profile && (
								<Image
									src={profilePicUrl}
									alt="Your profile"
									width={45}
									height={45}
									className="rounded-full object-cover"
									unoptimized={profilePicUrl.startsWith(
										"http"
									)}
									onClick={() => {
										router.push(`/profile`);
									}}
								/>
							)}
							<Button
								variant="ghost"
								className="w-full rounded-full bg-muted px-5 text-muted-foreground hover:bg-accent hover:text-primary hover:cursor-pointer"
								onClick={handleCreatePost}
							>
								<span className="w-full text-left">
									What do you want to ask?
								</span>
							</Button>
						</div>
					</div>

					{/* Filter Section */}
					<div className="rounded-2xl border border-border bg-card p-4 shadow-lg">
						<div className="flex items-center justify-between gap-2 rounded-xl bg-muted p-1">
							{(
								[
									"Recommended",
									"Recent",
									"Most Upvoted",
									"Trending",
								] as FilterOption[]
							).map((filterOption) => (
								<button
									key={filterOption}
									type="button"
									onClick={() => setFilter(filterOption)}
									className={`flex items-center gap-2 rounded-lg px-4 py-2 text-sm font-medium transition-all duration-200 ${
										filter === filterOption
											? "bg-card text-primary shadow-sm"
											: "text-muted-foreground hover:bg-card/50 hover:text-primary hover:cursor-pointer"
									}`}
								>
									{getFilterIcon(filterOption)}
									<span className="hidden sm:inline">
										{filterOption}
									</span>
								</button>
							))}
						</div>
					</div>

					{/* Posts Section */}
					{isLoading ? (
						<div className="space-y-6">
							<LoadingSkeleton count={4} />
						</div>
					) : isError ? (
						<div className="rounded-2xl border border-destructive/20 bg-card p-8 shadow-lg">
							<div className="text-center">
								<div className="mb-4 text-4xl text-destructive">
									⚠️
								</div>
								<div className="font-medium text-destructive">
									Unable to load posts
								</div>
								<p className="mt-2 text-sm text-muted-foreground">
									Please try refreshing the page
								</p>
							</div>
						</div>
					) : (
						<>
							<div className="space-y-6">
								{data?.pages.map(
									(
										page: PostsCursorPaginatedResponse,
										pageIndex: number
									) => {
										const sortedPosts = sortPosts(
											page.posts
										);

										return (
											<React.Fragment
												key={
													page.pageInfo?.endCursor ??
													`page-${pageIndex}`
												}
											>
												{sortedPosts.map(
													(post, index) => (
														<div
															key={post.id}
															ref={
																post.id ===
																startingPostId
																	? startingPostRef
																	: null
															}
															className="animate-fade-in"
															style={{
																animationDelay: `${
																	index * 50
																}ms`,
															}}
														>
															<PostCard
																post={post}
															/>
														</div>
													)
												)}
											</React.Fragment>
										);
									}
								)}

								{/* Load more trigger */}
								<div ref={loadMoreRef}>
									{isFetchingNextPage && (
										<div className="space-y-6">
											<LoadingSkeleton count={3} />
										</div>
									)}
								</div>
							</div>

							{/* Load more button (fallback) */}
							{!isFetchingNextPage && (
								<div className="flex justify-center pt-4">
									<button
										type="button"
										onClick={() => fetchNextPage()}
										disabled={
											!hasNextPage || isFetchingNextPage
										}
										className={`rounded-full px-4 py-2 font-medium transition-all duration-300 ${
											hasNextPage && !isFetchingNextPage
												? "bg-primary text-primary-foreground shadow-lg hover:bg-primary/90 hover:shadow-xl"
												: "cursor-not-allowed bg-muted text-muted-foreground"
										}`}
									>
										{isFetchingNextPage
											? "Loading more posts..."
											: hasNextPage
											? "Load more posts"
											: "🎉 You've reached the end!"}
									</button>
								</div>
							)}

							{/* Background fetching indicator */}
							{isFetching && !isFetchingNextPage && (
								<div className="space-y-6">
									<LoadingSkeleton count={2} />
								</div>
							)}
						</>
					)}
				</main>
			</div>
		</div>
	);
};

export default Feed;
