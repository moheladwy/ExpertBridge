import { useDeletePostMutation } from "@/features/posts/postsSlice";
import { Link } from "react-router-dom";
import {
	MessageCircle,
	Ellipsis,
	Link2,
	Edit as EditIcon,
	Trash2,
} from "lucide-react";
import {
	DropdownMenu,
	DropdownMenuContent,
	DropdownMenuItem,
	DropdownMenuTrigger,
} from "@/views/components/custom/dropdown-menu";
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
import toast from "react-hot-toast";
import { useEffect, useMemo, useState } from "react";
import PostVoteButtons from "./PostVoteButtons";
import defaultProfile from "../../../../assets/Profile-pic/ProfilePic.svg";
import EditPostModal from "./EditPostModal";
import MediaCarousel from "../media/MediaCarousel";
import PostTimeStamp from "./PostTimeStamp";
import { Post } from "@/features/posts/types";
import PostingTags from "../tags/PostingTags";

interface PostCardProps {
	post: Post;
	currUserId?: string | null;
}

const PostCard: React.FC<PostCardProps> = ({ post, currUserId }) => {
	const memoizedPost = useMemo(() => post, [post]);

	const currentUserId = useMemo(() => currUserId, [currUserId]);

	const [isEditModalOpen, setIsEditModalOpen] = useState(false);
	const [deletePost, deleteResult] = useDeletePostMutation();
	// conferm delete dialog
	const [showDeleteDialog, setShowDeleteDialog] = useState(false);

	useEffect(() => {
		if (deleteResult.isSuccess) {
			toast.success("Your post was deleted successfully.");
		}
		if (deleteResult.isError) {
			toast.error("An error occurred while deleting you post.");
			console.log(deleteResult.error);
		}
	}, [deleteResult.isSuccess, deleteResult.isError, deleteResult.error]);

	if (!memoizedPost || !memoizedPost.author) return null;

	const totalCommentsNumber = memoizedPost.comments;

	const handleCopyLink = () => {
		const postUrl = `${window.location.origin}/feed/${memoizedPost.id}`;
		navigator.clipboard
			.writeText(postUrl)
			.then(() => {
				toast.success("Link copied successfully");
			})
			.catch(() => {
				toast.error("Failed to copy link");
			});
	};

	const handleDeletePost = async () => {
		await deletePost(memoizedPost.id);
	};

	return (
		<>
			<div className="flex flex-col gap-3 bg-white dark:bg-gray-800 shadow-md rounded-lg p-4 border border-gray-200 dark:border-gray-700">
				{/* Author Info */}
				<div className="flex items-center space-x-3">
					<Link to={`/profile/${memoizedPost.author.id}`}>
						{memoizedPost.author.profilePictureUrl ? (
							<img
								src={memoizedPost.author.profilePictureUrl}
								width={40}
								height={40}
								className="rounded-full"
							/>
						) : (
							<img
								src={defaultProfile}
								width={40}
								height={40}
								className="rounded-full"
							/>
						)}
					</Link>
					<div className="flex w-full justify-between">
						<div>
							<Link to={`/profile/${memoizedPost.author.id}`}>
								{/* Name */}
								<span className="text-md font-semibold text-gray-800 dark:text-gray-100 block">
									{memoizedPost.author.firstName +
										" " +
										memoizedPost.author.lastName}
								</span>
							</Link>
							<PostTimeStamp
								createdAt={memoizedPost.createdAt}
								lastModified={memoizedPost.lastModified}
							/>
						</div>

						{/* More */}
						<DropdownMenu>
							<DropdownMenuTrigger>
								<Ellipsis className="text-gray-500 dark:text-gray-400 hover:text-gray-700 dark:hover:text-gray-200 hover:cursor-pointer" />
							</DropdownMenuTrigger>
							<DropdownMenuContent>
								<DropdownMenuItem>
									<div
										className="flex items-center text-gray-800 dark:text-gray-200 justify-center gap-2 cursor-pointer"
										onClick={handleCopyLink}
									>
										<Link2 className="w-5" />
										<h6>Copy link</h6>
									</div>
								</DropdownMenuItem>
								{memoizedPost.author.id === currentUserId && (
									<>
										{/* Edit */}
										<DropdownMenuItem>
											<div
												className="flex items-center text-gray-800 dark:text-gray-200 justify-center gap-2 cursor-pointer"
												onClick={() =>
													setIsEditModalOpen(true)
												}
											>
												<EditIcon className="w-5" />
												<h6>Edit post</h6>
											</div>
										</DropdownMenuItem>
										{/* Delete */}
										<DropdownMenuItem
											onClick={() =>
												setShowDeleteDialog(true)
											}
										>
											<div className="flex items-center text-gray-800 dark:text-gray-200 justify-center gap-2 cursor-pointer">
												<Trash2 className="w-5 text-red-700" />
												<h6 className="text-red-700">
													Delete post
												</h6>
											</div>
										</DropdownMenuItem>
									</>
								)}
							</DropdownMenuContent>
						</DropdownMenu>

						{/* Delete confermation dialog */}
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
										permanently delete your question.
									</AlertDialogDescription>
								</AlertDialogHeader>
								<AlertDialogFooter>
									<AlertDialogCancel>
										Cancel
									</AlertDialogCancel>
									<AlertDialogAction
										onClick={() => {
											handleDeletePost();
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
				</div>

				{/* Post Title */}
				<Link to={`/feed/${memoizedPost.id}`}>
					<div className="break-words">
						<h2
							className="text-lg font-bold text-gray-700 dark:text-gray-100 whitespace-pre-wrap"
							dir="auto"
						>
							{memoizedPost.title}
						</h2>
					</div>

					{/* Post Content */}
					<div className="break-words">
						<p
							className="text-gray-600 dark:text-gray-300 whitespace-pre-wrap"
							dir="auto"
						>
							{memoizedPost.content.substring(0, 100)}...
						</p>
					</div>
				</Link>

				{/* Media */}
				<MediaCarousel medias={memoizedPost.medias} />

				{/* Post Metadata */}
				{/* Tags */}
				<PostingTags
					tags={memoizedPost.tags}
					language={memoizedPost.language}
				/>

				{/* Interactions */}
				<div className="flex justify-between items-center">
					<div className="flex gap-2 items-center">
						{/* Votes */}
						<PostVoteButtons post={memoizedPost} />

						{/* Comments */}
						<Link to={`/feed/${memoizedPost.id}`}>
							<div className="flex items-center gap-2 rounded-full p-1 px-2 hover:bg-gray-200 dark:hover:bg-gray-700 hover:cursor-pointer">
								<MessageCircle className="text-gray-500 dark:text-gray-400" />
								<div className="text-gray-500 dark:text-gray-400 text-md font-bold">
									{totalCommentsNumber}
								</div>
							</div>
						</Link>
					</div>
				</div>
			</div>
			<EditPostModal
				post={memoizedPost}
				isOpen={isEditModalOpen}
				onClose={() => setIsEditModalOpen(false)}
			/>
		</>
	);
};

export default PostCard;
