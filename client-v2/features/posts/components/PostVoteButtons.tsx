"use client";

import { useEffect } from "react";
import { ArrowBigUp } from "lucide-react";
import toast from "react-hot-toast";

import {
	useUpvotePostMutation,
	useDownvotePostMutation,
} from "@/features/posts/postsSlice";
import type { Post } from "@/features/posts/types";
import { useAuth } from "@/lib/firebase/AuthProvider";
import { useAuthPrompt } from "@/lib/contexts/AuthPromptContext";

interface PostVoteButtonsProps {
	post: Post;
}

/**
 * Voting buttons for posts with upvote/downvote functionality.
 * Shows net vote count and highlights user's current vote.
 * Prompts unauthenticated users to sign in.
 */
const PostVoteButtons = ({ post }: PostVoteButtonsProps) => {
	const { user } = useAuth();
	const { showAuthPrompt } = useAuthPrompt();

	const [upvotePost, upvoteResult] = useUpvotePostMutation();
	const [downvotePost, downvoteResult] = useDownvotePostMutation();

	// Derive vote state directly from post data (updated via optimistic updates)
	const userVote: "upvote" | "downvote" | null = post.isUpvoted
		? "upvote"
		: post.isDownvoted
		? "downvote"
		: null;

	// Show error toast on mutation failure (triggers once per new error)
	useEffect(() => {
		if (upvoteResult.error) {
			toast.error("An error occurred while voting.");
		}
	}, [upvoteResult.error]);

	useEffect(() => {
		if (downvoteResult.error) {
			toast.error("An error occurred while voting.");
		}
	}, [downvoteResult.error]);

	const voteDifference = post.upvotes - post.downvotes;

	const handleUpvote = async () => {
		if (!user) {
			showAuthPrompt();
			return;
		}
		await upvotePost(post);
	};

	const handleDownvote = async () => {
		if (!user) {
			showAuthPrompt();
			return;
		}
		await downvotePost(post);
	};

	return (
		<div className="flex items-stretch gap-2 rounded-full bg-muted w-fit">
			{/* Upvote button */}
			<button
				type="button"
				className={`rounded-l-full p-1 transition-colors hover:cursor-pointer hover:bg-green-500/10 ${
					userVote === "upvote" ? "bg-green-500/20" : ""
				}`}
				onClick={handleUpvote}
				aria-label="Upvote"
			>
				<ArrowBigUp
					className={`h-6 w-6 ${
						userVote === "upvote"
							? "text-green-600"
							: "text-muted-foreground hover:text-green-500"
					}`}
				/>
			</button>

			{/* Vote count */}
			<div
				className={`flex items-center justify-center text-sm font-bold ${
					voteDifference >= 0 ? "text-green-600" : "text-destructive"
				}`}
			>
				{voteDifference}
			</div>

			{/* Downvote button */}
			<button
				type="button"
				className={`rounded-r-full p-1 rotate-180 transition-colors hover:cursor-pointer hover:bg-destructive/10 ${
					userVote === "downvote" ? "bg-destructive/20" : ""
				}`}
				onClick={handleDownvote}
				aria-label="Downvote"
			>
				<ArrowBigUp
					className={`h-6 w-6 ${
						userVote === "downvote"
							? "text-destructive"
							: "text-muted-foreground hover:text-destructive"
					}`}
				/>
			</button>
		</div>
	);
};

export default PostVoteButtons;
