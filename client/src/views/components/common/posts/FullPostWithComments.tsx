import React, { useEffect, useState } from "react";
import { Post } from "@/features/posts/types";
import { Button, IconButton, TextField } from "@mui/material";
import { ThumbUp, ThumbDown, AttachFile } from "@mui/icons-material";
import CommentCard from "../comments/CommentCard";
import { useGetCommentsByPostIdQuery } from "@/features/comments/commentsSlice";
import CommentsSection from "../comments/CommentsSection";
import { useAppSelector } from "@/app/hooks";
import { selectPostById, useDownvotePostMutation, useUpvotePostMutation } from "@/features/posts/postsSlice";
import toast from "react-hot-toast";
import PostVoteButtons from "./PostVoteButtons";

interface FullPostWithCommentsProps {
  post: Post;
}

const FullPostWithComments: React.FC<FullPostWithCommentsProps> = ({ post }) => {
  if (!post) return <p>Post not found.</p>;

  console.log(post.isUpvoted);

  return (
    <div className="bg-white shadow-md rounded-lg p-4 border border-gray-200 max-w-3xl mx-auto">
      {/* Post Header */}
      <h2 className="text-lg font-semibold mt-3">{post.title}</h2>
      <p className="text-gray-700 mt-2">{post.content}</p>

      {/* Post Voting */}
      <PostVoteButtons post={post} />

      <CommentsSection postId={post.id} />

    </div>
  );
};

export default FullPostWithComments;
