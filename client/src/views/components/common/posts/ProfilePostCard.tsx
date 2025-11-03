import React from "react";
import { Link } from "react-router-dom";
import { Post } from "@/features/posts/types";
import { ArrowBigDown, ArrowBigUp, MessageCircle } from "lucide-react";
import { Button } from "@/views/components/ui/button";
import defaultProfile from "../../../../assets/Profile-pic/ProfilePic.svg";
import PostTimeStamp from "./PostTimeStamp";

interface ProfilePostCardProps {
	post: Post;
}

const ProfilePostCard: React.FC<ProfilePostCardProps> = ({ post }) => {
	const totalCommentsNumber = post.comments;
	const netVotes = post.upvotes - post.downvotes;

	return (
		<div className="flex flex-col gap-3 bg-card shadow-md rounded-lg p-4 border border-border">
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
							<h3 className="text-md font-semibold text-card-foreground">
								{post.author.firstName +
									" " +
									post.author.lastName}
							</h3>
						</Link>
						<PostTimeStamp
							createdAt={post.createdAt}
							lastModified={post.lastModified}
						/>
					</div>
				</div>
			</div>

			{/* Post Title */}
			<div className="break-words">
				<h2
					className="text-lg font-bold text-card-foreground whitespace-pre-wrap"
					dir="auto"
				>
					{post.title}
				</h2>
			</div>

			{/* Post Content */}
			<div className="break-words">
				<p
					className="text-muted-foreground whitespace-pre-wrap line-clamp-3"
					dir="auto"
				>
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
							className="text-xs bg-muted text-muted-foreground px-2 py-1 rounded-full"
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
					<div className="flex items-center gap-1 text-muted-foreground">
						<div className="flex items-center">
							{netVotes >= 0 ? (
								<ArrowBigUp className="text-muted-foreground w-5 h-5" />
							) : (
								<ArrowBigDown className="text-muted-foreground w-5 h-5" />
							)}
							<span
								className={`ml-1 ${netVotes < 0 ? "text-destructive" : "text-muted-foreground"}`}
							>
								{Math.abs(netVotes)}
							</span>
						</div>
					</div>

					{/* Comments */}
					<div className="flex items-center gap-1 text-muted-foreground">
						<MessageCircle className="w-5 h-5" />
						<span>{totalCommentsNumber}</span>
					</div>
				</div>

				{/* View Button */}
				<Button
					asChild
					variant="outline"
					size="sm"
					className="text-muted-foreground bg-background hover:bg-accent"
				>
					<Link to={`/feed/${post.id}`}>View Post</Link>
				</Button>
			</div>
		</div>
	);
};

export default ProfilePostCard;
