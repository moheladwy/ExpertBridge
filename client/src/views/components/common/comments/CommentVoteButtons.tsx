import {
	useDownvoteCommentMutation,
	useUpvoteCommentMutation,
} from "@/features/comments/commentsSlice";
import { Comment } from "@/features/comments/types";
import { ArrowBigUp } from "lucide-react";
import { useEffect, useState } from "react";
import toast from "react-hot-toast";
import { useCurrentAuthUser } from "@/hooks/useCurrentAuthUser";
import { useAuthPrompt } from "@/contexts/AuthPromptContext";

interface CommentVoteButtonsProps {
	comment: Comment;
}

const CommentVoteButtons: React.FC<CommentVoteButtonsProps> = ({ comment }) => {
	const authUser = useCurrentAuthUser(); // Now using singleton - no new subscription!
	const { showAuthPrompt } = useAuthPrompt();

	const [upvoteComment, upvoteResult] = useUpvoteCommentMutation();
	const [downvoteComment, downvoteResult] = useDownvoteCommentMutation();

	useEffect(() => {
		if (upvoteResult.isError || downvoteResult.isError) {
			toast.error("An error occurred while voting.");
		}

		setCommentVotes({
			upvotes: comment.upvotes,
			downvotes: comment.downvotes,
			userVote: comment.isUpvoted
				? "upvote"
				: comment.isDownvoted
					? "downvote"
					: (null as "upvote" | "downvote" | null),
		});
	}, [upvoteResult, downvoteResult, comment]);

	const [commentVotes, setCommentVotes] = useState({
		upvotes: comment.upvotes,
		downvotes: comment.downvotes,
		userVote: comment.isUpvoted
			? "upvote"
			: comment.isDownvoted
				? "downvote"
				: (null as "upvote" | "downvote" | null),
	});

	const voteDifference = comment.upvotes - comment.downvotes;

	const handleUpvote = async () => {
		if (!authUser) {
			showAuthPrompt();
			return;
		}
		await upvoteComment(comment);
	};

	const handleDownvote = async () => {
		if (!authUser) {
			showAuthPrompt();
			return;
		}
		await downvoteComment(comment);
	};

	return (
		<div className="flex gap-1 items-stretch bg-muted/50 rounded-full w-fit border border-border">
			<div
				className={`rounded-l-full p-1.5 hover:bg-green-100 hover:cursor-pointer transition-colors ${
					commentVotes.userVote === "upvote" ? "bg-green-100" : ""
				}`}
				onClick={handleUpvote}
			>
				<ArrowBigUp
					className={`w-5 h-5 ${
						commentVotes.userVote === "upvote"
							? "text-green-600 fill-green-600"
							: "text-muted-foreground hover:text-green-500"
					} transition-colors`}
				/>
			</div>

			<div
				className={`flex justify-center items-center text-sm font-semibold px-2 ${
					voteDifference > 0
						? "text-green-600"
						: voteDifference < 0
							? "text-red-600"
							: "text-muted-foreground"
				}`}
			>
				{voteDifference > 0 ? "+" : ""}
				{voteDifference}
			</div>

			<div
				className={`rounded-r-full p-1.5 rotate-180 hover:bg-red-100 hover:cursor-pointer transition-colors ${
					commentVotes.userVote === "downvote" ? "bg-red-100" : ""
				}`}
				onClick={handleDownvote}
			>
				<ArrowBigUp
					className={`w-5 h-5 ${
						commentVotes.userVote === "downvote"
							? "text-red-600 fill-red-600"
							: "text-muted-foreground hover:text-red-500"
					} transition-colors`}
				/>
			</div>
		</div>
	);
};

export default CommentVoteButtons;
