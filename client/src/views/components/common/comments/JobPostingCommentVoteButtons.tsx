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
		<div className="flex items-center space-x-4">
			{/* Upvote Button */}
			<button
				onClick={handleUpvote}
				className={`flex items-center space-x-1 px-2 py-1 rounded-lg transition-colors duration-200 ${
					comment.isUpvoted
						? "bg-green-100 dark:bg-green-900 text-green-700 dark:text-green-300"
						: "hover:bg-gray-100 dark:hover:bg-gray-700 text-gray-600 dark:text-gray-400"
				}`}
			>
				{comment.isUpvoted ? (
					<HandThumbUpIconSolid className="h-4 w-4" />
				) : (
					<HandThumbUpIcon className="h-4 w-4" />
				)}
				<span className="text-sm font-medium">{comment.upvotes}</span>
			</button>

			{/* Downvote Button */}
			<button
				onClick={handleDownvote}
				className={`flex items-center space-x-1 px-2 py-1 rounded-lg transition-colors duration-200 ${
					comment.isDownvoted
						? "bg-red-100 dark:bg-red-900 text-red-700 dark:text-red-300"
						: "hover:bg-gray-100 dark:hover:bg-gray-700 text-gray-600 dark:text-gray-400"
				}`}
			>
				{comment.isDownvoted ? (
					<HandThumbDownIconSolid className="h-4 w-4" />
				) : (
					<HandThumbDownIcon className="h-4 w-4" />
				)}
				<span className="text-sm font-medium">{comment.downvotes}</span>
			</button>
		</div>
	);
};

export default JobPostingCommentVoteButtons;
