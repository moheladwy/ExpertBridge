import { ArrowBigUp } from "lucide-react";
import { useEffect, useState } from "react";
import toast from "react-hot-toast";
import { useCurrentAuthUser } from "@/hooks/useCurrentAuthUser";
import { useAuthPrompt } from "@/contexts/AuthPromptContext";
import { JobPosting } from "@/features/jobPostings/types";
import {
	useDownvoteJobPostingMutation,
	useUpvoteJobPostingMutation,
} from "@/features/jobPostings/jobPostingsSlice";

interface JobPostingVoteButtonsProps {
	jobPosting: JobPosting;
}

const JobPostingVoteButtons: React.FC<JobPostingVoteButtonsProps> = ({
	jobPosting,
}) => {
	const authUser = useCurrentAuthUser();
	const { showAuthPrompt } = useAuthPrompt();

	const [upvoteJobPosting, upvoteResult] = useUpvoteJobPostingMutation();
	const [downvoteJobPosting, downvoteResult] =
		useDownvoteJobPostingMutation();

	useEffect(() => {
		if (upvoteResult.isError || downvoteResult.isError) {
			toast.error("An error occurred.");
		}

		setJobPostingVotes((prev) => ({
			...prev,
			userVote: jobPosting.isUpvoted
				? "upvote"
				: jobPosting.isDownvoted
					? "downvote"
					: null,
		}));
	}, [upvoteResult, downvoteResult, jobPosting]);

	const [jobPostingVotes, setJobPostingVotes] = useState({
		upvotes: jobPosting.upvotes,
		downvotes: jobPosting.downvotes,
		userVote: jobPosting.isUpvoted
			? "upvote"
			: jobPosting.isDownvoted
				? "downvote"
				: null,
	});

	const voteDifference = jobPosting.upvotes - jobPosting.downvotes;

	const handleUpvote = async () => {
		if (!authUser) {
			showAuthPrompt();
			return;
		}
		await upvoteJobPosting(jobPosting);
	};

	const handleDownvote = async () => {
		if (!authUser) {
			showAuthPrompt();
			return;
		}
		await downvoteJobPosting(jobPosting);
	};

	return (
		<div className="flex gap-2 items-stretch bg-gray-200 rounded-full w-fit">
			<div
				className={`rounded-l-full p-1 hover:bg-green-100 hover:cursor-pointer ${
					jobPostingVotes.userVote === "upvote" ? "bg-green-200" : ""
				}`}
				onClick={handleUpvote}
			>
				<ArrowBigUp
					className={`${
						jobPostingVotes.userVote === "upvote"
							? "text-green-600"
							: "text-gray-500 hover:text-green-400"
					}`}
				/>
			</div>

			<div
				className={`flex justify-center items-center text-sm font-bold ${
					voteDifference >= 0 ? "text-green-600" : "text-red-600"
				}`}
			>
				{voteDifference}
			</div>

			<div
				className={`rounded-l-full p-1 rotate-180 hover:bg-red-100 hover:cursor-pointer ${
					jobPostingVotes.userVote === "downvote" ? "bg-red-200" : ""
				}`}
				onClick={handleDownvote}
			>
				<ArrowBigUp
					className={`${
						jobPostingVotes.userVote === "downvote"
							? "text-red-600"
							: "text-gray-500 hover:text-red-400"
					}`}
				/>
			</div>
		</div>
	);
};

export default JobPostingVoteButtons;
