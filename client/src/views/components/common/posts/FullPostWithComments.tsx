import React, { useMemo, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import { Post } from "@/features/posts/types";
import CommentsSection from "../comments/CommentsSection";
import PostVoteButtons from "./PostVoteButtons";
import { CircleArrowLeft, Ellipsis, Link2, Trash2 } from "lucide-react";
import {
	DropdownMenu,
	DropdownMenuContent,
	DropdownMenuItem,
	DropdownMenuTrigger,
} from "@/views/components/ui/dropdown-menu";
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
import useIsUserLoggedIn from "@/hooks/useIsUserLoggedIn";
import defaultProfile from "../../../../assets/Profile-pic/ProfilePic.svg";
import MediaCarousel from "../media/MediaCarousel";
import PostTimeStamp from "./PostTimeStamp";
import SimilarPosts from "./SimilarPosts";
import EditPostModal from "./EditPostModal";
import PostingTags from "../tags/PostingTags";

interface FullPostWithCommentsProps {
	post: Post;
	deletePost: (...args: any) => any;
}

const FullPostWithComments: React.FC<FullPostWithCommentsProps> = ({
	post,
	deletePost,
}) => {
	const [, , , , userProfile] = useIsUserLoggedIn();
	const memoizedPost = useMemo(() => post, [post]);
	const [showDeleteDialog, setShowDeleteDialog] = useState(false);
	const [isEditModalOpen, setIsEditModalOpen] = useState(false);
	const navigate = useNavigate();

	const handleCopyLink = () => {
		const postUrl = `${window.location.origin}/feed/${post?.id}`;
		navigator.clipboard
			.writeText(postUrl)
			.then(() => {
				toast.success("Link copied successfully");
			})
			.catch((err) => {
				toast.error("Failed to copy link");
				console.error("Failed to copy link: ", err);
			});
	};

	const handleDeletePost = async () => {
		deletePost(post.id);
		navigate("/home");
	};

	return (
		<>
			<div className="w-full flex justify-center">
				<div className="w-5/6 mx-auto py-4 flex gap-3 max-lg:flex-col max-sm:w-full">
					{/* Main Post Content - Left Side */}
					<div className="w-5/6 max-lg:w-full">
						{post ? (
							<div className="flex flex-col gap-3">
								<div className="flex flex-col gap-3 bg-card shadow-md rounded-lg p-4 border border-border">
									{/* Post Header */}
									<div className="flex items-center justify-between pb-3 border-b border-border">
										{/* Back Icon */}
										<div
											onClick={() => {
												navigate(-1);
											}}
											className="cursor-pointer"
										>
											<CircleArrowLeft className="text-muted-foreground hover:text-card-foreground hover:cursor-pointer" />
										</div>

										{/* More */}
										<DropdownMenu>
											<DropdownMenuTrigger>
												<Ellipsis className="text-muted-foreground hover:text-card-foreground hover:cursor-pointer" />
											</DropdownMenuTrigger>
											<DropdownMenuContent>
												{/* Copy */}
												<DropdownMenuItem>
													<div
														className="flex items-center text-card-foreground justify-center gap-2 cursor-pointer"
														onClick={handleCopyLink}
													>
														<Link2 className="w-5" />
														<h6>Copy link</h6>
													</div>
												</DropdownMenuItem>

												{userProfile?.id ===
												post.author.id ? (
													<DropdownMenuItem>
														<div
															className="flex items-center text-card-foreground justify-center gap-2 cursor-pointer"
															onClick={() =>
																setIsEditModalOpen(
																	true
																)
															}
														>
															<Link2 className="w-5" />
															<h6>Edit post</h6>
														</div>
													</DropdownMenuItem>
												) : null}

												{/* Delete */}
												{post.author.id ===
													userProfile?.id && (
													<DropdownMenuItem
														onClick={() =>
															setShowDeleteDialog(
																true
															)
														}
													>
														<div className="flex items-center text-card-foreground justify-center gap-2 cursor-pointer">
															<Trash2 className="w-5 text-red-700" />
															<h6 className="text-red-700">
																Delete post
															</h6>
														</div>
													</DropdownMenuItem>
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
														This action cannot be
														undone. This will
														permanently delete your
														question.
													</AlertDialogDescription>
												</AlertDialogHeader>
												<AlertDialogFooter>
													<AlertDialogCancel>
														Cancel
													</AlertDialogCancel>
													<AlertDialogAction
														onClick={() => {
															handleDeletePost();
															setShowDeleteDialog(
																false
															);
														}}
														className="bg-red-700 hover:bg-red-900"
													>
														Delete
													</AlertDialogAction>
												</AlertDialogFooter>
											</AlertDialogContent>
										</AlertDialog>
									</div>

									{/* Post */}
									<div className="flex flex-col justify-center gap-3">
										{/* Author Info */}
										<div className="flex items-center space-x-3">
											<Link
												to={`/profile/${post.author.id}`}
											>
												{post.author
													.profilePictureUrl ? (
													<img
														src={
															post.author
																.profilePictureUrl
														}
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
											<div>
												{/* Name */}
												<Link
													to={`/profile/${post.author.id}`}
												>
													<h3 className="text-md font-semibold text-card-foreground">
														{post.author.firstName +
															" " +
															post.author
																.lastName}
													</h3>
												</Link>
												{/* Publish Date */}
												<PostTimeStamp
													createdAt={post.createdAt}
													lastModified={
														post.lastModified
													}
												/>
											</div>
										</div>
									</div>

									{/* Post Header */}
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
									<MediaCarousel medias={post.medias} />

									{/* Tags */}
									<PostingTags
										tags={post.tags}
										language={post.language}
									/>

									{/* Post Voting */}
									<PostVoteButtons post={post} />

									{/* Comments */}
									<CommentsSection postId={post.id} />
								</div>
								{userProfile?.id == post.author.id ? (
									<EditPostModal
										post={memoizedPost}
										isOpen={isEditModalOpen}
										onClose={() =>
											setIsEditModalOpen(false)
										}
									/>
								) : null}
							</div>
						) : (
							<p className="text-card-foreground">
								Post not found.
							</p>
						)}
					</div>

					{/* Similar Posts - Right Side */}
					{post && (
						<div className="w-1/6 max-lg:w-full">
							<SimilarPosts currentPostId={post.id} />
						</div>
					)}
				</div>
			</div>
		</>
	);
};

export default FullPostWithComments;
