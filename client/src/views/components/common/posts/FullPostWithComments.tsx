import React, { useEffect, useState } from "react";
import { Post } from "@/features/posts/types";
import { Button, IconButton, TextField } from "@mui/material";
import { ThumbUp, ThumbDown, AttachFile } from "@mui/icons-material";
import CommentCard from "../comments/CommentCard";
import { useGetCommentsByPostIdQuery } from "@/features/comments/commentsSlice";
import CommentsSection from "../comments/CommentsSection";

interface FullPostWithCommentsProps {
  post: Post;
}

const FullPostWithComments: React.FC<FullPostWithCommentsProps> = ({ post }) => {
  const [postVotes, setPostVotes] = useState({
    upvotes: post.upvotes,
    downvotes: post.downvotes,
    userVote: null as "upvote" | "downvote" | null,
  });

  if (!post) return <p>Post not found.</p>;

  const handlePostVote = (type: "upvote" | "downvote") => {
    setPostVotes((prev) => {
      const isUpvote = type === "upvote";
      const isDownvote = type === "downvote";

      if (prev.userVote === type) {
        return {
          upvotes: isUpvote ? prev.upvotes - 1 : prev.upvotes,
          downvotes: isDownvote ? prev.downvotes - 1 : prev.downvotes,
          userVote: null,
        };
      }

      return {
        upvotes: isUpvote ? prev.upvotes + 1 : prev.upvotes - (prev.userVote === "upvote" ? 1 : 0),
        downvotes: isDownvote ? prev.downvotes + 1 : prev.downvotes - (prev.userVote === "downvote" ? 1 : 0),
        userVote: type,
      };
    });
  };

  return (
    <div className="bg-white shadow-md rounded-lg p-4 border border-gray-200 max-w-3xl mx-auto">
      {/* Post Header */}
      <h2 className="text-lg font-semibold mt-3">{post.title}</h2>
      <p className="text-gray-700 mt-2">{post.content}</p>

      {/* Post Voting */}
      <div className="flex justify-between items-center mt-4 text-sm text-gray-500">
        <span>Posted on {new Date(post.createdAt).toLocaleDateString()}</span>
        <div className="flex space-x-4 items-center">
          <IconButton
            color={postVotes.userVote === "upvote" ? "primary" : "default"}
            onClick={() => handlePostVote("upvote")}
          >
            <ThumbUp fontSize="small" />
          </IconButton>
          <span className="text-green-600">{postVotes.upvotes}</span>
          <IconButton
            color={postVotes.userVote === "downvote" ? "secondary" : "default"}
            onClick={() => handlePostVote("downvote")}
          >
            <ThumbDown fontSize="small" />
          </IconButton>
          <span className="text-red-600">{postVotes.downvotes}</span>
        </div>
      </div>

      <CommentsSection postId={post.id} />

    </div>
  );
};

export default FullPostWithComments;
