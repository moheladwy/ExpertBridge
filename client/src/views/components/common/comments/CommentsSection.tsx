import { useCreateCommentMutation, useDeleteCommentMutation, useGetCommentsByPostIdQuery } from "@/features/comments/commentsSlice";
import { AttachFile } from "@mui/icons-material";
import { Button, IconButton, TextField, Typography } from "@mui/material";
import { useEffect, useState } from "react";
import CommentCard from "./CommentCard";
import toast from "react-hot-toast";
import useRefetchOnLogin from "@/hooks/useRefetchOnLogin";
import useIsUserLoggedIn from "@/hooks/useIsUserLoggedIn";
import defaultProfile from "@/assets/Profile-pic/ProfilePic.svg"
import { useCurrentAuthUser } from "@/hooks/useCurrentAuthUser";
import { useAuthPrompt } from "@/contexts/AuthPromptContext";
import { Skeleton } from "@/views/components/ui/skeleton";
import { DeleteCommentRequest } from "@/features/comments/types";

interface CommentsSectionProps {
  postId: string;
}

const MAX_COMMENT_LENGTH = 5000;

const CommentsSection: React.FC<CommentsSectionProps> = ({ postId }) => {

  const authUser = useCurrentAuthUser();
  const { showAuthPrompt } = useAuthPrompt();
  const [, , , , userProfile] = useIsUserLoggedIn();

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
  const [deleteComment, deleteResult] = useDeleteCommentMutation();

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

    if (!authUser) {
      showAuthPrompt();
      return;
    }

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

  const handleAttachClick = (e: React.MouseEvent<HTMLLabelElement>) => {
    if (!authUser) {
      e.preventDefault();
      showAuthPrompt();
      return;
    }

  };

  const handleDeleteComment = async (request: DeleteCommentRequest) => {
    await deleteComment(request);
  };

  useEffect(() => {
    if (deleteResult.isSuccess) {
      toast.success('Comment deleted successfully.');
    }
    else if (deleteResult.isError) {
      toast.error('An error occurred while deleting your comment.');
    }
  }, [deleteResult.isSuccess, deleteResult.isError]);

  return (
    <div>
      {/* Add Comment Form */}
      <div className="mt-6">
        <form onSubmit={handleCommentSubmit} className="space-y-3">
          <div className="flex justify-center gap-3">
            {/* Profile Pic */}
            <div>
              {
                userProfile?.profilePictureUrl
                  ? <img
                    src={userProfile.profilePictureUrl}
                    width={45}
                    height={45}
                    className="rounded-full"
                  />
                  : <img
                    src={defaultProfile}
                    width={45}
                    height={45}
                    className="rounded-full"
                  />
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
                slotProps={{
                  htmlInput: {
                    maxLength: MAX_COMMENT_LENGTH,
                    dir: 'auto'
                  }
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
                <IconButton component="label" onClick={handleAttachClick}>
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

      {/* Comment List with Loading States */}
      {commentsLoading ? (
        // Skeleton UI for loading comments
        <div className="space-y-4">
          {[1, 2, 3].map((i) => (
            <div key={i} className="flex gap-3">
              <Skeleton className="h-10 w-10 rounded-full" />
              <div className="w-full">
                <Skeleton className="h-4 w-32 mb-2" />
                <Skeleton className="h-3 w-20 mb-3" />
                <Skeleton className="h-12 w-full" />
              </div>
            </div>
          ))}
        </div>
      ) : isCommentsError ? (
        <div className="p-4 rounded-md bg-gray-50 text-center">
          <p className="text-gray-500">Unable to load comments. Please try again later.</p>
        </div>
      ) : comments && comments.length > 0 ? (
        [...comments]
          .sort((a, b) => a.createdAt.localeCompare(b.createdAt))
          .map((comment) => (
            <CommentCard
              key={comment.id}
              comment={comment}
              currentUserId={userProfile?.id}
              onDelete={handleDeleteComment}
            />
          ))
      ) : (
        <div className="p-4 rounded-md bg-gray-50 text-center">
          <p className="text-gray-500">No comments yet. Be the first to share your thoughts!</p>
        </div>
      )}
    </div>
  );
}



export default CommentsSection;
