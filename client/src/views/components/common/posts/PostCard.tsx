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
import {
  Carousel,
  CarouselContent,
  CarouselItem,
  CarouselNext,
  CarouselPrevious,
} from "@/views/components/ui/carousel"
import { Box, Modal } from "@mui/material";
import toast from "react-hot-toast";
import { useEffect, useMemo, useState } from "react";
import PostVoteButtons from "./PostVoteButtons";
import ReactPlayer from "react-player";
import DeleteIcon from "@mui/icons-material/Delete";
import useIsUserLoggedIn from "@/hooks/useIsUserLoggedIn";
import TimeAgo from "../../custom/TimeAgo";
import defaultProfile from "../../../../assets/Profile-pic/ProfilePic.svg"
import { ProfileResponse } from "@/features/profiles/types";
import { Edit as EditIcon } from "lucide-react";
import EditPostModal from "./EditPostModal";



interface PostCardProps {
	postId: string;
	currUserId?: string | null;
}

const PostCard: React.FC<PostCardProps> = ({ postId, currUserId }) => {
	const memoizedPostId = useMemo(() => postId, [postId]);
	const post = useAppSelector((state) => selectPostById(state, memoizedPostId));
	const [open, setOpen] = useState(false);
  const [picToBeOpened, setPicToBeOpened] = useState(0);
  const [activeMediaIndex, setActiveMediaIndex] = useState(0);

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

	const handleOpen = (index: number) => {
    setPicToBeOpened(index);
    setOpen(true)
  };

	const handleClose = () => {
		setOpen(false);
	};

	const handleDeletePost = async () => {
		await deletePost(post.id);
	};

	//Manage diffrent media typs
	// if (post.medias?.length > 0) {
	// 	if (post.medias[0].type.startsWith("image")) {
	// 		media = (
	// 			<img
	// 				src={post.medias[0].url!}
	// 				alt="Post content"
	// 				onClick={handleOpen}
	// 			/>
	// 		);
	// 	} else if (post.medias[0].type.startsWith("video")) {
	// 		media = <ReactPlayer url={post.medias[0].url!} controls />;
	// 	}
	// }

	// console.log(post);


	return (
		<>
			<Modal
				open={open}
				onClose={handleClose}
				aria-labelledby="create-post-modal"
				className="flex justify-center items-center"
			>
				{post.medias?.[picToBeOpened]?.url ? (
					<img
						src={post.medias[picToBeOpened].url}
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
						{
							post.author.profilePictureUrl ?
								<img
									src={post.author.profilePictureUrl}
									width={40}
									height={40}
									className="rounded-full"
								/>
								: <img
									src={defaultProfile}
									width={40}
									height={40}
									className="rounded-full"
								/>
						}
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
								{post.author.id === currentUserId && (
								  <>
                    {/* Edit */}
										<DropdownMenuItem>
                      <div
                        className="flex items-center text-gray-800 justify-center gap-2 cursor-pointer"
                        onClick={() => setIsEditModalOpen(true)}
                      >
                        <EditIcon className="w-5" />
                        <h6>Edit post</h6>
                      </div>
                    </DropdownMenuItem>
                    {/* Delete */}
                    <DropdownMenuItem onClick={() => setShowDeleteDialog(true)}>
                      <div
                        className="flex items-center text-gray-800 justify-center gap-2 cursor-pointer"
                        >
                        <DeleteIcon className="w-5 text-red-700" />
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
            <AlertDialog open={showDeleteDialog} onOpenChange={setShowDeleteDialog}>
              <AlertDialogContent>
                <AlertDialogHeader>
                  <AlertDialogTitle>Are you absolutely sure?</AlertDialogTitle>
                  <AlertDialogDescription>
                    This action cannot be undone. This will permanently delete your question.
                  </AlertDialogDescription>
                </AlertDialogHeader>
                <AlertDialogFooter>
                  <AlertDialogCancel>Cancel</AlertDialogCancel>
                  <AlertDialogAction
                    onClick={() => {
                      handleDeletePost();
                      setShowDeleteDialog(false);
                    }}
                    className="bg-red-700 hover:bg-red-900">
                      Delete
                  </AlertDialogAction>
                </AlertDialogFooter>
              </AlertDialogContent>
            </AlertDialog>
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
          className={`aspect-auto flex justify-center items-center w-full rounded-md`}
        >
          <Carousel onSlideChange={(index: number) => setActiveMediaIndex(index)}>
            <CarouselContent>
              {post.medias.map((media, index) => (
                <CarouselItem className="cursor-pointer">
                  {media.type.startsWith("video") ? (
                    <ReactPlayer
                      url={media.url}
                      width="100%"
                      height="100%"
                      controls
                      style={{ pointerEvents: "none" }}
                    />
                  ) : (
                    <img
                      src={media.url}
                      alt={`media-${index}`}
                      onClick={() => handleOpen(index)}
                      className="w-full h-full object-cover"
                    />
                  )}
                </CarouselItem>
              ))}

            </CarouselContent>
              {/* Carousel Controls (overlayed inside the media) */}
              {post.medias.length > 1 && (
                <>
                  <div className="absolute top-1/2 left-14 -translate-y-1/2 z-20">
                    <CarouselPrevious />
                  </div>
                  <div className="absolute top-1/2 right-14 -translate-y-1/2 z-10">
                    <CarouselNext />
                  </div>
                </>
              )}
          </Carousel>
        </div>

        {/* Media Dots */}
        {
          post.medias.length > 1 && 
          <div className="flex justify-center items-center mt-1 gap-2">
            {post.medias.map((_, index) => (
              <span
                key={index}
                className={`w-2 max-md:w-1.5 h-2 max-md:h-1.5 rounded-full ${
                  index === activeMediaIndex ? "bg-main-blue" : "bg-gray-400"
                }`}
              />
            ))}
          </div>
        }

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
      <EditPostModal
        post={post}
        isOpen={isEditModalOpen}
        onClose={() => setIsEditModalOpen(false)}
      />
		</>
	);
};

export default PostCard;
