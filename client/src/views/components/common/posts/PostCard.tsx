import { useAppSelector } from "@/app/hooks";
import {
	selectPostById,
	useDeletePostMutation,
} from "@/features/posts/postsSlice";
import { Link, useNavigate } from "react-router-dom";
import { Post } from "@/features/posts/types";
import { ArrowBigUp } from "lucide-react";
import { MessageCircle } from "lucide-react";
import { Ellipsis } from "lucide-react";
import { Link2 } from "lucide-react";
import {
	DropdownMenu,
	DropdownMenuContent,
	DropdownMenuItem,
	DropdownMenuLabel,
	DropdownMenuSeparator,
	DropdownMenuTrigger,
} from "@/views/components/custom/dropdown-menu";
import { Box, Modal } from "@mui/material";
import toast from "react-hot-toast";
import { useEffect, useState } from "react";
import PostVoteButtons from "./PostVoteButtons";
import ReactPlayer from "react-player";
import DeleteIcon from "@mui/icons-material/Delete";
import useIsUserLoggedIn from "@/hooks/useIsUserLoggedIn";
import TimeAgo from "../../custom/TimeAgo";

interface PostCardProps {
	postId: string;
}

const PostCard: React.FC<PostCardProps> = ({ postId }) => {
	const post = useAppSelector((state) => selectPostById(state, postId));
	const [open, setOpen] = useState(false);
	const [, , , , userProfile] = useIsUserLoggedIn();

	const [deletePost, deleteResult] = useDeletePostMutation();

	useEffect(() => {
		if (deleteResult.isSuccess) {
			toast.success("Your post was deleted successfully.");
		}
		if (deleteResult.isError) {
			toast.error("An error occurred while deleting you post.");
			console.log(deleteResult.error);
		}
	}, [deleteResult.isSuccess, deleteResult.isError, deleteResult.error]);

	if (!post) return null;

	const totalCommentsNumber = post.comments;
	let media;

	const handleCopyLink = () => {
		const postUrl = `${window.location.origin}/feed/${post.id}`;
		navigator.clipboard
			.writeText(postUrl)
			.then(() => {
				toast.success("Link copied successfully");
			})
			.catch((err) => {
				toast.error("Failed to copy link");
			});
	};

	const handleOpen = () => setOpen(true);
	const handleClose = () => {
		setOpen(false);
	};

	const handleDeletePost = async () => {
		await deletePost(post.id);
	};

	//Manage diffrent media typs
	if (post.medias?.length > 0) {
		if (post.medias[0].type.startsWith("image")) {
			media = (
				<img
					src={post.medias[0].url!}
					alt="Post content"
					onClick={handleOpen}
				/>
			);
		} else if (post.medias[0].type.startsWith("video")) {
			media = <ReactPlayer url={post.medias[0].url!} controls />;
		}
	}

	// console.log(post);

	return (
		<>
			<Modal
				open={open}
				onClose={handleClose}
				aria-labelledby="create-post-modal"
				className="flex justify-center items-center"
			>
				{post.medias?.[0]?.url ? (
					<img
						src={post.medias[0].url}
						alt="Post content"
						className="max-w-full max-h-[90vh] object-contain"
					/>
				) : (
					<div className="p-4 text-center">
						<p>No media available</p>
					</div>
				)}
			</Modal>
			<div className="flex flex-col gap-3 bg-white shadow-md rounded-lg p-4 border border-gray-200">
				{/* Author Info */}
				<div className="flex items-center space-x-3">
					<Link to={`/profile/${post.author.id}`}>
						<img
							src={post.author.profilePictureUrl}
							// alt={`${post.author.id} Profile`}
							width={40}
							height={40}
							className="rounded-full"
						/>
					</Link>
					<div className="flex w-full justify-between">
						<div>
							<Link to={`/profile/${post.author.id}`}>
								{/* Name */}
								<h3 className="text-md font-semibold">
									{post.author.firstName +
										" " +
										post.author.lastName}
								</h3>
							</Link>
							{/* Publish Date */}
							<div className="flex justify-between items-center text-sm text-gray-500">
								<span>
									<TimeAgo timestamp={post.createdAt} />
								</span>
							</div>
						</div>

						{/* More */}
						<DropdownMenu>
							<DropdownMenuTrigger>
								<Ellipsis className=" text-gray-500 hover:text-gray-700 hover:cursor-pointer" />
							</DropdownMenuTrigger>
							<DropdownMenuContent>
								<DropdownMenuItem>
									<div
										className="flex items-center text-gray-800 justify-center gap-2 cursor-pointer"
										onClick={handleCopyLink}
									>
										<Link2 className="w-5" />
										<h6>Copy link</h6>
									</div>
								</DropdownMenuItem>
								{post.author.id === userProfile?.id && (
									<DropdownMenuItem>
										<div
											className="flex items-center text-gray-800 justify-center gap-2 cursor-pointer"
											onClick={handleDeletePost}
										>
											<DeleteIcon className="w-5 text-red-700" />
											<h6 className="text-red-700">
												Delete post
											</h6>
										</div>
									</DropdownMenuItem>
								)}
							</DropdownMenuContent>
						</DropdownMenu>
					</div>
				</div>

				{/* Post Title */}
				<Link to={`/feed/${post.id}`}>
					<div className="break-words">
						<h2 className="text-lg font-bold text-gray-700 whitespace-pre-wrap">
							{post.title}
						</h2>
					</div>
				</Link>

				{/* Post Content */}
				<div className="break-words">
					<p className="text-gray-600 whitespace-pre-wrap">
						{post.content}
					</p>
				</div>

				{/* Media */}
				<div
					className={`flex justify-center items-center bg-slate-500 w-full aspect-video rounded-md overflow-hidden cursor-pointer ${post.medias?.length > 0 ? "block" : "hidden"}`}
				>
					{post.medias.length > 0 ? (
						post.medias[0].type.startsWith("video") ? (
							<ReactPlayer url={post.medias[0].url} controls />
						) : (
							<img
								src={post.medias[0].url}
								onClick={handleOpen}
								alt="oh shit it did not load..."
							/>
						)
					) : null}
				</div>

				{/* Post Metadata */}
				{/* Tags */}
				{post.postTags?.length > 0 && (
					<div className="flex space-x-2">
						{post.postTags.map((tag: any, index: number) => (
							<span
								key={index}
								className="text-xs bg-gray-200 px-2 py-1 rounded-full"
							>
								{tag.name}
							</span>
						))}
					</div>
				)}

				{/* Interactions */}
				<div className="flex justify-between items-center">
					<div className="flex gap-2 items-center">
						{/* Votes */}
						<PostVoteButtons post={post} />

						{/* Comments */}
						<Link to={`/feed/${post.id}`}>
							<div className="flex items-center gap-2 rounded-full p-1 px-2 hover:bg-gray-200  hover:cursor-pointer">
								<MessageCircle className="text-gray-500" />
								<div className="text-gray-500 text-md font-bold ">
									{totalCommentsNumber}
								</div>
							</div>
						</Link>
					</div>
				</div>
			</div>
		</>
	);
};

export default PostCard;
