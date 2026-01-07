"use client";

import { useEffect, useMemo, useState } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import Image from "next/image";
import {
	MessageCircle,
	Ellipsis,
	Link2,
	Edit as EditIcon,
	Trash2,
} from "lucide-react";
import toast from "react-hot-toast";

import {
	DropdownMenu,
	DropdownMenuContent,
	DropdownMenuItem,
	DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu";
import {
	AlertDialog,
	AlertDialogAction,
	AlertDialogCancel,
	AlertDialogContent,
	AlertDialogDescription,
	AlertDialogFooter,
	AlertDialogHeader,
	AlertDialogTitle,
} from "@/components/ui/alert-dialog";

import { useDeletePostMutation } from "@/features/posts/postsSlice";
import type { Post } from "@/features/posts/types";
import MediaCarousel from "@/features/media/MediaCarousel";
import { useIsUserLoggedIn } from "@/hooks/useIsUserLoggedIn";
import PostVoteButtons from "./PostVoteButtons";
import PostTimeStamp from "./PostTimeStamp";
import PostingTags from "./PostingTags";

// Default profile picture for users without one
const DEFAULT_PROFILE_PIC = "/ProfilePic.svg";

interface PostCardProps {
	post: Post;
}

/**
 * PostCard displays a single post in the feed.
 * Includes author info, content preview, media, tags, and interaction buttons.
 */
const PostCard = ({ post }: PostCardProps) => {
	const router = useRouter();
	const { profile } = useIsUserLoggedIn();
	const memoizedPost = useMemo(() => post, [post]);

	const [deletePost, deleteResult] = useDeletePostMutation();
	const [showDeleteDialog, setShowDeleteDialog] = useState(false);

	// Handle delete result notifications
	useEffect(() => {
		if (deleteResult.isSuccess) {
			toast.success("Your post was deleted successfully.");
		}
		if (deleteResult.isError) {
			toast.error("An error occurred while deleting your post.");
			console.error(deleteResult.error);
		}
	}, [deleteResult.isSuccess, deleteResult.isError, deleteResult.error]);

	// Guard against invalid post data
	if (!memoizedPost || !memoizedPost.author) return null;

	const totalCommentsNumber = memoizedPost.comments;
	const isAuthor = memoizedPost.author.id === profile?.id;

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

	const handleEditPost = () => {
		router.push(`/posts/${memoizedPost.id}/edit`);
	};

	const handleDeletePost = async () => {
		try {
			await deletePost(memoizedPost.id).unwrap();
			setShowDeleteDialog(false);
		} catch (error) {
			// Error handling is already done in useEffect
			// Dialog stays open so user can retry
			toast.error("Failed to delete post. Please try again.");
			console.error("Failed to delete post:", error);
		}
	};

	const authorName = `${memoizedPost.author.firstName ?? ""} ${
		memoizedPost.author.lastName ?? ""
	}`.trim();
	const profilePicUrl =
		memoizedPost.author.profilePictureUrl || DEFAULT_PROFILE_PIC;

	// Truncate content for preview
	const contentPreview =
		memoizedPost.content.length > 100
			? `${memoizedPost.content.substring(0, 100)}...`
			: memoizedPost.content;

	return (
		<>
			<article className="flex flex-col gap-3 rounded-lg border border-border bg-card p-4 shadow-md">
				{/* Author Info Header */}
				<div className="flex items-center space-x-3">
					<Link href={`/profile/${memoizedPost.author.id}`}>
						<Image
							src={profilePicUrl}
							alt={authorName || "User profile"}
							width={40}
							height={40}
							className="rounded-full object-cover"
							unoptimized={profilePicUrl.startsWith("http")}
						/>
					</Link>

					<div className="flex w-full justify-between">
						<div>
							<Link href={`/profile/${memoizedPost.author.id}`}>
								<span className="block text-md font-semibold text-card-foreground hover:underline">
									{authorName || "Anonymous"}
								</span>
							</Link>
							<PostTimeStamp
								createdAt={memoizedPost.createdAt}
								lastModified={memoizedPost.lastModified}
							/>
						</div>

						{/* Actions Dropdown */}
						<DropdownMenu>
							<DropdownMenuTrigger className="px-1 rounded hover:bg-accent">
								<Ellipsis className="text-muted-foreground hover:text-card-foreground py-0" />
							</DropdownMenuTrigger>
							<DropdownMenuContent align="end">
								{/* Copy Link */}
								<DropdownMenuItem onClick={handleCopyLink}>
									<Link2 className="mr-2 h-4 w-4" />
									Copy link
								</DropdownMenuItem>

								{/* Author-only actions */}
								{isAuthor && (
									<>
										<DropdownMenuItem
											onClick={handleEditPost}
										>
											<EditIcon className="mr-2 h-4 w-4" />
											Edit post
										</DropdownMenuItem>
										<DropdownMenuItem
											onClick={() =>
												setShowDeleteDialog(true)
											}
											className="text-destructive focus:text-destructive"
										>
											<Trash2 className="mr-2 h-4 w-4" />
											Delete post
										</DropdownMenuItem>
									</>
								)}
							</DropdownMenuContent>
						</DropdownMenu>
					</div>
				</div>

				{/* Post Content */}
				<Link href={`/feed/${memoizedPost.id}`} className="block">
					<div className="wrap-break-word">
						<h2
							className="text-lg font-bold text-card-foreground whitespace-pre-wrap"
							dir="auto"
						>
							{memoizedPost.title}
						</h2>
					</div>
					<div className="wrap-break-word mt-1">
						<p
							className="text-muted-foreground whitespace-pre-wrap"
							dir="auto"
						>
							{contentPreview}
						</p>
					</div>
				</Link>

				{/* Media Carousel */}
				<MediaCarousel medias={memoizedPost.medias} />

				{/* Tags */}
				<PostingTags
					tags={memoizedPost.tags}
					language={
						memoizedPost.language?.toLowerCase() === "arabic"
							? "ar"
							: "en"
					}
				/>

				{/* Interactions */}
				<div className="flex items-center justify-between">
					<div className="flex items-center gap-2">
						{/* Votes */}
						<PostVoteButtons post={memoizedPost} />

						{/* Comments */}
						<Link href={`/feed/${memoizedPost.id}`}>
							<div className="flex items-center gap-2 rounded-full p-1 px-2 transition-colors hover:bg-accent hover:cursor-pointer">
								<MessageCircle className="h-5 w-5 text-muted-foreground" />
								<span className="text-md font-bold text-muted-foreground">
									{totalCommentsNumber}
								</span>
							</div>
						</Link>
					</div>
				</div>
			</article>

			{/* Delete Confirmation Dialog */}
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
							This action cannot be undone. This will permanently
							delete your post.
						</AlertDialogDescription>
					</AlertDialogHeader>
					<AlertDialogFooter>
						<AlertDialogCancel className="rounded-full">
							Cancel
						</AlertDialogCancel>
						<AlertDialogAction
							onClick={handleDeletePost}
							className="rounded-full bg-destructive hover:bg-destructive/90"
						>
							Delete
						</AlertDialogAction>
					</AlertDialogFooter>
				</AlertDialogContent>
			</AlertDialog>
		</>
	);
};

export default PostCard;
