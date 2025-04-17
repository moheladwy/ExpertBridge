import { useCreateCommentMutation, useGetCommentsByPostIdQuery } from "@/features/comments/commentsSlice";
import { AttachFile } from "@mui/icons-material";
import { Button, IconButton, TextField } from "@mui/material";
import { useEffect, useState } from "react";
import CommentCard from "./CommentCard";
import toast from "react-hot-toast";


interface CommentsSectionProps {
  postId: string;
}

const CommentsSection: React.FC<CommentsSectionProps> = ({ postId }) => {
  const {
    data: comments,
    isLoading: commentsLoading,
    isError: isCommentsError,
    error: commentsError,
    isSuccess: commentsSuccess,
  } = useGetCommentsByPostIdQuery(postId);

  const [createComment, { isLoading, isSuccess, isError }] = useCreateCommentMutation();

  useEffect(() => {
    if (isError) toast.error("An error occurred while creating your comment");
    if (isSuccess) {
      toast.success("comment created successfully");
    }
  }, [isSuccess, isError]);

  // form data
  const [commentText, setCommentText] = useState("");
  const [media, setMedia] = useState<File[]>([]);

  useEffect(() => {
    if (commentsSuccess) {
      // post.comments = Object.values(comments.entities);
    }
  }, [commentsSuccess, comments]);


  if (commentsLoading) {
    return <p>Loading...</p>;
  }

  const handleCommentSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    console.log("New comment:", commentText, media);

    await createComment({
      postId,
      content: commentText
    });

    setCommentText("");
    setMedia([]);
  };

  return (
    < div className="mt-6" >
      <h3 className="text-lg font-semibold mb-3">Comments</h3>

      {/* Add Comment Form */}
      <div className="mt-6">
        <h3 className="text-lg font-semibold mb-3">Leave a Comment</h3>
        <form onSubmit={handleCommentSubmit} className="space-y-3">
          <TextField
            fullWidth
            multiline
            rows={3}
            variant="outlined"
            label="Write a comment..."
            value={commentText}
            onChange={(e) => setCommentText(e.target.value)}
            disabled={isLoading}
          />
          <div className="flex justify-between items-center">
            <Button variant="contained" color="primary" type="submit"
              disabled={isLoading}
            >
              Post Comment
            </Button>
            <IconButton component="label">
              <AttachFile />
              <input type="file" hidden multiple onChange={(e) => setMedia([...media, ...(e.target.files || [])])} />
            </IconButton>
          </div>
        </form>
      </div>

      {/* Comment List */}
      {
        comments &&
        [...comments]
          .sort((a, b) => a.createdAt.localeCompare(b.createdAt))
          .map((comment) => (
          <CommentCard
            key={comment.id}
            comment={comment}
          // onReplySubmit={(replyText) => console.log("New reply:", replyText)}
          />
        ))
      }
    </div >
  );
}



export default CommentsSection;
