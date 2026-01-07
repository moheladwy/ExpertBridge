"use client";

import { use, useEffect } from "react";
import { useRouter } from "next/navigation";

import { useGetPostQuery } from "@/features/posts/postsSlice";
import FullPostWithComments from "@/features/posts/components/FullPostWithComments";
import { ErrorBoundary } from "@/components/shared/ErrorBoundary";
import { AlertTriangle, RefreshCw } from "lucide-react";
import { Button } from "@/components/ui/button";
import {
	Card,
	CardContent,
	CardDescription,
	CardFooter,
	CardHeader,
	CardTitle,
} from "@/components/ui/card";
import router from "next/router";

interface PostPageProps {
	params: Promise<{
		postId: string;
	}>;
}

/**
 * Error fallback component for post page errors.
 */
function PostErrorFallback() {
	const handleRetry = () => {
		window.location.reload();
	};

	return (
		<div className="flex min-h-100 items-center justify-center p-4">
			<Card className="w-full max-w-md">
				<CardHeader className="text-center">
					<div className="mx-auto mb-4 flex h-12 w-12 items-center justify-center rounded-full bg-destructive/10">
						<AlertTriangle className="h-6 w-6 text-destructive" />
					</div>
					<CardTitle>Failed to load post</CardTitle>
					<CardDescription>
						Something went wrong while loading the post. Please try
						again.
					</CardDescription>
				</CardHeader>
				<CardContent className="text-center">
					<p className="text-sm text-muted-foreground">
						This could be due to a network issue or the post may no
						longer exist.
					</p>
				</CardContent>
				<CardFooter className="flex justify-center gap-4">
					<Button
						variant="outline"
						onClick={() => router.push("/feed")}
					>
						Back to Feed
					</Button>
					<Button onClick={handleRetry}>
						<RefreshCw className="mr-2 h-4 w-4" />
						Try Again
					</Button>
				</CardFooter>
			</Card>
		</div>
	);
}

/**
 * Loading skeleton for post page.
 */
function PostPageSkeleton() {
	return (
		<div className="w-full flex justify-center">
			<div className="w-5/6 mx-auto py-4 flex gap-3 max-lg:flex-col max-sm:w-full">
				{/* Main Post Content - Left Side */}
				<div className="w-5/6 max-lg:w-full">
					<div className="bg-card shadow-md rounded-lg p-4 border border-border animate-pulse">
						{/* Header */}
						<div className="flex items-center justify-between pb-3 border-b border-border">
							<div className="w-6 h-6 bg-muted rounded-full" />
							<div className="w-6 h-6 bg-muted rounded-full" />
						</div>

						{/* Author Info */}
						<div className="flex items-center space-x-3 mt-4">
							<div className="w-10 h-10 bg-muted rounded-full" />
							<div className="space-y-2">
								<div className="w-32 h-4 bg-muted rounded" />
								<div className="w-20 h-3 bg-muted rounded" />
							</div>
						</div>

						{/* Title */}
						<div className="mt-4 w-3/4 h-6 bg-muted rounded" />

						{/* Content */}
						<div className="mt-4 space-y-2">
							<div className="w-full h-4 bg-muted rounded" />
							<div className="w-full h-4 bg-muted rounded" />
							<div className="w-2/3 h-4 bg-muted rounded" />
						</div>

						{/* Tags */}
						<div className="mt-4 flex gap-2">
							<div className="w-16 h-6 bg-muted rounded-full" />
							<div className="w-20 h-6 bg-muted rounded-full" />
							<div className="w-14 h-6 bg-muted rounded-full" />
						</div>

						{/* Vote buttons */}
						<div className="mt-4 flex gap-4">
							<div className="w-20 h-8 bg-muted rounded" />
							<div className="w-20 h-8 bg-muted rounded" />
						</div>
					</div>
				</div>

				{/* Similar Posts - Right Side */}
				<div className="w-1/6 max-lg:w-full">
					<div className="bg-card shadow-md rounded-lg p-4 border border-border animate-pulse">
						<div className="w-3/4 h-6 bg-muted rounded mb-4" />
						<div className="h-32 bg-muted rounded" />
					</div>
				</div>
			</div>
		</div>
	);
}

/**
 * Inner post content component.
 * Separated to allow ErrorBoundary reset on postId change.
 */
function PostContent({ postId }: { postId: string }) {
	const router = useRouter();
	const {
		data: post,
		isLoading,
		error,
	} = useGetPostQuery(postId, {
		skip: !postId || postId.trim() === "",
	});

	// Redirect to feed if postId is invalid or post is missing (after loading)
	useEffect(() => {
		if (!postId || postId.trim() === "") {
			router.replace("/feed");
			return;
		}
		if (!isLoading && !error && !post) {
			router.replace("/feed");
		}
	}, [postId, isLoading, error, post, router]);

	// Show loading state
	if (isLoading) {
		return <PostPageSkeleton />;
	}

	// Handle errors
	if (error) {
		throw error;
	}

	// Handle missing post (show skeleton while redirecting)
	if (!post) {
		return <PostPageSkeleton />;
	}

	return <FullPostWithComments post={post} />;
}

/**
 * Single post page with full details and comments.
 * Uses dynamic route to fetch post by ID.
 */
export default function PostPage({ params }: PostPageProps) {
	const { postId } = use(params);

	return (
		<ErrorBoundary fallback={<PostErrorFallback />}>
			<PostContent postId={postId} />
		</ErrorBoundary>
	);
}
