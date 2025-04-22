import React, { useState } from "react";
import { Link } from "react-router-dom";
import { Post } from "@/features/posts/types";
import CommentsSection from "../comments/CommentsSection";
import PostVoteButtons from "./PostVoteButtons";
import ReactPlayer from "react-player";
import { Modal } from "@mui/material";
import { CircleArrowLeft } from 'lucide-react';
import { Ellipsis } from 'lucide-react';
import { Link2 } from 'lucide-react';
import DeleteIcon from '@mui/icons-material/Delete';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/views/components/custom/dropdown-menu"
import toast from "react-hot-toast";
import useIsUserLoggedIn from "@/hooks/useIsUserLoggedIn";

interface FullPostWithCommentsProps {
  post: Post;
}

const FullPostWithComments: React.FC<FullPostWithCommentsProps> = ({ post }) => {
  const [open, setOpen] = useState(false);
  const handleOpen = () => setOpen(true);
  const handleClose = () => setOpen(false);
  const [,,,, userProfile] = useIsUserLoggedIn();

  const handleCopyLink = () => {
    const postUrl = `${window.location.origin}/feed/${post.id}`;
    navigator.clipboard.writeText(postUrl)
      .then(() => {
        toast.success("Link copied successfully");
      })
      .catch((err) => {
        toast.error("Failed to copy link");
      });
  }

  const handleDeletePost = () => {

  }

  if (!post) return <p>Post not found.</p>;

  console.log(post.isUpvoted);

  return (
    <>
      <div className="w-full flex justify-center">
        <div className="w-2/5 mx-auto p-4 gap-5 max-xl:w-3/5 max-lg:w-4/5 max-sm:w-full">
          <Modal open={open} onClose={handleClose} aria-labelledby="create-post-modal" className="flex justify-center items-center">
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
            {/* Post Header */}
            <div className="flex items-center justify-between pb-3 border-b border-gray-300">
              {/* Back Icon */}
              <Link to={`/home`}>
                <CircleArrowLeft className="text-gray-500 hover:text-gray-700 hover:cursor-pointer"/>
              </Link>

              {/* More */}
              <DropdownMenu>
                <DropdownMenuTrigger>
                  <Ellipsis className=" text-gray-500 hover:text-gray-700 hover:cursor-pointer" />
                </DropdownMenuTrigger>
                <DropdownMenuContent>
                  <DropdownMenuItem>
                    <div className="flex items-center text-gray-800 justify-center gap-2 cursor-pointer" onClick={handleCopyLink}>
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
                        <h6 className="text-red-700">Delete post</h6>
                      </div>
                    </DropdownMenuItem>
                  )}
                </DropdownMenuContent>
              </DropdownMenu>
            </div>

            {/* Post */}
            <div className="flex flex-col justify-center gap-3">
              {/* Author Info */}
              <div className="flex items-center space-x-3">
                <img
                  src={post.author.profilePictureUrl}
                  // alt={`${post.author.id} Profile`}
                  width={40}
                  height={40}
                  className="rounded-full"
                />
                <div>
                  {/* Name */}
                  <h3 className="text-md font-semibold">{post.author.firstName + ' ' + post.author.lastName}</h3>
                  {/* Publish Date */}
                  <div className="flex justify-between items-center text-sm text-gray-500">
                    <span>{new Date(post.createdAt).toLocaleDateString()}</span>
                  </div>
                </div>
              </div>
            </div>

            {/* Post Header */}
            <div className="break-words">
              <h2 className="text-lg font-bold text-gray-700 whitespace-pre-wrap">{post.title}</h2>
            </div>

            {/* Post Content */}
            <div className="break-words">
              <p className="text-gray-600 whitespace-pre-wrap">{post.content}</p>
            </div>

            {/* Media */}
            <div className={`flex justify-center items-center bg-slate-500 w-full aspect-video rounded-md overflow-hidden cursor-pointer ${post.medias?.length > 0 ? "block" : "hidden"}`}>
              {
                post.medias.length > 0 ?
                  (
                    post.medias[0].type.startsWith('video')
                      ? <ReactPlayer url={post.medias[0].url} controls />
                      : <img src={post.medias[0].url} onClick={handleOpen} alt="oh shit it did not load..." />
                  )
                  : null
              }
            </div>

            {/* Post Voting */}
            <PostVoteButtons post={post} />

            {/* Comments */}
            <CommentsSection postId={post.id} />
          </div>
        </div>
      </div>
    </>
  );
};

export default FullPostWithComments;
