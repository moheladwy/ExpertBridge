import React from "react";
import {Link} from "react-router-dom";
import {Post} from "@/features/posts/types";
import {ArrowBigDown, ArrowBigUp, MessageCircle} from "lucide-react";
import {Button} from "@/views/components/ui/button";
import TimeAgo from "@/views/components/custom/TimeAgo";
import defaultProfile from "../../../../assets/Profile-pic/ProfilePic.svg";
import PostTimeStamp from "./PostTimeStamp";

interface ProfilePostCardProps {
  post: Post;
}

const ProfilePostCard: React.FC<ProfilePostCardProps> = ({ post }) => {
  const totalCommentsNumber = post.comments;
  const netVotes = post.upvotes - post.downvotes;

  return (
      <div
          className="flex flex-col gap-3 bg-white dark:bg-gray-800 shadow-md rounded-lg p-4 border border-gray-200 dark:border-gray-700">
        {/* Author Info */}
        <div className="flex items-center space-x-3">
          <Link to={`/profile/${post.author.id}`}>
            {post.author.profilePictureUrl ? (
                <img
                    src={post.author.profilePictureUrl}
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
          <div className="flex w-full justify-between">
            <div>
              <Link to={`/profile/${post.author.id}`}>
                <h3 className="text-md font-semibold dark:text-white">
                  {post.author.firstName + " " + post.author.lastName}
                </h3>
              </Link>
              <PostTimeStamp createdAt={post.createdAt} lastModified={post.lastModified} />
            </div>
          </div>
        </div>

        {/* Post Title */}
        <div className="break-words">
          <h2 className="text-lg font-bold text-gray-700 dark:text-gray-200 whitespace-pre-wrap" dir="auto">
            {post.title}
          </h2>
        </div>

        {/* Post Content */}
        <div className="break-words">
          <p className="text-gray-600 dark:text-gray-300 whitespace-pre-wrap line-clamp-3" dir="auto">
            {post.content}
          </p>
        </div>

        {/* Post Metadata */}
        {/* Tags */}
        {post.tags?.length > 0 && (
            <div className="flex space-x-2">
              {post.tags.map((tag: any, index: number) => (
                  <span
                      key={index}
                      className="text-xs bg-gray-200 dark:bg-gray-700 dark:text-gray-200 px-2 py-1 rounded-full"
                      dir="auto"
                  >
              {tag.name}
            </span>
              ))}
            </div>
        )}

        {/* Footer */}
        <div className="flex justify-between items-center mt-2">
          <div className="flex space-x-4">
            {/* Votes Display */}
            <div className="flex items-center gap-1 text-gray-500 dark:text-gray-400">
              <div className="flex items-center">
                {netVotes >= 0 ? (
                    <ArrowBigUp className="text-gray-500 dark:text-gray-400 w-5 h-5"/>
                ) : (
                    <ArrowBigDown className="text-gray-500 dark:text-gray-400 w-5 h-5"/>
                )}
                <span
                    className={`ml-1 ${netVotes < 0 ? 'text-red-500 dark:text-red-400' : 'text-gray-500 dark:text-gray-400'}`}>
                {Math.abs(netVotes)}
              </span>
              </div>
            </div>

            {/* Comments */}
            <div className="flex items-center gap-1 text-gray-500 dark:text-gray-400">
              <MessageCircle className="w-5 h-5"/>
              <span>{totalCommentsNumber}</span>
            </div>
          </div>

          {/* View Button */}
          <Button
              asChild
              variant="outline"
              size="sm"
              className="text-gray-500 dark:text-gray-400 dark:bg-gray-900 hover:bg-gray-100 dark:hover:bg-gray-700"
          >
            <Link to={`/feed/${post.id}`}>
              View Post
            </Link>
          </Button>
        </div>
      </div>
  );
};

export default ProfilePostCard;