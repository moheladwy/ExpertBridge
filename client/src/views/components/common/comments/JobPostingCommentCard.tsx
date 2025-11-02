import React, { useEffect, useState } from "react";
import { Button, TextField } from "@mui/material";
import { Comment, DeleteCommentRequest } from "@/features/comments/types";
import {
	useCreateReplyMutation,
	useUpdateCommentMutation,
} from "@/features/comments/commentsSlice";
import toast from "react-hot-toast";
import JobPostingCommentVoteButtons from "./JobPostingCommentVoteButtons";
import TimeAgo from "../../custom/TimeAgo";
import defaultProfile from "../../../../assets/Profile-pic/ProfilePic.svg";
import { useCurrentAuthUser } from "@/hooks/useCurrentAuthUser";
import { useAuthPrompt } from "@/contexts/AuthPromptContext";
import { Link } from "react-router";
import {
	DropdownMenu,
	DropdownMenuContent,
	DropdownMenuItem,
	DropdownMenuTrigger,
} from "@/views/components/custom/dropdown-menu";
import {
	DeleteIcon,
	EditIcon,
	CircleEllipsis as Ellipsis,
	Link2,
} from "lucide-react";
import {
	AlertDialog,
	AlertDialogAction,
	AlertDialogCancel,
	AlertDialogContent,
	AlertDialogDescription,
	AlertDialogFooter,
	AlertDialogHeader,
	AlertDialogTitle,
} from "@/views/components/ui/alert-dialog";
import MediaCarousel from "../media/MediaCarousel";

interface JobCommentItemProps {
	comment: Comment;
	currentUserId?: string | null;
	onDelete: (request: DeleteCommentRequest) => void;
	isReply?: boolean;
	jobPostingId: string;
}

const JobPostingCommentCard: React.FC<JobCommentItemProps> = ({
	comment,
	currentUserId,
	onDelete,
	isReply,
	jobPostingId,
}) => {
	const [showReplies, setShowReplies] = useState(false);
	const [replyText, setReplyText] = useState("");
	const authUser = useCurrentAuthUser();
	const { showAuthPrompt } = useAuthPrompt();
	const [showDeleteDialog, setShowDeleteDialog] = useState(false);

	const [createReply, { isLoading, isSuccess, isError }] =
		useCreateReplyMutation();

	useEffect(() => {
		if (isError) toast.error("An error occurred while creating your reply");
		if (isSuccess) {
			toast.success("Reply created successfully");
		}
	}, [isSuccess, isError, isLoading]);

	const handleReplySubmit = async () => {
		if (!authUser) {
			showAuthPrompt();
			return;
		}

		if (!replyText.trim()) return;

		await createReply({
			jobPostingId: jobPostingId,
			content: replyText,
			parentCommentId: comment.id,
		});

		setReplyText("");
	};

	const handleCopyCommentLink = () => {
		const url = `${window.location.origin}/jobs/${jobPostingId}#comment-${comment.id}`;
		navigator.clipboard.writeText(url);
		toast.success("Comment link copied to clipboard");
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
			jobPostingId: jobPostingId,
			authorId: comment.authorId,
			parentCommentId: comment.parentCommentId,
		}).unwrap();

		setIsEditing(false);
	};

	useEffect(() => {
		if (editResult.isSuccess) {
			toast.success("Comment edited successfully.");
		} else if (editResult.isError) {
			toast.error("An error occurred while editing your comment.");
		}
	}, [editResult.isSuccess, editResult.isError]);

	const handleDeleteComment = () =>
		onDelete({
			commentId: comment.id,
			jobPostingId: jobPostingId,
			authorId: comment.authorId,
			parentCommentId: comment.parentCommentId,
		});

	return (
		<div
			className={`flex flex-col gap-3 p-3 border-t border-gray-300 dark:border-gray-600 ${isReply ? "ml-4 border-l-2 border-t-0 border-gray-300 dark:border-l-gray-600 pl-3" : ""}`}
			id={`comment-${comment.id}`}
		>
			{/* Comment Author */}
			<div className="flex items-center justify-between">
				<div className="flex items-center space-x-3">
					<Link to={`/profile/${comment.author.id}`}>
						{comment.author?.profilePictureUrl ? (
							<img
								src={
									comment.author.profilePictureUrl ||
									"./src/assets/default-avatar.png"
								}
								alt="Comment Author"
								width={30}
								height={30}
								className="rounded-full"
							/>
						) : (
							<img
								src={defaultProfile}
								alt="Comment Author"
								width={30}
								height={30}
								className="rounded-full"
							/>
						)}
					</Link>
					<div>
						<h4 className="text-sm font-semibold dark:text-white">
							{comment.author.firstName +
								" " +
								comment.author.lastName}
						</h4>
						<span className="text-xs text-gray-500">
							@{comment.author.username}
						</span>
						<p className="text-xs text-gray-500 dark:text-gray-400">
							<TimeAgo timestamp={comment.createdAt} />
						</p>
					</div>
				</div>

				{/* More Dropdown */}
				<DropdownMenu>
					<DropdownMenuTrigger>
						<Ellipsis className="text-gray-500 dark:text-gray-400 hover:text-gray-700 dark:hover:text-gray-300 hover:cursor-pointer w-4 h-4" />
					</DropdownMenuTrigger>
					<DropdownMenuContent>
						<DropdownMenuItem>
							<div
								className="flex items-center text-gray-800 dark:text-gray-200 justify-center gap-2 cursor-pointer"
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
										className="flex items-center text-gray-800 dark:text-gray-200 justify-center gap-2 cursor-pointer"
										onClick={handleEditComment}
									>
										<EditIcon className="w-4" />
										<h6 className="text-sm">Edit</h6>
									</div>
								</DropdownMenuItem>
								<DropdownMenuItem
									onClick={() => setShowDeleteDialog(true)}
								>
									<div className="flex items-center text-gray-800 dark:text-gray-200 justify-center gap-2 cursor-pointer">
										<DeleteIcon className="w-4 text-red-700" />
										<h6 className="text-sm text-red-700">
											Delete
										</h6>
									</div>
								</DropdownMenuItem>
							</>
						)}
					</DropdownMenuContent>
				</DropdownMenu>

				{/* Delete confirmation dialog */}
				<AlertDialog
					open={showDeleteDialog}
					onOpenChange={setShowDeleteDialog}
				>
					<AlertDialogContent>
						<AlertDialogHeader>
							<AlertDialogTitle>
								Are you absolutely sure?
							</AlertDialogTitle>
							<AlertDialogDescription>
								This action cannot be undone. This will
								permanently delete your comment.
							</AlertDialogDescription>
						</AlertDialogHeader>
						<AlertDialogFooter>
							<AlertDialogCancel>Cancel</AlertDialogCancel>
							<AlertDialogAction
								onClick={() => {
									handleDeleteComment();
									setShowDeleteDialog(false);
								}}
								className="bg-red-700 hover:bg-red-900"
							>
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
							className="dark:text-white [&_.MuiOutlinedInput-root]:dark:text-white [&_.MuiInputBase-input]:dark:text-white [&_.MuiInputBase-input]::placeholder:dark:text-gray-300 [&_.MuiOutlinedInput-notchedOutline]:dark:border-gray-600"
							inputProps={{
								className:
									"dark:text-white placeholder:dark:text-gray-300",
							}}
							sx={{
								"& .MuiInputBase-input": {
									color: "var(--tw-text-opacity: 1); color: rgb(255 255 255 / var(--tw-text-opacity))",
								},
								"& .MuiOutlinedInput-root": {
									"&.Mui-focused fieldset": {
										borderColor:
											"var(--tw-border-opacity: 1); border-color: rgb(75 85 99 / var(--tw-border-opacity))",
									},
								},
							}}
						/>
						<div className="flex gap-2 justify-end">
							<Button
								size="small"
								variant="contained"
								className="bg-main-blue hover:bg-blue-950 dark:bg-blue-700 dark:hover:bg-blue-800"
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
								className="dark:text-gray-300 dark:hover:text-white"
							>
								Cancel
							</Button>
						</div>
					</div>
				) : (
					<p
						className="text-gray-700 dark:text-gray-300 whitespace-pre-wrap"
						dir="auto"
					>
						{comment.content}
					</p>
				)}
			</div>

			<MediaCarousel medias={comment.medias} />

			{/* Comment Actions */}
			{isReply ? null : (
				<>
					<div className="flex items-center space-x-3">
						<JobPostingCommentVoteButtons comment={comment} />

						{comment.replies && comment.replies.length > 0 && (
							<Button
								size="small"
								onClick={() => setShowReplies((prev) => !prev)}
								className="text-blue-500 dark:text-blue-400 hover:text-blue-700 dark:hover:text-blue-300"
							>
								{showReplies ? "Hide Replies" : "Show Replies"}{" "}
								({comment.replies.length})
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
									dir: "auto",
								},
							}}
							className="dark:text-white [&_.MuiOutlinedInput-root]:dark:text-white [&_.MuiInputBase-input]:dark:text-white [&_.MuiOutlinedInput-notchedOutline]:dark:border-gray-600"
							inputProps={{
								className:
									"dark:text-white placeholder:dark:text-gray-300",
							}}
							sx={{
								"& .MuiInputBase-input": {
									"&::placeholder": {
										color: "var(--tw-text-opacity: 1); color: rgb(209 213 219 / var(--tw-text-opacity))",
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
						<div className="w-full flex mt-2 justify-end">
							<Button
								onClick={handleReplySubmit}
								variant="contained"
								size="small"
								className="bg-main-blue hover:bg-blue-950 dark:bg-blue-700 dark:hover:bg-blue-800"
								disabled={isLoading}
							>
								Reply
							</Button>
						</div>
					</div>

					{/* Replies Section */}
					{showReplies &&
						comment.replies?.map((reply) => (
							<JobPostingCommentCard
								key={reply.id}
								comment={reply}
								currentUserId={currentUserId}
								onDelete={onDelete}
								isReply={true}
								jobPostingId={jobPostingId}
							/>
						))}
				</>
			)}
		</div>
	);
};

export default JobPostingCommentCard;
