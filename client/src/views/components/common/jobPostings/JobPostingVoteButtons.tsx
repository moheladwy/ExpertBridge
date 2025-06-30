import { useDownvotePostMutation, useUpvotePostMutation } from "@/features/posts/postsSlice";
import { Post } from "@/features/posts/types";
import { ThumbDown, ThumbUp } from "@mui/icons-material";
import { IconButton } from "@mui/material";
import { ArrowBigUp } from "lucide-react";
import { useEffect, useState } from "react";
import toast from "react-hot-toast";
import { useCurrentAuthUser } from "@/hooks/useCurrentAuthUser";
import { useAuthPrompt } from "@/contexts/AuthPromptContext";
import { JobPosting } from "@/features/jobPostings/types";
import { useDownvoteJobPostingMutation, useUpvoteJobPostingMutation } from "@/features/jobPostings/jobPostingsSlice";

interface JobPostingVoteButtonsProps {
  jobPosting: JobPosting;
}

const JobPostingVoteButtons: React.FC<JobPostingVoteButtonsProps> = ({ jobPosting }) => {
  const authUser = useCurrentAuthUser();
  const { showAuthPrompt } = useAuthPrompt();

  const [upvoteJobPosting, upvoteResult] = useUpvoteJobPostingMutation();
  const [downvoteJobPosting, downvoteResult] = useDownvoteJobPostingMutation();

  useEffect(() => {
    if (upvoteResult.isError || downvoteResult.isError) {
      toast.error('An error occurred.');
    }

    setJobPostingVotes(prev => ({
      ...prev,
      userVote: jobPosting.isUpvoted ? "upvote" : jobPosting.isDownvoted ? "downvote" : null,
    }));

  }, [upvoteResult, downvoteResult, jobPosting]);

  const [jobPostingVotes, setJobPostingVotes] = useState({
    upvotes: jobPosting.upvotes,
    downvotes: jobPosting.downvotes,
    userVote: jobPosting.isUpvoted ? "upvote" : jobPosting.isDownvoted ? "downvote" : null,
  });


  const voteDifference = jobPosting.upvotes - jobPosting.downvotes;

  const handleUpvote = async () => {
    if (!authUser) {
      showAuthPrompt();
      return;
    }
    await upvoteJobPosting(jobPosting);

    // setjobPostingVotes((prev) => {
    //   const isUpvote = type === "upvote";
    //   const isDownvote = type === "downvote";

    //   if (prev.userVote === type) {
    //     return {
    //       upvotes: isUpvote ? prev.upvotes - 1 : prev.upvotes,
    //       downvotes: isDownvote ? prev.downvotes - 1 : prev.downvotes,
    //       userVote: null,
    //     };
    //   }

    //   return {
    //     upvotes: isUpvote ? prev.upvotes + 1 : prev.upvotes - (prev.userVote === "upvote" ? 1 : 0),
    //     downvotes: isDownvote ? prev.downvotes + 1 : prev.downvotes - (prev.userVote === "downvote" ? 1 : 0),
    //     userVote: type,
    //   };
    // });
  };

  const handleDownvote = async () => {
    if (!authUser) {
      showAuthPrompt();
      return;
    }
    await downvoteJobPosting(jobPosting);
  };

  return (
    // <div className="flex space-x-4 items-center">
    //   <IconButton
    //     color={jobPostingVotes.userVote === "upvote" ? "primary" : "default"}
    //     onClick={handlejobPostingUpvote}
    //   >
    //     <ThumbUp fontSize="small" />
    //   </IconButton>
    //   <span className="text-green-600">{jobPostingVotes.upvotes}</span>
    //   <IconButton
    //     color={jobPostingVotes.userVote === "downvote" ? "secondary" : "default"}
    //     onClick={handlejobPostingDownvote}
    //   >
    //     <ThumbDown fontSize="small" />
    //   </IconButton>
    //   <span className="text-red-600">{jobPostingVotes.downvotes}</span>
    // </div>
    <div className="flex gap-2 items-stretch bg-gray-200 rounded-full w-fit">
      <div
        className={`rounded-l-full p-1 hover:bg-green-100 hover:cursor-pointer ${jobPostingVotes.userVote === "upvote" ? "bg-green-200" : ""
          }`}
        onClick={handleUpvote}
      >
        <ArrowBigUp
          className={`${jobPostingVotes.userVote === "upvote" ? "text-green-600" : "text-gray-500 hover:text-green-400"
            }`}
        />
      </div>

      <div
        className={`flex justify-center items-center text-sm font-bold ${voteDifference >= 0 ? "text-green-600" : "text-red-600"
          }`}
      >
        {voteDifference}
      </div>

      <div
        className={`rounded-l-full p-1 rotate-180 hover:bg-red-100 hover:cursor-pointer ${jobPostingVotes.userVote === "downvote" ? "bg-red-200" : ""
          }`}
        onClick={handleDownvote}
      >
        <ArrowBigUp
          className={`${jobPostingVotes.userVote === "downvote" ? "text-red-600" : "text-gray-500 hover:text-red-400"
            }`}
        />
      </div>
    </div>
  );
}

export default JobPostingVoteButtons;
