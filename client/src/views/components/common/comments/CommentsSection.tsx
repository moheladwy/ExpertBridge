import {
  useCreateCommentMutation,
  useDeleteCommentMutation,
  useGetCommentsByPostIdQuery,
} from "@/features/comments/commentsSlice";
import { AttachFile, Sort } from "@mui/icons-material";
import {
  Button,
  IconButton,
  TextField,
  Typography,
  Menu,
  MenuItem,
  ListItemText,
} from "@mui/material";
import { useEffect, useState } from "react";
import CommentCard from "./CommentCard";
import toast from "react-hot-toast";
import useRefetchOnLogin from "@/hooks/useRefetchOnLogin";
import useIsUserLoggedIn from "@/hooks/useIsUserLoggedIn";
import defaultProfile from "@/assets/Profile-pic/ProfilePic.svg";
import { useCurrentAuthUser } from "@/hooks/useCurrentAuthUser";
import { useAuthPrompt } from "@/contexts/AuthPromptContext";
import { Skeleton } from "@/views/components/ui/skeleton";
import { DeleteCommentRequest } from "@/features/comments/types";
import FileUploadForm from "../../custom/FileUploadForm";
import { MediaObject } from "@/features/media/types";
import useCallbackOnMediaUploadSuccess from "@/hooks/useCallbackOnMediaUploadSuccess";

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
    error: _commentsFetchError,
    isSuccess: commentsSuccess,
    refetch,
  } = useGetCommentsByPostIdQuery(postId);

  const [createComment, { isLoading, isSuccess, isError }] =
    useCreateCommentMutation();
  const [deleteComment, deleteResult] = useDeleteCommentMutation();

  useRefetchOnLogin(refetch);

  // form data
  const [commentText, setCommentText] = useState("");
  const [media, setMedia] = useState<File[]>([]);
  const [commentError, setCommentError] = useState(""); // New state for error tracking
  const [showMediaForm, setShowMediaForm] = useState(false);
  const [mediaList, setMediaList] = useState<MediaObject[]>([]);
  const [sortOption, setSortOption] = useState<
    "newest" | "oldest" | "mostUpvoted" | "mostReplies"
  >("oldest");
  const [anchorEl, setAnchorEl] = useState<null | HTMLElement>(null);

  // Map sort options to display names
  const sortOptionLabels = {
    newest: "Newest",
    oldest: "Oldest",
    mostUpvoted: "Most upvoted",
    mostReplies: "Most replies",
  };

  useEffect(() => {
    if (isError) toast.error("An error occurred while creating your comment");
    if (isSuccess) {
      toast.success("Comment created successfully");
      setCommentText("");
      setShowMediaForm(false);
    }
  }, [isSuccess, isError]);

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

  const { uploadMedia, uploadResult } = useCallbackOnMediaUploadSuccess(
    createComment,
    { postId, content: commentText },
  );

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

    await uploadMedia({ mediaList });
    // await createComment({
    //   postId,
    //   content: commentText
    // });
    setMedia([]);
    setCommentError(""); // Clear error on successful submission
  };

  const handleDeleteComment = async (request: DeleteCommentRequest) => {
    await deleteComment(request);
  };

  useEffect(() => {
    if (deleteResult.isSuccess) {
      toast.success("Comment deleted successfully.");
    } else if (deleteResult.isError) {
      toast.error("An error occurred while deleting your comment.");
    }
  }, [deleteResult.isSuccess, deleteResult.isError]);

  const handleAttachClick = (e: React.MouseEvent<HTMLLabelElement>) => {
    e.stopPropagation();
    e.preventDefault();
    if (!authUser) {
      showAuthPrompt();
      return;
    }
    setShowMediaForm((prev) => !prev);
  };

  const handleSortMenuOpen = (event: React.MouseEvent<HTMLButtonElement>) => {
    setAnchorEl(event.currentTarget);
  };

  const handleSortMenuClose = () => {
    setAnchorEl(null);
  };

  const handleSortChange = (
    option: "newest" | "oldest" | "mostUpvoted" | "mostReplies",
  ) => {
    setSortOption(option);
    handleSortMenuClose();
  };

  const getSortedComments = () => {
    if (!comments || comments.length === 0) return [];

    const sortedComments = [...comments];

    switch (sortOption) {
      case "newest":
        return sortedComments.sort((a, b) =>
          b.createdAt.localeCompare(a.createdAt),
        );
      case "oldest":
        return sortedComments.sort((a, b) =>
          a.createdAt.localeCompare(b.createdAt),
        );
      case "mostUpvoted":
        return sortedComments.sort(
          (a, b) => b.upvotes - b.downvotes - (a.upvotes - a.downvotes),
        );
      case "mostReplies":
        return sortedComments.sort(
          (a, b) => (b.replies?.length || 0) - (a.replies?.length || 0),
        );
      default:
        return sortedComments;
    }
  };

  return (
    <div className="dark:text-gray-200">
      {/* Add Comment Form */}
      <div className="mt-6">
        <div onSubmit={handleCommentSubmit} className="space-y-3">
          <div className="flex justify-center gap-3">
            {/* Profile Pic */}
            <div>
              {userProfile?.profilePictureUrl ? (
                <img
                  src={userProfile.profilePictureUrl}
                  width={45}
                  height={45}
                  className="rounded-full"
                />
              ) : (
                <img
                  src={defaultProfile}
                  width={45}
                  height={45}
                  className="rounded-full"
                />
              )}
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
                    dir: "auto",
                  },
                }}
                onChange={handleCommentChange}
                disabled={isLoading || uploadResult.isLoading}
                error={!!commentError}
                helperText={commentError}
                className="dark:text-white [&_.MuiOutlinedInput-root]:dark:text-white [&_.MuiInputBase-input]:dark:text-white [&_.MuiInputBase-input]::placeholder:dark:text-gray-500 [&_.MuiOutlinedInput-notchedOutline]:dark:border-gray-600"
                inputProps={{
                  className: "dark:text-white placeholder:dark:text-gray-500",
                }}
                sx={{
                  "& .MuiInputBase-input": {
                    "&::placeholder": {
                      color:
                        "var(--tw-text-opacity: 1); color: rgb(107 114 128 / var(--tw-text-opacity))",
                    },
                  },
                  "& .MuiOutlinedInput-root": {
                    "&.Mui-focused fieldset": {
                      borderColor:
                        "var(--tw-border-opacity: 1); border-color: rgb(75 85 99 / var(--tw-border-opacity))",
                    },
                  },
                }}
              />

              {showMediaForm && (
                <div className="border p-2 rounded-md bg-gray-50 dark:bg-gray-700 dark:border-gray-600">
                  {/* You can replace this div with your actual FileUploadForm component */}
                  <p className="text-sm mb-2 dark:text-gray-200">
                    Attach files:
                  </p>

                  <FileUploadForm
                    onSubmit={handleCommentSubmit}
                    setParentMediaList={setMediaList}
                  />

                  {media.length > 0 && (
                    <ul className="mt-2 list-disc pl-5 text-sm text-gray-600 dark:text-gray-300">
                      {media.map((file, idx) => (
                        <li key={idx}>{file.name}</li>
                      ))}
                    </ul>
                  )}
                </div>
              )}

              {/* Character counter */}
              {!commentError && (
                <div className="flex justify-end">
                  <Typography
                    variant="caption"
                    color={charsLeft === 0 ? "error" : "text.secondary"}
                    className="dark:text-gray-400"
                  >
                    {charsLeft} characters left
                  </Typography>
                </div>
              )}

              {/* <div className="flex justify-end gap-2 items-center">
                <IconButton
                  component="label"
                  onClick={handleAttachClick}
                  className="dark:text-gray-300"
                >
                  <AttachFile />
                </IconButton>

                <Button
                  variant="contained"
                  color="primary"
                  onClick={handleCommentSubmit}
                  disabled={isLoading || uploadResult.isLoading}
                  className="bg-main-blue hover:bg-blue-950 dark:bg-blue-700 dark:hover:bg-blue-800"
                >
                  Add Comment
                </Button>
              </div> */}
            </div>
          </div>
        </div>
      </div>

      <div className="flex items-center justify-between font-semibold text-lg my-3 dark:text-white">
        <div className="flex items-center justify-start">
          <Typography
            variant="body2"
            className="text-gray-600 dark:text-gray-400"
          >
            Sort by:
          </Typography>
          <Button
            onClick={handleSortMenuOpen}
            className="dark:text-gray-300 text-gray-700"
            aria-label="Sort comments"
            aria-controls="sort-menu"
            aria-haspopup="true"
            size="small"
            endIcon={<Sort />}
            variant="text"
          >
            {sortOptionLabels[sortOption]}
          </Button>
          <Menu
            id="sort-menu"
            anchorEl={anchorEl}
            keepMounted
            open={Boolean(anchorEl)}
            onClose={handleSortMenuClose}
            className="dark:text-gray-200"
            PaperProps={{
              className: "dark:bg-gray-800 dark:text-gray-200",
              sx: {
                "& .MuiMenuItem-root.Mui-selected": {
                  backgroundColor: "rgba(25, 118, 210, 0.12)",
                  "&.dark:text-gray-200": {
                    backgroundColor: "rgba(59, 130, 246, 0.2)",
                  },
                },
              },
            }}
          >
            <MenuItem
              onClick={() => handleSortChange("newest")}
              selected={sortOption === "newest"}
              className="dark:text-gray-200 dark:hover:bg-gray-700"
            >
              <ListItemText
                primary="Newest first"
                className="dark:text-gray-200"
              />
            </MenuItem>
            <MenuItem
              onClick={() => handleSortChange("oldest")}
              selected={sortOption === "oldest"}
              className="dark:text-gray-200 dark:hover:bg-gray-700"
            >
              <ListItemText
                primary="Oldest first"
                className="dark:text-gray-200"
              />
            </MenuItem>
            <MenuItem
              onClick={() => handleSortChange("mostUpvoted")}
              selected={sortOption === "mostUpvoted"}
              className="dark:text-gray-200 dark:hover:bg-gray-700"
            >
              <ListItemText
                primary="Most upvoted"
                className="dark:text-gray-200"
              />
            </MenuItem>
            <MenuItem
              onClick={() => handleSortChange("mostReplies")}
              selected={sortOption === "mostReplies"}
              className="dark:text-gray-200 dark:hover:bg-gray-700"
            >
              <ListItemText
                primary="Most replies"
                className="dark:text-gray-200"
              />
            </MenuItem>
          </Menu>
        </div>
        <div className="flex justify-end gap-2 items-center">
          <IconButton
            component="label"
            onClick={handleAttachClick}
            className="dark:text-gray-300"
          >
            <AttachFile />
          </IconButton>

          <Button
            variant="contained"
            color="primary"
            onClick={handleCommentSubmit}
            disabled={isLoading || uploadResult.isLoading}
            className="bg-main-blue hover:bg-blue-950 dark:bg-blue-700 dark:hover:bg-blue-800"
          >
            Add Comment
          </Button>
        </div>
      </div>

      {/* Comment List with Loading States */}
      {commentsLoading ? (
        // Skeleton UI for loading comments
        <div className="space-y-4">
          {[1, 2, 3].map((i) => (
            <div key={i} className="flex gap-3">
              <Skeleton className="h-10 w-10 rounded-full dark:bg-gray-700" />
              <div className="w-full">
                <Skeleton className="h-4 w-32 mb-2 dark:bg-gray-700" />
                <Skeleton className="h-3 w-20 mb-3 dark:bg-gray-700" />
                <Skeleton className="h-12 w-full dark:bg-gray-700" />
              </div>
            </div>
          ))}
        </div>
      ) : isCommentsError ? (
        <div className="p-4 rounded-md bg-gray-50 dark:bg-gray-700 text-center">
          <p className="text-gray-500 dark:text-gray-300">
            Unable to load comments. Please try again later.
          </p>
        </div>
      ) : comments && comments.length > 0 ? (
        getSortedComments().map((comment) => (
          <CommentCard
            key={comment.id}
            comment={comment}
            currentUserId={userProfile?.id}
            onDelete={handleDeleteComment}
          />
        ))
      ) : (
        <div className="p-4 rounded-md bg-gray-50 dark:bg-gray-700 text-center">
          <p className="text-gray-500 dark:text-gray-300">
            No comments yet. Be the first to share your thoughts!
          </p>
        </div>
      )}
    </div>
  );
};

export default CommentsSection;
