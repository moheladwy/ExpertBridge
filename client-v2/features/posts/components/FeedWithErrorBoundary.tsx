"use client";

import { ErrorBoundary } from "@/components/shared/ErrorBoundary";
import Feed from "./Feed";
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

/**
 * Custom fallback UI for feed errors.
 * Displays a user-friendly message when the feed fails to load.
 */
function FeedErrorFallback() {
	const handleRetry = () => {
		// Reload the page to reset the error boundary state
		window.location.reload();
	};

	return (
		<div className="flex min-h-100 items-center justify-center p-4">
			<Card className="w-full max-w-md">
				<CardHeader className="text-center">
					<div className="mx-auto mb-4 flex h-12 w-12 items-center justify-center rounded-full bg-destructive/10">
						<AlertTriangle className="h-6 w-6 text-destructive" />
					</div>
					<CardTitle>Something went wrong loading the feed</CardTitle>
					<CardDescription>
						We encountered an unexpected error while loading your
						feed. Please try again.
					</CardDescription>
				</CardHeader>
				<CardContent>
					<p className="text-sm text-muted-foreground text-center">
						If this problem persists, please refresh the page or try
						again later.
					</p>
				</CardContent>
				<CardFooter className="flex justify-center">
					<Button onClick={handleRetry} variant="outline">
						<RefreshCw className="mr-2 h-4 w-4" />
						Retry
					</Button>
				</CardFooter>
			</Card>
		</div>
	);
}

/**
 * Feed component wrapped in an ErrorBoundary for graceful error handling.
 * Catches runtime errors in the Feed component and displays a user-friendly fallback UI.
 */
export default function FeedWithErrorBoundary() {
	return (
		<ErrorBoundary fallback={<FeedErrorFallback />}>
			<Feed />
		</ErrorBoundary>
	);
}
