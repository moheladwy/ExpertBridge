import React, { useEffect, useState } from "react";
import { Post } from "@/features/posts/types";
import { Button, IconButton, Modal, TextField } from "@mui/material";
import { ThumbUp, ThumbDown, AttachFile } from "@mui/icons-material";
import CommentCard from "../comments/CommentCard";
import { useGetCommentsByPostIdQuery } from "@/features/comments/commentsSlice";
import CommentsSection from "../comments/CommentsSection";
import { useAppSelector } from "@/app/hooks";
import { selectPostById, useDownvotePostMutation, useUpvotePostMutation } from "@/features/posts/postsSlice";
import toast from "react-hot-toast";
import PostVoteButtons from "./PostVoteButtons";
import ReactPlayer from "react-player";

interface FullPostWithCommentsProps {
  post: Post;
}

const FullPostWithComments: React.FC<FullPostWithCommentsProps> = ({ post }) => {
  const [open, setOpen] = useState(false);
  const handleOpen = () => setOpen(true);
  const handleClose = () => setOpen(false);

  if (!post) return <p>Post not found.</p>;

  console.log(post.isUpvoted);

  return (
    <>
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
      <div className="bg-white shadow-md rounded-lg p-4 border border-gray-200 max-w-3xl mx-auto">
        {/* Post Header */}
        <h2 className="text-lg font-semibold mt-3">{post.title}</h2>
        <p className="text-gray-700 mt-2">{post.content}</p>

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

        <CommentsSection postId={post.id} />

      </div>
    </>
  );
};

export default FullPostWithComments;
