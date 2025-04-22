import { useDownvoteCommentMutation, useUpvoteCommentMutation } from "@/features/comments/commentsSlice";
import { Comment } from "@/features/comments/types";
import { ArrowBigUp } from "lucide-react";
import { useEffect, useState } from "react";
import toast from "react-hot-toast";

interface CommentVoteButtonsProps {
  comment: Comment;
}

const CommentVoteButtons: React.FC<CommentVoteButtonsProps> = ({ comment }) => {
  const [upvoteComment, upvoteResult] = useUpvoteCommentMutation();
  const [downvoteComment, downvoteResult] = useDownvoteCommentMutation();

  useEffect(() => {
    if (upvoteResult.isError || downvoteResult.isError) {
      toast.error("An error occurred while voting.");
    }

    setCommentVotes({
      upvotes: comment.upvotes,
      downvotes: comment.downvotes,
      userVote: comment.isUpvoted ? "upvote" : comment.isDownvoted ? "downvote" : null as "upvote" | "downvote" | null,
    });

  }, [upvoteResult, downvoteResult, comment]);

  const [commentVotes, setCommentVotes] = useState({
    upvotes: comment.upvotes,
    downvotes: comment.downvotes,
    userVote: comment.isUpvoted ? "upvote" : comment.isDownvoted ? "downvote" : null as "upvote" | "downvote" | null,
  });

  const voteDifference = commentVotes.upvotes - commentVotes.downvotes;

  const handleUpvote = async () => {
    await upvoteComment(comment);
  };

  const handleDownvote = async () => {
    await downvoteComment(comment);
  };

  return (
    <div className="flex gap-2 items-stretch bg-gray-100 rounded-full w-fit">
      <div
        className={`rounded-l-full p-1 hover:bg-green-100 hover:cursor-pointer ${commentVotes.userVote === "upvote" ? "bg-green-200" : ""
          }`}
        onClick={handleUpvote}
      >
        <ArrowBigUp
          className={`${commentVotes.userVote === "upvote"
            ? "text-green-600"
            : "text-gray-500 hover:text-green-400"
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
        className={`rounded-l-full p-1 rotate-180 hover:bg-red-100 hover:cursor-pointer ${commentVotes.userVote === "downvote" ? "bg-red-200" : ""
          }`}
        onClick={handleDownvote}
      >
        <ArrowBigUp
          className={`${commentVotes.userVote === "downvote"
            ? "text-red-600"
            : "text-gray-500 hover:text-red-400"
            }`}
        />
      </div>
    </div>
  );
};

export default CommentVoteButtons;
