"use client";

import { useEffect } from "react";
import { useRouter } from "next/navigation";
import { ArrowLeft, HelpCircle } from "lucide-react";
import toast from "react-hot-toast";

import { Button } from "@/components/ui/button";
import CreatePostForm from "@/features/posts/components/CreatePostForm";
import { useIsUserLoggedIn } from "@/hooks/useIsUserLoggedIn";
import { LoadingScreen } from "@/components/shared/LoadingScreen";
import { ErrorBoundary } from "@/components/shared/ErrorBoundary";

/**
 * Error fallback component for the create post page.
 */
function CreatePostErrorFallback() {
	const router = useRouter();

	useEffect(() => {
		toast.error(
			"Something went wrong while loading the Ask a Question page"
		);
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
 * Create post page - allows authenticated users to create new posts.
 * Redirects to signin if not authenticated.
 */
export default function CreatePostPage() {
	const router = useRouter();
	const { isAuthenticated, isLoading } = useIsUserLoggedIn();

	// Redirect to signin if not authenticated
	useEffect(() => {
		if (!isLoading && !isAuthenticated) {
			router.push("/auth/signin?redirect=/posts/create");
		}
	}, [isLoading, isAuthenticated, router]);

	// Show loading while checking auth
	if (isLoading) {
		return <LoadingScreen />;
	}

	// Don't render if not logged in (redirect will happen)
	if (!isAuthenticated) {
		return <LoadingScreen />;
	}

	return (
		<ErrorBoundary fallback={<CreatePostErrorFallback />}>
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
							<HelpCircle className="w-6 h-6 text-primary" />
						</div>
						<h1 className="text-3xl font-bold text-card-foreground">
							Ask a Question
						</h1>
					</div>
				</div>

				{/* Main Content */}
				<div className="max-w-4xl mx-auto py-3">
					<CreatePostForm />
				</div>
			</div>
		</ErrorBoundary>
	);
}
