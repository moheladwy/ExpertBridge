import React, { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
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

import toast from "react-hot-toast";
import useIsUserLoggedIn from "@/hooks/useIsUserLoggedIn";
import { useDeletePostMutation } from "@/features/posts/postsSlice";
import TimeAgo from "../../custom/TimeAgo";
import defaultProfile from "../../../../assets/Profile-pic/ProfilePic.svg"
import MediaCarousel from "../media/MediaCarousel";

interface FullPostWithCommentsProps {
  post: Post;
  deletePost: (...args: any) => any;
}

const FullPostWithComments: React.FC<FullPostWithCommentsProps> = ({ post, deletePost }) => {
  const [, , , , userProfile] = useIsUserLoggedIn();

  // conferm delete dialog
  const [showDeleteDialog, setShowDeleteDialog] = useState(false);
  const navigate = useNavigate();

  const handleCopyLink = () => {
    const postUrl = `${window.location.origin}/feed/${post?.id}`;
    navigator.clipboard.writeText(postUrl)
      .then(() => {
        toast.success("Link copied successfully");
      })
      .catch((err) => {
        toast.error("Failed to copy link");
      });
  }

  const handleDeletePost = async () => {
    deletePost(post.id);
    navigate("/home");
  }


  // if (!post) return <p>Post not found.</p>;

  // console.log(post.isUpvoted);

  return (
    <>
      <div className="w-full flex justify-center">
        <div className="w-2/5 mx-auto p-4 gap-5 max-xl:w-3/5 max-lg:w-4/5 max-sm:w-full">
          {
            post ? (
              <>
                
                <div className="flex flex-col gap-3 bg-white shadow-md rounded-lg p-4 border border-gray-200">
                  {/* Post Header */}
                  <div className="flex items-center justify-between pb-3 border-b border-gray-300">
                    {/* Back Icon */}
                    <Link to={`/home`}>
                      <CircleArrowLeft className="text-gray-500 hover:text-gray-700 hover:cursor-pointer" />
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

                        {/* Delete */}
                        {post.author.id === userProfile?.id && (
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

                  {/* Post */}
                  <div className="flex flex-col justify-center gap-3">
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
                      <div>
                        {/* Name */}
                        <Link to={`/profile/${post.author.id}`}>
                          <h3 className="text-md font-semibold">{post.author.firstName + ' ' + post.author.lastName}</h3>
                        </Link>
                        {/* Publish Date */}
                        <div className="flex justify-between items-center text-sm text-gray-500">
                          <span>
                            <TimeAgo timestamp={post.createdAt} />
                          </span>
                        </div>
                      </div>
                    </div>
                  </div>

                  {/* Post Header */}
                  <div className="break-words">
                    <h2 className="text-lg font-bold text-gray-700 whitespace-pre-wrap" dir="auto">{post.title}</h2>
                  </div>

                  {/* Post Content */}
                  <div className="break-words">
                    <p className="text-gray-600 whitespace-pre-wrap" dir="auto">{post.content}</p>
                  </div>

                  <MediaCarousel medias={post.medias} />

                  {/* Post Voting */}
                  <PostVoteButtons post={post} />

                  {/* Comments */}
                  <CommentsSection postId={post.id} />
                </div>
              </>
            )
              :
              <p>Post not found.</p>
          }
        </div>
      </div>
    </>
  );
};

export default FullPostWithComments;
