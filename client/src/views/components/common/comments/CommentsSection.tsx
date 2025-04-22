import { useCreateCommentMutation, useGetCommentsByPostIdQuery } from "@/features/comments/commentsSlice";
import { AttachFile } from "@mui/icons-material";
import { Button, IconButton, TextField } from "@mui/material";
import { useEffect, useState } from "react";
import CommentCard from "./CommentCard";
import toast from "react-hot-toast";
import useRefetchOnLogin from "@/hooks/useRefetchOnLogin";
import useIsUserLoggedIn from "@/hooks/useIsUserLoggedIn";

interface CommentsSectionProps {
  postId: string;
}

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
    <div>
      {/* <h3 className="text-lg font-semibold mb-3">Comments</h3> */}

      {/* Add Comment Form */}
      <div className="mt-6">
        {/* <h3 className="text-lg font-semibold mb-3">Leave a Comment</h3> */}
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
                onChange={(e) => setCommentText(e.target.value)}
                disabled={isLoading}
              />
              <div className="flex justify-end gap-2 items-center">
                <IconButton component="label">
                  <AttachFile />
                  <input type="file" hidden multiple onChange={(e) => setMedia([...media, ...(e.target.files || [])])} />
                </IconButton>

                <Button variant="contained" color="primary" type="submit"
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
            // onReplySubmit={(replyText) => console.log("New reply:", replyText)}
            />
          ))
      }
    </div >
  );
}



export default CommentsSection;
