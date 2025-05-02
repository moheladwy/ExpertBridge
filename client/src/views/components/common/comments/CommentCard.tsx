import React, { useEffect, useState } from "react";
import { Button, TextField, IconButton } from "@mui/material";
import { ThumbUp, ThumbDown } from "@mui/icons-material";
import { Comment, DeleteCommentRequest } from "@/features/comments/types";
import { commentsApiSlice, useCreateReplyMutation, useUpdateCommentMutation } from "@/features/comments/commentsSlice";
import toast from "react-hot-toast";
import CommentVoteButtons from "./CommentVoteButtons";
import TimeAgo from "../../custom/TimeAgo";
import defaultProfile from "../../../../assets/Profile-pic/ProfilePic.svg"
import { useCurrentAuthUser } from "@/hooks/useCurrentAuthUser";
import { useAuthPrompt } from "@/contexts/AuthPromptContext";
import { Link } from "react-router";

import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/views/components/custom/dropdown-menu";
import { DeleteIcon, EditIcon, Ellipsis, Link2 } from "lucide-react";

import {
  AlertDialog,
  AlertDialogAction,
  AlertDialogCancel,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
  AlertDialogTrigger,
} from "@/views/components/ui/alert-dialog"

interface CommentItemProps {
  comment: Comment;
  currentUserId?: string | null;
  onDelete: (request: DeleteCommentRequest) => void;
  isReply?: boolean;
}

const CommentCard: React.FC<CommentItemProps> = ({ comment, currentUserId, onDelete, isReply }) => {
  const [showReplies, setShowReplies] = useState(false);
  const [replyText, setReplyText] = useState("");
  const authUser = useCurrentAuthUser();
  const { showAuthPrompt } = useAuthPrompt();
  // const [replies, setReplies] = useState<Comment[]>(comment.replies || []);
  const [showDeleteDialog, setShowDeleteDialog] = useState(false);

  const [createReply, { isLoading, isSuccess, isError }] = useCreateReplyMutation();

  useEffect(() => {
    if (isError) toast.error("An error occurred while creating your reply");
    if (isSuccess) {
      toast.success("reply created successfully");
      // setReplies(comment.replies || []);
    }
  }, [isSuccess, isError, isLoading]);

  const handleReplySubmit = async () => {

    if (!authUser) {
      showAuthPrompt();
      return;
    }


    if (!replyText.trim()) return;

    await createReply({
      postId: comment.postId,
      content: replyText,
      parentCommentId: comment.id,
    });

    setReplyText("");
  };

  const handleCopyCommentLink = () => {
    const url = `${window.location.origin}/posts/${comment.postId}#comment-${comment.id}`;
    navigator.clipboard.writeText(url);
  };

  const [isEditing, setIsEditing] = useState(false);
  const [editedText, setEditedText] = useState(comment.content);

  const handleEditComment = () => {
    setIsEditing(true);
    setEditedText(comment.content);
  };

  const [updateComment, editResult] = useUpdateCommentMutation();

  const handleSaveEdit = async () => {
    if (!editedText.trim()) return;

    await updateComment({
      commentId: comment.id,
      content: editedText,
      postId: comment.postId,
      authorId: comment.authorId,
      parentCommentId: comment.parentCommentId,
    }).unwrap();

    setIsEditing(false);
  };

  useEffect(() => {
    if (editResult.isSuccess) {
      toast.success('Comment edited successfully.');
    }
    else if (editResult.isError) {
      toast.error('An error occurred while editing your comment.');
    }
  }, [editResult.isSuccess, editResult.isError]);

  const handleDeleteComment = () => onDelete({
    commentId: comment.id,
    postId: comment.postId,
    authorId: comment.authorId,
    parentCommentId: comment.parentCommentId,
  });

  return (
    <div
      className={`flex flex-col gap-3 p-3 border-t border-gray-300 ${isReply ? "ml-4 border-l-2 border-gray-300 pl-3" : ""}`}
      id={`comment-${comment.id}`}
    >
      {/* Comment Author */}
      <div className="flex items-center justify-between">
        <div className="flex items-center space-x-3">
          <Link to={`/profile/${comment.author.id}`}>
            {comment.author?.profilePictureUrl ?
              <img
                src={comment.author.profilePictureUrl || "./src/assets/default-avatar.png"}
                alt="Comment Author"
                width={30}
                height={30}
                className="rounded-full"
              />
              :
              <img
                src={defaultProfile}
                alt="Comment Author"
                width={30}
                height={30}
                className="rounded-full"
              />
            }
          </Link>
          <div>
            <h4 className="text-sm font-semibold">{comment.author.firstName + ' ' + comment.author.lastName}</h4>
            <p className="text-xs text-gray-500">
              <TimeAgo timestamp={comment.createdAt} />
            </p>
          </div>
        </div>

        {/* More Dropdown */}
        <DropdownMenu>
          <DropdownMenuTrigger>
            <Ellipsis className="text-gray-500 hover:text-gray-700 hover:cursor-pointer w-4 h-4" />
          </DropdownMenuTrigger>
          <DropdownMenuContent>
            <DropdownMenuItem>
              <div
                className="flex items-center text-gray-800 justify-center gap-2 cursor-pointer"
                onClick={handleCopyCommentLink}
              >
                <Link2 className="w-4" />
                <h6 className="text-sm">Copy link</h6>
              </div>
            </DropdownMenuItem>

            {comment.author.id === currentUserId && (
              <>
                <DropdownMenuItem>
                  <div
                    className="flex items-center text-gray-800 justify-center gap-2 cursor-pointer"
                    onClick={handleEditComment}
                  >
                    <EditIcon className="w-4" />
                    <h6 className="text-sm">Edit</h6>
                  </div>
                </DropdownMenuItem>
                <DropdownMenuItem
                  onClick={() => setShowDeleteDialog(true)}
                >
                  <div className="flex items-center text-gray-800 justify-center gap-2 cursor-pointer">
                    <DeleteIcon className="w-4 text-red-700" />
                    <h6 className="text-sm text-red-700">Delete</h6>
                  </div>
                </DropdownMenuItem>
              </>
            )}
          </DropdownMenuContent>
        </DropdownMenu>

        {/* Delete confermation dialog */}
        <AlertDialog open={showDeleteDialog} onOpenChange={setShowDeleteDialog}>
          <AlertDialogContent>
            <AlertDialogHeader>
              <AlertDialogTitle>Are you absolutely sure?</AlertDialogTitle>
              <AlertDialogDescription>
                This action cannot be undone. This will permanently delete your comment.
              </AlertDialogDescription>
            </AlertDialogHeader>
            <AlertDialogFooter>
              <AlertDialogCancel>Cancel</AlertDialogCancel>
              <AlertDialogAction
                onClick={() => {
                  handleDeleteComment();
                  setShowDeleteDialog(false);
                }}
                className="bg-red-700 hover:bg-red-900">
                Delete
              </AlertDialogAction>
            </AlertDialogFooter>
          </AlertDialogContent>
        </AlertDialog>
      </div>

      {/* Comment Content */}
      <div className="w-full break-words">
        {isEditing ? (
          <div className="flex flex-col gap-2">
            <TextField
              fullWidth
              multiline
              size="small"
              value={editedText}
              onChange={(e) => setEditedText(e.target.value)}
              disabled={editResult.isLoading}
              slotProps={{ htmlInput: { dir: "auto" } }}
            />
            <div className="flex gap-2 justify-end">
              <Button
                size="small"
                variant="contained"
                className="bg-main-blue hover:bg-blue-950"
                onClick={handleSaveEdit}
                disabled={editResult.isLoading}
              >
                Save
              </Button>
              <Button
                size="small"
                variant="text"
                onClick={() => setIsEditing(false)}
                disabled={editResult.isLoading}
              >
                Cancel
              </Button>
            </div>
          </div>
        ) : (
          <p className="text-gray-700 whitespace-pre-wrap" dir="auto">{comment.content}</p>
        )}
      </div>

      {/* Comment Actions */}
      {isReply ? null :
        <>
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
              slotProps={{
                htmlInput: {
                  dir: "auto"
                }
              }}
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
          {showReplies && comment.replies?.map((reply) => (
            <CommentCard
              key={reply.id}
              comment={reply}
              currentUserId={currentUserId}
              onDelete={onDelete}
              isReply={true}
            />
          ))
          }
        </>
      }
    </div>
  );
};

export default CommentCard;
