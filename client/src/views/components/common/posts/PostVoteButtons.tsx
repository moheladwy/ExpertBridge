import { useDownvotePostMutation, useUpvotePostMutation } from "@/features/posts/postsSlice";
import { Post } from "@/features/posts/types";
import { ThumbDown, ThumbUp } from "@mui/icons-material";
import { IconButton } from "@mui/material";
import { ArrowBigUp } from "lucide-react";
import { useEffect, useState } from "react";
import toast from "react-hot-toast";
import { useCurrentAuthUser } from "@/hooks/useCurrentAuthUser";
import { useAuthPrompt } from "@/contexts/AuthPromptContext";

interface PostVoteButtonsProps {
  post: Post;
}

const PostVoteButtons: React.FC<PostVoteButtonsProps> = ({ post }) => {

  const authUser = useCurrentAuthUser();
  const { showAuthPrompt } = useAuthPrompt();

  const [upvotePost, upvoteResult] = useUpvotePostMutation();
  const [downvotePost, downvoteResult] = useDownvotePostMutation();

  useEffect(() => {
    if (upvoteResult.isError || downvoteResult.isError) {
      toast.error('An error occurred.');
    }

    setPostVotes(prev => ({
      ...prev,
      userVote: post.isUpvoted ? "upvote" : post.isDownvoted ? "downvote" : null,
    }));

  }, [upvoteResult, downvoteResult, post]);

  const [postVotes, setPostVotes] = useState({
    upvotes: post.upvotes,
    downvotes: post.downvotes,
    userVote: post.isUpvoted ? "upvote" : post.isDownvoted ? "downvote" : null,
  });


  const voteDifference = post.upvotes - post.downvotes;

  const handleUpvote = async () => {
    if (!authUser) {
      showAuthPrompt();
      return;
    }
    await upvotePost(post);

    // setPostVotes((prev) => {
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
    if (!authUser){
      showAuthPrompt();
      return;
    }
    await downvotePost(post);
  };

  return (
    // <div className="flex space-x-4 items-center">
    //   <IconButton
    //     color={postVotes.userVote === "upvote" ? "primary" : "default"}
    //     onClick={handlePostUpvote}
    //   >
    //     <ThumbUp fontSize="small" />
    //   </IconButton>
    //   <span className="text-green-600">{postVotes.upvotes}</span>
    //   <IconButton
    //     color={postVotes.userVote === "downvote" ? "secondary" : "default"}
    //     onClick={handlePostDownvote}
    //   >
    //     <ThumbDown fontSize="small" />
    //   </IconButton>
    //   <span className="text-red-600">{postVotes.downvotes}</span>
    // </div>
    <div className="flex gap-2 items-stretch bg-gray-200 rounded-full w-fit">
      <div
        className={`rounded-l-full p-1 hover:bg-green-100 hover:cursor-pointer ${postVotes.userVote === "upvote" ? "bg-green-200" : ""
          }`}
        onClick={handleUpvote}
      >
        <ArrowBigUp
          className={`${postVotes.userVote === "upvote" ? "text-green-600" : "text-gray-500 hover:text-green-400"
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
        className={`rounded-l-full p-1 rotate-180 hover:bg-red-100 hover:cursor-pointer ${postVotes.userVote === "downvote" ? "bg-red-200" : ""
          }`}
        onClick={handleDownvote}
      >
        <ArrowBigUp
          className={`${postVotes.userVote === "downvote" ? "text-red-600" : "text-gray-500 hover:text-red-400"
            }`}
        />
      </div>
    </div>
  );
}

export default PostVoteButtons;
