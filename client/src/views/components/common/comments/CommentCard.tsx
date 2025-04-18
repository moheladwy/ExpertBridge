import React, { useEffect, useState } from "react";
import { Button, TextField, IconButton } from "@mui/material";
import { ThumbUp, ThumbDown } from "@mui/icons-material";
import { Comment } from "@/features/comments/types";
import { useCreateReplyMutation } from "@/features/comments/commentsSlice";
import toast from "react-hot-toast";
import CommentVoteButtons from "./CommentVoteButtons";

interface CommentItemProps {
  comment: Comment;
}

const CommentCard: React.FC<CommentItemProps> = ({ comment }) => {
  const [showReplies, setShowReplies] = useState(false);
  const [replyText, setReplyText] = useState("");
  const [replies, setReplies] = useState<Comment[]>(comment.replies || []);

  const [createReply, { isLoading, isSuccess, isError }] = useCreateReplyMutation();

  useEffect(() => {
    if (isError) toast.error("An error occurred while creating your reply");
    if (isSuccess) {
      toast.success("reply created successfully");
      setReplies(comment.replies || []);
    }
  }, [isSuccess, isError]);

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
    <div className="p-3 border-t border-gray-300">
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
          <h4 className="text-sm font-semibold">{comment.author.firstName + ' ' + comment.author.lastName}</h4>
          <p className="text-xs text-gray-500">{comment.author.jobTitle || "No job title"}</p>
        </div>
      </div>

      {/* Comment Content */}
      <p className="text-gray-700 mt-2">{comment.content}</p>

      {/* Comment Actions */}
      <div className="flex items-center mt-2 space-x-3">
        <CommentVoteButtons comment={comment} />

        {comment.replies && comment.replies.length > 0 && (
          <Button size="small" onClick={() => setShowReplies((prev) => !prev)} className="text-blue-500">
            {showReplies ? "Hide Replies" : "Show Replies"}
          </Button>
        )}
      </div>

      {/* Reply Form */}
      <div className="mt-3">
        <TextField
          fullWidth
          size="small"
          variant="outlined"
          placeholder="Write a reply..."
          value={replyText}
          onChange={(e) => setReplyText(e.target.value)}
          disabled={isLoading}
        />
        <Button onClick={handleReplySubmit} variant="contained" size="small" className="mt-2"
          disabled={isLoading}
        >
          Reply
        </Button>
      </div>

      {/* Replies Section */}
      {showReplies && replies.length > 0 && (
        <div className="ml-6 mt-3 border-l-2 border-gray-300 pl-3">
          {replies.map((reply) => (
            <div key={reply.id} className="mt-2">
              <div className="flex items-center space-x-3">
                <img
                  src={reply.author.profilePictureUrl || "/default-avatar.png"}
                  alt="Reply Author"
                  width={25}
                  height={25}
                  className="rounded-full"
                />
                <div>
                  <h5 className="text-xs font-semibold">{reply.author.firstName + ' ' + reply.author.lastName}</h5>
                </div>
              </div>
              <p className="text-gray-700 mt-1">{reply.content}</p>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

export default CommentCard;
