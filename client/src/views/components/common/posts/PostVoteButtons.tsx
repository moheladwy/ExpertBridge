import {
	useDownvotePostMutation,
	useUpvotePostMutation,
} from "@/features/posts/postsSlice";
import { Post } from "@/features/posts/types";
import { ArrowBigUp } from "lucide-react";
import { useEffect, useState } from "react";
import toast from "react-hot-toast";
import { useCurrentAuthUser } from "@/hooks/useCurrentAuthUser";
import { useAuthPrompt } from "@/contexts/AuthPromptContext";

interface PostVoteButtonsProps {
	post: Post;
}

const PostVoteButtons: React.FC<PostVoteButtonsProps> = ({ post }) => {
	const authUser = useCurrentAuthUser(); // Now using singleton - no new subscription!
	const { showAuthPrompt } = useAuthPrompt();

	const [upvotePost, upvoteResult] = useUpvotePostMutation();
	const [downvotePost, downvoteResult] = useDownvotePostMutation();

	useEffect(() => {
		if (upvoteResult.isError || downvoteResult.isError) {
			toast.error("An error occurred.");
		}

		setPostVotes((prev) => ({
			...prev,
			userVote: post.isUpvoted
				? "upvote"
				: post.isDownvoted
					? "downvote"
					: null,
		}));
	}, [upvoteResult, downvoteResult, post]);

	const [postVotes, setPostVotes] = useState({
		upvotes: post.upvotes,
		downvotes: post.downvotes,
		userVote: post.isUpvoted
			? "upvote"
			: post.isDownvoted
				? "downvote"
				: null,
	});

	const voteDifference = post.upvotes - post.downvotes;

	const handleUpvote = async () => {
		if (!authUser) {
			showAuthPrompt();
			return;
		}
		await upvotePost(post);
	};

	const handleDownvote = async () => {
		if (!authUser) {
			showAuthPrompt();
			return;
		}
		await downvotePost(post);
	};

	return (
		<div className="flex gap-2 items-stretch bg-muted rounded-full w-fit">
			<div
				className={`rounded-l-full p-1 hover:bg-green-500/10 hover:cursor-pointer ${
					postVotes.userVote === "upvote" ? "bg-green-500/20" : ""
				}`}
				onClick={handleUpvote}
			>
				<ArrowBigUp
					className={`${
						postVotes.userVote === "upvote"
							? "text-green-600"
							: "text-muted-foreground hover:text-green-500"
					}`}
				/>
			</div>

			<div
				className={`flex justify-center items-center text-sm font-bold ${
					voteDifference >= 0 ? "text-green-600" : "text-destructive"
				}`}
			>
				{voteDifference}
			</div>

			<div
				className={`rounded-l-full p-1 rotate-180 hover:bg-destructive/10 hover:cursor-pointer ${
					postVotes.userVote === "downvote" ? "bg-destructive/20" : ""
				}`}
				onClick={handleDownvote}
			>
				<ArrowBigUp
					className={`${
						postVotes.userVote === "downvote"
							? "text-destructive"
							: "text-muted-foreground hover:text-destructive"
					}`}
				/>
			</div>
		</div>
	);
};

export default PostVoteButtons;
