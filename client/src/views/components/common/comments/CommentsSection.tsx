import { useCreateCommentMutation, useGetCommentsByPostIdQuery } from "@/features/comments/commentsSlice";
import { AttachFile } from "@mui/icons-material";
import { Button, IconButton, TextField, Typography } from "@mui/material";
import { useEffect, useState } from "react";
import CommentCard from "./CommentCard";
import toast from "react-hot-toast";
import useRefetchOnLogin from "@/hooks/useRefetchOnLogin";
import useIsUserLoggedIn from "@/hooks/useIsUserLoggedIn";

interface CommentsSectionProps {
  postId: string;
}

const MAX_COMMENT_LENGTH = 5000;

const CommentsSection: React.FC<CommentsSectionProps> = ({ postId }) => {

  const [,,, authUser, userProfile] = useIsUserLoggedIn();

  const {
    data: comments,
    isLoading: commentsLoading,
    isError: isCommentsError,
    error: commentsError,
    isSuccess: commentsSuccess,
    refetch,
  } = useGetCommentsByPostIdQuery(postId);

  useRefetchOnLogin(refetch);

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
  const [commentError, setCommentError] = useState(""); // New state for error tracking
  
  useEffect(() => {
    if (commentsSuccess) {
      // post.comments = Object.values(comments.entities);
    }
  }, [commentsSuccess, comments]);


  if (commentsLoading) {
    return <p>Loading...</p>;
  }
  
  // Calculate characters left
  const charsLeft = MAX_COMMENT_LENGTH - commentText.length;
  
  const handleCommentChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const newValue = e.target.value;
    if (newValue.length <= MAX_COMMENT_LENGTH) {
      setCommentText(newValue);
      if (newValue.trim()) {
        setCommentError(""); // Clear error when user types valid content
      }
    }
  };

  const handleCommentSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    // Validate comment text
    if (!commentText.trim()) {
      setCommentError("Comment cannot be empty");
      return;
    }
    console.log("New comment:", commentText, media);
    await createComment({
      postId,
      content: commentText
    });
    setCommentText("");
    setMedia([]);
    setCommentError(""); // Clear error on successful submission
  };

  return (
    <div>
      {/* Add Comment Form */}
      <div className="mt-6">
        <form onSubmit={handleCommentSubmit} className="space-y-3">
          <div className="flex justify-center gap-3">
            {/* Profile Pic */}
            <div>
              {/* using the name's first letter as a profile */}
              {
                userProfile?.profilePictureUrl
                  ? <img
                    src={userProfile.profilePictureUrl}
                    width={45}
                    height={45}
                    className="rounded-full"
                  />
                  : <h1 className="text-main-blue font-bold text-lg ">{authUser?.displayName?.charAt(0).toUpperCase()}</h1>
              }
            </div>

            <div className="flex flex-col gap-3 w-full">  
              {/* Comment Text Field */}
              <TextField
                fullWidth
                multiline
                size="small"
                variant="outlined"
                placeholder="Add a comment..."
                value={commentText}
                inputProps={{
                  maxLength: MAX_COMMENT_LENGTH,
                }}
                onChange={handleCommentChange}
                disabled={isLoading}
                error={!!commentError}
                helperText={commentError}
              />
              
              {/* Character counter */}
              {!commentError && (
                <div className="flex justify-end">
                  <Typography 
                    variant="caption" 
                    color={charsLeft === 0 ? "error" : "text.secondary"}
                  >
                    {charsLeft} characters left
                  </Typography>
                </div>
              )}
              
              <div className="flex justify-end gap-2 items-center">
                <IconButton component="label">
                  <AttachFile />
                  <input type="file" hidden multiple onChange={(e) => setMedia([...media, ...(e.target.files || [])])} />
                </IconButton>

                <Button 
                  variant="contained" 
                  color="primary" 
                  type="submit"
                  disabled={isLoading}
                  className="bg-main-blue hover:bg-blue-950"
                >
                  Add Comment
                </Button>
              </div>
            </div>
          </div>
        </form>
      </div>

      <div className="flex items-center justify-between font-semibold text-lg my-3">
        <h2>Comments</h2>
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
            />
          ))
      }
    </div>
  );
}



export default CommentsSection;
