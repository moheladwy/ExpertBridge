import { useAppSelector } from "@/app/hooks";
import { selectPostById } from "@/features/posts/postsSlice";
import { Link } from "react-router-dom";
import { Post } from "@/features/posts/types";
import { ArrowBigUp } from 'lucide-react';
import { MessageCircle } from 'lucide-react';
import { Ellipsis } from 'lucide-react';
import { Link2 } from 'lucide-react';
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/views/components/custom/dropdown-menu"
import {
  Box,
  Modal,
} from "@mui/material";
import toast from "react-hot-toast";
import { useState } from "react";
import PostVoteButtons from "./PostVoteButtons";

interface PostCardProps {
  postId: string;
}

const PostCard: React.FC<PostCardProps> = ({ postId }) => {
  const post = useAppSelector((state) => selectPostById(state, postId));

  if (!post) return null;

  const [open, setOpen] = useState(false);
  const totalCommentsNumber = post.comments;
  let media;

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

  const handleOpen = () => setOpen(true);
  const handleClose = () => {
    setOpen(false);
  };

  //Manage diffrent media typs
  if (post.medias?.length > 0) {
    if (post.medias[0].type === "Picture") {
      media = (
        <img
          src={post.medias[0].url}
          alt="Post content"
          onClick={handleOpen}
        />
      );
    } else {
      media = (
        <video
          src={post.medias[0].url}
          controls
        />
      );
    }
  }



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
      <div className="flex flex-col gap-3 bg-white shadow-md rounded-lg p-4 border border-gray-200">
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

        {/* Post Title */}
        <Link to={`/feed/${post.id}`}>
          <h2 className="text-lg font-bold text-gray-700">{post.title}</h2>
        </Link>

        {/* Post Content */}
        <p className="text-gray-600">{post.content}</p>

        {/* Media */}
        <div className={`flex justify-center items-center bg-slate-500 w-full aspect-video rounded-md overflow-hidden cursor-pointer ${post.medias?.length > 0 ? "block" : "hidden"}`}>
          {media}
        </div>

        {/* Post Metadata */}
        {/* Tags */}
        {post.postTags?.length > 0 && (
          <div className="flex space-x-2">
            {post.postTags.map((tag: any, index: number) => (
              <span key={index} className="text-xs bg-gray-200 px-2 py-1 rounded-full">
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
            </DropdownMenuContent>
          </DropdownMenu>
        </div>
      </div>
    </>
  );
};

export default PostCard