import React from "react";
import { Link } from "react-router-dom";
import { Post } from "@/features/posts/types";
import { MessageCircle, ArrowBigUp, ArrowBigDown } from "lucide-react";
import { Button } from "@/views/components/ui/button";
import TimeAgo from "../../custom/TimeAgo";
import defaultProfile from "../../../../assets/Profile-pic/ProfilePic.svg";

interface ProfilePostCardProps {
  post: Post;
}

const ProfilePostCard: React.FC<ProfilePostCardProps> = ({ post }) => {
  const totalCommentsNumber = post.comments;
  const netVotes = post.upvotes - post.downvotes;
  
  return (
    <div className="flex flex-col gap-3 bg-white shadow-md rounded-lg p-4 border border-gray-200">
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
              <h3 className="text-md font-semibold">
                {post.author.firstName + " " + post.author.lastName}
              </h3>
            </Link>
            <div className="flex justify-between items-center text-sm text-gray-500">
              <span>
                <TimeAgo timestamp={post.createdAt} />
              </span>
            </div>
          </div>
        </div>
      </div>

      {/* Post Title */}
      <div className="break-words">
        <h2 className="text-lg font-bold text-gray-700 whitespace-pre-wrap" dir="auto">
          {post.title}
        </h2>
      </div>

      {/* Post Content */}
      <div className="break-words">
        <p className="text-gray-600 whitespace-pre-wrap line-clamp-3" dir="auto">
          {post.content}
        </p>
      </div>

      {/* Post Metadata */}
      {/* Tags */}
      {post.postTags?.length > 0 && (
        <div className="flex space-x-2">
          {post.postTags.map((tag: any, index: number) => (
            <span
              key={index}
              className="text-xs bg-gray-200 px-2 py-1 rounded-full"
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
          <div className="flex items-center gap-1 text-gray-500">
            <div className="flex items-center">
              {netVotes >= 0 ? (
                <ArrowBigUp className="text-gray-500 w-5 h-5" />
              ) : (
                <ArrowBigDown className="text-gray-500 w-5 h-5" />
              )}
              <span className={`ml-1 ${netVotes < 0 ? 'text-red-500' : 'text-gray-500'}`}>
                {Math.abs(netVotes)}
              </span>
            </div>
          </div>
          
          {/* Comments */}
          <div className="flex items-center gap-1 text-gray-500">
            <MessageCircle className="w-5 h-5" />
            <span>{totalCommentsNumber}</span>
          </div>
        </div>

        {/* View Button */}
        <Button 
          asChild
          variant="outline" 
          size="sm"
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
