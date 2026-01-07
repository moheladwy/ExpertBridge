"use client";

import { use, useEffect } from "react";
import { useRouter } from "next/navigation";
import { ArrowLeft, Edit as EditIcon } from "lucide-react";
import toast from "react-hot-toast";

import { Button } from "@/components/ui/button";
import EditPostForm from "@/features/posts/components/EditPostForm";
import { useGetPostQuery } from "@/features/posts/postsSlice";
import { useIsUserLoggedIn } from "@/hooks/useIsUserLoggedIn";
import { LoadingScreen } from "@/components/shared/LoadingScreen";
import { ErrorBoundary } from "@/components/shared/ErrorBoundary";

interface EditPostPageProps {
	params: Promise<{
		postId: string;
	}>;
}

/**
 * Loading skeleton for edit post page.
 */
function EditPostSkeleton() {
	return (
		<div className="w-full max-w-4xl mx-auto px-4 py-8">
			{/* Header skeleton */}
			<div className="mb-6">
				<div className="w-20 h-10 bg-muted rounded animate-pulse mb-4" />
				<div className="flex items-center gap-3">
					<div className="w-12 h-12 bg-muted rounded-full animate-pulse" />
					<div className="w-48 h-8 bg-muted rounded animate-pulse" />
				</div>
			</div>

			{/* Form skeleton */}
			<div className="bg-card rounded-lg border border-border p-6 shadow-sm animate-pulse">
				{/* Profile info */}
				<div className="flex items-center mb-6">
					<div className="w-12 h-12 bg-muted rounded-full mr-3" />
					<div className="space-y-2">
						<div className="w-32 h-4 bg-muted rounded" />
						<div className="w-24 h-3 bg-muted rounded" />
					</div>
				</div>

				<div className="h-px bg-muted mb-6" />

				{/* Title field */}
				<div className="space-y-2 mb-6">
					<div className="w-24 h-4 bg-muted rounded" />
					<div className="w-full h-12 bg-muted rounded" />
				</div>

				{/* Content field */}
				<div className="space-y-2 mb-6">
					<div className="w-32 h-4 bg-muted rounded" />
					<div className="w-full h-64 bg-muted rounded" />
				</div>

				{/* Buttons */}
				<div className="flex justify-center gap-3 pt-4">
					<div className="w-24 h-10 bg-muted rounded-full" />
					<div className="w-32 h-10 bg-muted rounded-full" />
				</div>
			</div>
		</div>
	);
}

/**
 * Error fallback component for the edit post page.
 */
function EditPostErrorFallback() {
	const router = useRouter();

	useEffect(() => {
		toast.error("Something went wrong while loading the Edit Question page");
	}, []);

	return (
		<div className="w-full max-w-4xl mx-auto px-4 py-8 text-center">
			<p className="text-muted-foreground mb-4">
				Something went wrong. Please try again.
			</p>
			<Button onClick={() => router.back()} variant="outline">
				Go Back
			</Button>
		</div>
	);
}

/**
 * Edit post page - allows authors to edit their own posts.
 * Includes authorization check and redirects if not owner.
 */
export default function EditPostPage({ params }: EditPostPageProps) {
	const { postId } = use(params);
	const router = useRouter();
	const {
		isAuthenticated,
		isLoading: isAuthLoading,
		profile,
	} = useIsUserLoggedIn();

	const {
		data: post,
		isLoading: isPostLoading,
		isError: isPostError,
	} = useGetPostQuery(postId, { skip: !postId });

	// Redirect to signin if not authenticated
	useEffect(() => {
		if (!isAuthLoading && !isAuthenticated) {
			router.push(`/auth/signin?redirect=/posts/${postId}/edit`);
		}
	}, [isAuthLoading, isAuthenticated, router, postId]);

	// Show loading while checking auth or loading post
	if (isAuthLoading || isPostLoading) {
		return <EditPostSkeleton />;
	}

	// Don't render if not logged in (redirect will happen)
	if (!isAuthenticated) {
		return <LoadingScreen />;
	}

	// Error state - post not found
	if (isPostError || !post) {
		return (
			<div className="min-h-screen bg-background flex items-center justify-center">
				<div className="text-center">
					<h2 className="text-2xl font-bold text-destructive mb-4">
						Post not found
					</h2>
					<p className="text-muted-foreground mb-4">
						The post you&apos;re trying to edit doesn&apos;t exist
						or has been deleted.
					</p>
					<Button onClick={() => router.push("/feed")}>
						Go to Feed
					</Button>
				</div>
			</div>
		);
	}

	// Authorization check - only author can edit
	if (post.author.id !== profile?.id) {
		return (
			<div className="min-h-screen bg-background flex items-center justify-center">
				<div className="text-center">
					<h2 className="text-2xl font-bold text-destructive mb-4">
						You don&apos;t have permission to edit this post
					</h2>
					<p className="text-muted-foreground mb-4">
						Only the author of this post can edit it.
					</p>
					<Button onClick={() => router.push("/feed")}>
						Go to Feed
					</Button>
				</div>
			</div>
		);
	}

	return (
		<ErrorBoundary fallback={<EditPostErrorFallback />}>
			<div className="w-full max-w-4xl mx-auto px-4 py-8">
				{/* Header */}
				<div className="mb-6">
					<Button
						variant="ghost"
						onClick={() => router.back()}
						className="mb-4 hover:bg-muted"
					>
						<ArrowLeft className="w-4 h-4 mr-2" />
						Back
					</Button>
					<div className="flex items-center gap-3">
						<div className="w-12 h-12 rounded-full bg-primary/10 flex items-center justify-center">
							<EditIcon className="w-6 h-6 text-primary" />
						</div>
						<h1 className="text-3xl font-bold text-card-foreground">
							Edit Question
						</h1>
					</div>
				</div>

				{/* Main Content */}
				<div className="py-3">
					<EditPostForm post={post} />
				</div>
			</div>
		</ErrorBoundary>
	);
}
