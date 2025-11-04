import React from "react";
import { Comment } from "@/features/comments/types";
import {
	HandThumbUpIcon,
	HandThumbDownIcon,
} from "@heroicons/react/24/outline";
import {
	HandThumbUpIcon as HandThumbUpIconSolid,
	HandThumbDownIcon as HandThumbDownIconSolid,
} from "@heroicons/react/24/solid";
import {
	useUpvoteCommentMutation,
	useDownvoteCommentMutation,
} from "@/features/comments/commentsSlice";
import useIsUserLoggedIn from "@/hooks/useIsUserLoggedIn";
import { useAuthPrompt } from "@/contexts/AuthPromptContext";

interface JobCommentVoteButtonsProps {
	comment: Comment;
}

const JobPostingCommentVoteButtons: React.FC<JobCommentVoteButtonsProps> = ({
	comment,
}) => {
	const [isLoggedIn] = useIsUserLoggedIn();
	const { showAuthPrompt } = useAuthPrompt();
	const [upvoteComment] = useUpvoteCommentMutation();
	const [downvoteComment] = useDownvoteCommentMutation();

	const handleUpvote = async () => {
		if (!isLoggedIn) {
			showAuthPrompt();
			return;
		}

		try {
			await upvoteComment(comment).unwrap();
		} catch (error) {
			console.error("Failed to upvote comment:", error);
		}
	};

	const handleDownvote = async () => {
		if (!isLoggedIn) {
			showAuthPrompt();
			return;
		}

		try {
			await downvoteComment(comment).unwrap();
		} catch (error) {
			console.error("Failed to downvote comment:", error);
		}
	};

	return (
		<div className="flex items-center gap-2">
			{/* Upvote Button */}
			<button
				onClick={handleUpvote}
				className={`flex items-center gap-1.5 px-3 py-1.5 rounded-full border transition-all duration-200 ${
					comment.isUpvoted
						? "bg-green-100 text-green-700 border-green-200"
						: "bg-muted/50 hover:bg-green-50 text-muted-foreground hover:text-green-600 border-border hover:border-green-200"
				}`}
			>
				{comment.isUpvoted ? (
					<HandThumbUpIconSolid className="h-4 w-4" />
				) : (
					<HandThumbUpIcon className="h-4 w-4" />
				)}
				<span className="text-sm font-semibold">{comment.upvotes}</span>
			</button>

			{/* Downvote Button */}
			<button
				onClick={handleDownvote}
				className={`flex items-center gap-1.5 px-3 py-1.5 rounded-full border transition-all duration-200 ${
					comment.isDownvoted
						? "bg-red-100 text-red-700 border-red-200"
						: "bg-muted/50 hover:bg-red-50 text-muted-foreground hover:text-red-600 border-border hover:border-red-200"
				}`}
			>
				{comment.isDownvoted ? (
					<HandThumbDownIconSolid className="h-4 w-4" />
				) : (
					<HandThumbDownIcon className="h-4 w-4" />
				)}
				<span className="text-sm font-semibold">
					{comment.downvotes}
				</span>
			</button>
		</div>
	);
};

export default JobPostingCommentVoteButtons;
