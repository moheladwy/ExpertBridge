"use client";

import { useEffect, useState } from "react";
import Link from "next/link";
import { useRouter } from "next/navigation";
import Image from "next/image";
import {
	CircleArrowLeft,
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

interface FullPostWithCommentsProps {
	post: Post;
}

/**
 * Full post view with all details and comments section.
 * Displays the complete post with author info, content, media, tags, voting, and comments.
 */
const FullPostWithComments = ({ post }: FullPostWithCommentsProps) => {
	const router = useRouter();
	const { profile } = useIsUserLoggedIn();

	const [deletePost, deleteResult] = useDeletePostMutation();
	const [showDeleteDialog, setShowDeleteDialog] = useState(false);

	// Handle delete result notifications
	useEffect(() => {
		if (deleteResult.isSuccess) {
			toast.success("Your post was deleted successfully.");
			router.push("/feed");
		}
	}, [deleteResult.isSuccess, router]);

	useEffect(() => {
		if (deleteResult.error) {
			toast.error("An error occurred while deleting your post.");
		}
	}, [deleteResult.error]);

	// Copy post link to clipboard
	const handleCopyLink = async () => {
		const postUrl = `${window.location.origin}/feed/${post.id}`;
		try {
			await navigator.clipboard.writeText(postUrl);
			toast.success("Link copied successfully");
		} catch (err) {
			toast.error("Failed to copy link");
			console.error("Failed to copy link: ", err);
		}
	};

	// Handle post deletion
	const handleDeletePost = async () => {
		await deletePost(post.id);
		setShowDeleteDialog(false);
	};

	// Navigate back to previous page
	const handleGoBack = () => {
		router.back();
	};

	const isAuthor = profile?.id === post.author.id;
	const authorName = `${post.author.firstName || ""} ${
		post.author.lastName || ""
	}`.trim();
	const profilePicUrl = post.author.profilePictureUrl || DEFAULT_PROFILE_PIC;

	return (
		<div className="w-full flex justify-center">
			<div className="w-4/6 mx-auto py-4 flex gap-3 max-lg:flex-col max-sm:w-full">
				{/* Main Post Content - Left Side */}
				<div className="w-6/6 max-lg:w-full">
					<div className="flex flex-col gap-3">
						<div className="flex flex-col gap-3 bg-card shadow-md rounded-lg p-4 border border-border">
							{/* Post Header */}
							<div className="flex items-center justify-between pb-3 border-b border-border">
								{/* Back Icon */}
								<button
									type="button"
									onClick={handleGoBack}
									className="cursor-pointer"
									aria-label="Go back"
								>
									<CircleArrowLeft className="text-muted-foreground hover:text-card-foreground transition-colors" />
								</button>

								{/* More Menu */}
								<DropdownMenu>
									<DropdownMenuTrigger
										className="cursor-pointer"
										aria-label="Post options"
									>
										<Ellipsis className="text-muted-foreground hover:text-card-foreground transition-colors" />
									</DropdownMenuTrigger>
									<DropdownMenuContent align="end">
										{/* Copy Link */}
										<DropdownMenuItem
											onClick={handleCopyLink}
										>
											<Link2 className="w-4 h-4 mr-2" />
											Copy link
										</DropdownMenuItem>

										{/* Edit (author only) */}
										{isAuthor && (
											<DropdownMenuItem
												onClick={() =>
													router.push(
														`/posts/${post.id}/edit`
													)
												}
											>
												<EditIcon className="w-4 h-4 mr-2" />
												Edit post
											</DropdownMenuItem>
										)}

										{/* Delete (author only) */}
										{isAuthor && (
											<DropdownMenuItem
												onClick={() =>
													setShowDeleteDialog(true)
												}
												className="text-destructive focus:text-destructive"
											>
												<Trash2 className="w-4 h-4 mr-2" />
												Delete post
											</DropdownMenuItem>
										)}
									</DropdownMenuContent>
								</DropdownMenu>

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
												This action cannot be undone.
												This will permanently delete
												your post.
											</AlertDialogDescription>
										</AlertDialogHeader>
										<AlertDialogFooter>
											<AlertDialogCancel className="rounded-full">
												Cancel
											</AlertDialogCancel>
											<AlertDialogAction
												onClick={handleDeletePost}
												className="bg-destructive hover:bg-destructive/90 rounded-full"
											>
												Delete
											</AlertDialogAction>
										</AlertDialogFooter>
									</AlertDialogContent>
								</AlertDialog>
							</div>

							{/* Author Info */}
							<div className="flex items-center space-x-3">
								<Link href={`/profile/${post.author.id}`}>
									<Image
										src={profilePicUrl}
										alt={`${authorName}'s profile picture`}
										width={40}
										height={40}
										className="rounded-full object-cover"
									/>
								</Link>
								<div>
									<Link href={`/profile/${post.author.id}`}>
										<h3 className="text-md font-semibold text-card-foreground hover:underline">
											{authorName ||
												post.author.username ||
												"Anonymous"}
										</h3>
									</Link>
									<PostTimeStamp
										createdAt={post.createdAt}
										lastModified={post.lastModified}
									/>
								</div>
							</div>

							{/* Post Title */}
							<div className="wrap-break-word">
								<h2
									className="text-lg font-bold text-card-foreground whitespace-pre-wrap"
									dir="auto"
								>
									{post.title}
								</h2>
							</div>

							{/* Post Content */}
							<div className="wrap-break-word">
								<p
									className="text-muted-foreground whitespace-pre-wrap"
									dir="auto"
								>
									{post.content}
								</p>
							</div>

							{/* Media */}
							{post.medias && post.medias.length > 0 && (
								<MediaCarousel medias={post.medias} />
							)}

							{/* Tags */}
							{post.tags && post.tags.length > 0 && (
								<PostingTags
									tags={post.tags}
									language={
										post.language?.toLowerCase() ===
										"arabic"
											? "ar"
											: "en"
									}
								/>
							)}

							{/* Post Voting */}
							<PostVoteButtons post={post} />

							{/* Comments Section Placeholder */}
							<div className="mt-4 pt-4 border-t border-border">
								<h3 className="text-lg font-semibold text-card-foreground mb-4">
									Comments ({post.comments || 0})
								</h3>
								<div className="text-muted-foreground text-center py-8 bg-muted/30 rounded-lg">
									<p>Comments section coming soon...</p>
								</div>
							</div>
						</div>
					</div>
				</div>
			</div>
		</div>
	);
};

export default FullPostWithComments;
