import {
	useCreateCommentMutation,
	useDeleteCommentMutation,
	useGetCommentsByPostIdQuery,
} from "@/features/comments/commentsSlice";
import { Paperclip, ArrowUpDown } from "lucide-react";
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
import { Button } from "@/views/components/ui/button";
import { Textarea } from "@/views/components/ui/textarea";
import {
	Field,
	FieldError,
} from "@/views/components/ui/field";
import {
	DropdownMenu,
	DropdownMenuContent,
	DropdownMenuItem,
	DropdownMenuTrigger,
} from "@/views/components/ui/dropdown-menu";

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
	>("oldest"); // Map sort options to display names
	const sortOptionLabels = {
		newest: "Newest",
		oldest: "Oldest",
		mostUpvoted: "Most upvoted",
		mostReplies: "Most replies",
	};

	useEffect(() => {
		if (isError)
			toast.error("An error occurred while creating your comment");
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

	const handleCommentChange = (e: React.ChangeEvent<HTMLTextAreaElement>) => {
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
		{ postId, content: commentText }
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

	const handleAttachClick = (e: React.MouseEvent<HTMLButtonElement>) => {
		e.stopPropagation();
		e.preventDefault();
		if (!authUser) {
			showAuthPrompt();
			return;
		}
		setShowMediaForm((prev) => !prev);
	};

	const handleSortChange = (
		option: "newest" | "oldest" | "mostUpvoted" | "mostReplies"
	) => {
		setSortOption(option);
	};

	const getSortedComments = () => {
		if (!comments || comments.length === 0) return [];

		const sortedComments = [...comments];

		switch (sortOption) {
			case "newest":
				return sortedComments.sort((a, b) =>
					b.createdAt.localeCompare(a.createdAt)
				);
			case "oldest":
				return sortedComments.sort((a, b) =>
					a.createdAt.localeCompare(b.createdAt)
				);
			case "mostUpvoted":
				return sortedComments.sort(
					(a, b) =>
						b.upvotes - b.downvotes - (a.upvotes - a.downvotes)
				);
			case "mostReplies":
				return sortedComments.sort(
					(a, b) =>
						(b.replies?.length || 0) - (a.replies?.length || 0)
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
							<Field
								className="w-full"
								data-invalid={!!commentError}
							>
								<Textarea
									id="comment-text"
									placeholder="Add a comment..."
									value={commentText}
									maxLength={MAX_COMMENT_LENGTH}
									dir="auto"
									onChange={handleCommentChange}
									disabled={
										isLoading || uploadResult.isLoading
									}
									className="min-h-[80px] resize-none"
								/>
								{commentError && (
									<FieldError>{commentError}</FieldError>
								)}
							</Field>

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
									<p
										className={`text-sm ${
											charsLeft === 0
												? "text-red-500 dark:text-red-400"
												: "text-muted-foreground"
										}`}
									>
										{charsLeft} characters left
									</p>
								</div>
							)}
						</div>
					</div>
				</div>
			</div>

			<div className="flex items-center justify-between font-semibold text-lg my-3 dark:text-white">
				<div className="flex items-center justify-start gap-2">
					<p className="text-sm text-gray-600 dark:text-gray-400">
						Sort by:
					</p>
					<DropdownMenu>
						<DropdownMenuTrigger asChild>
							<Button
								variant="ghost"
								size="sm"
								className="dark:text-gray-300 text-gray-700"
								aria-label="Sort comments"
							>
								{sortOptionLabels[sortOption]}
								<ArrowUpDown className="ml-2 h-4 w-4" />
							</Button>
						</DropdownMenuTrigger>
						<DropdownMenuContent align="start">
							<DropdownMenuItem
								onClick={() => handleSortChange("newest")}
								className={
									sortOption === "newest" ? "bg-accent" : ""
								}
							>
								Newest first
							</DropdownMenuItem>
							<DropdownMenuItem
								onClick={() => handleSortChange("oldest")}
								className={
									sortOption === "oldest" ? "bg-accent" : ""
								}
							>
								Oldest first
							</DropdownMenuItem>
							<DropdownMenuItem
								onClick={() => handleSortChange("mostUpvoted")}
								className={
									sortOption === "mostUpvoted"
										? "bg-accent"
										: ""
								}
							>
								Most upvoted
							</DropdownMenuItem>
							<DropdownMenuItem
								onClick={() => handleSortChange("mostReplies")}
								className={
									sortOption === "mostReplies"
										? "bg-accent"
										: ""
								}
							>
								Most replies
							</DropdownMenuItem>
						</DropdownMenuContent>
					</DropdownMenu>
				</div>
				<div className="flex justify-end gap-2 items-center">
					<Button
						variant="ghost"
						size="icon"
						onClick={handleAttachClick}
						className="dark:text-gray-300"
						aria-label="Attach files"
					>
						<Paperclip className="h-5 w-5" />
					</Button>

					<Button
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
