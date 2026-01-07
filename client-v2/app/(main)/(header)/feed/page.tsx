import FeedWithErrorBoundary from "@/features/posts/components/FeedWithErrorBoundary";

import type { Metadata } from "next";

export const metadata: Metadata = {
	title: "Feed | ExpertBridge",
	description:
		"Explore questions, answers, and discussions from experts and professionals.",
};

/**
 * Feed page - displays the main post feed with infinite scroll.
 * This is a server component that renders the client-side Feed component
 * wrapped in an ErrorBoundary for graceful error handling.
 */
export default function FeedPage() {
	return <FeedWithErrorBoundary />;
}
