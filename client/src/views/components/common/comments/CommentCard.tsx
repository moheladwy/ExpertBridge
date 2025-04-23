import React, { useEffect, useState } from "react";
import { Button, TextField, IconButton } from "@mui/material";
import { ThumbUp, ThumbDown } from "@mui/icons-material";
import { Comment } from "@/features/comments/types";
import { commentsApiSlice, useCreateReplyMutation } from "@/features/comments/commentsSlice";
import toast from "react-hot-toast";
import CommentVoteButtons from "./CommentVoteButtons";
import TimeAgo from "../../custom/TimeAgo";

interface CommentItemProps {
  comment: Comment;
}

const CommentCard: React.FC<CommentItemProps> = ({ comment }) => {
  const [showReplies, setShowReplies] = useState(false);
  const [replyText, setReplyText] = useState("");
  // const [replies, setReplies] = useState<Comment[]>(comment.replies || []);

  const [createReply, { isLoading, isSuccess, isError }] = useCreateReplyMutation();

  useEffect(() => {
    if (isError) toast.error("An error occurred while creating your reply");
    if (isSuccess) {
      toast.success("reply created successfully");
      // setReplies(comment.replies || []);
    }
  }, [isSuccess, isError, isLoading]);

  const handleReplySubmit = async () => {
    if (!replyText.trim()) return;

    await createReply({
      postId: comment.postId,
      content: replyText,
      parentCommentId: comment.id,
    });

    setReplyText("");
  };

  return (
    <div className="flex flex-col gap-3 p-3 border-t border-gray-300">
      {/* Comment Author */}
      <div className="flex items-center space-x-3">
        <img
          src={comment.author.profilePictureUrl || "./src/assets/default-avatar.png"}
          alt="Comment Author"
          width={30}
          height={30}
          className="rounded-full"
        />
        <div>
          {/* Name */}
          <h4 className="text-sm font-semibold">{comment.author.firstName + ' ' + comment.author.lastName}</h4>
          {/* Date of creation of the comment */}
          <p className="text-xs text-gray-500">
            <TimeAgo timestamp={comment.createdAt} /> 
          </p>
        </div>
      </div>

      {/* Comment Content */}
      <div className="w-full break-words">
        <p className="text-gray-700 whitespace-pre-wrap">{comment.content}</p>
      </div>

      {/* Comment Actions */}
      <div className="flex items-center space-x-3">
        <CommentVoteButtons comment={comment} />

        {comment.replies && comment.replies.length > 0 && (
          <Button size="small" onClick={() => setShowReplies((prev) => !prev)} className="text-blue-500">
            {showReplies ? "Hide Replies" : "Show Replies"}
          </Button>
        )}
      </div>

      {/* Reply Form */}
      <div>
        <TextField
          fullWidth
          multiline
          size="small"
          variant="outlined"
          placeholder="Write a reply..."
          value={replyText}
          onChange={(e) => setReplyText(e.target.value)}
          disabled={isLoading}
        />
        <div className="w-full flex mt-2 justify-end">
          <Button onClick={handleReplySubmit} variant="contained" size="small" className="bg-main-blue hover:bg-blue-950"
            disabled={isLoading}
          >
            Reply
          </Button>
        </div>
      </div>

      {/* Replies Section */}
      {showReplies &&
        (
          <div className=" flex flex-col gap-4 ml-4 border-l-2 border-gray-300 pl-3">
            {comment.replies?.map((reply) => (
              <div key={reply.id} className="">
                {/* Comment Author */}
                <div className="flex items-center space-x-3">
                  <img
                    src={reply.author.profilePictureUrl || "/default-avatar.png"}
                    alt="Reply Author"
                    width={30}
                    height={30}
                    className="rounded-full"
                  />
                  <div>
                    {/* Name */}
                    <h4 className="text-sm font-semibold">{reply.author.firstName + ' ' + reply.author.lastName}</h4>
                    {/* Date of creation of the comment */}
                    <p className="text-xs text-gray-500">
                      <TimeAgo timestamp={reply.createdAt} />
                    </p>
                  </div>
                </div>
                <div className="w-full break-words">
                  <p className="text-gray-700 whitespace-pre-wrap">{reply.content}</p>
                </div>
              </div>
            ))}
          </div>
        )}
    </div>
  );
};

export default CommentCard;
