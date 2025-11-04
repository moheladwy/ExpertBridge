import React from "react";
import { Link } from "react-router-dom";
import { Comment } from "@/features/comments/types";
import TimeAgo from "../../custom/TimeAgo";
import defaultProfile from "../../../../assets/Profile-pic/ProfilePic.svg";

interface ProfileCommentCardProps {
	comment: Comment;
	postTitle?: string;
}

const ProfileCommentCard: React.FC<ProfileCommentCardProps> = ({
	comment,
	postTitle,
}) => {
	const netVotes = comment.upvotes - comment.downvotes;

	return (
		<div className="group flex flex-col gap-3 p-4 border border-border rounded-xl bg-card hover:border-primary/50 hover:shadow-lg transition-all duration-300">
			{postTitle && (
				<div className="mb-2 text-sm text-muted-foreground">
					<span className="font-semibold">On Post: </span>
					<Link
						to={`/feed/${comment.postId}`}
						className="hover:text-primary hover:underline transition-colors"
						dir="auto"
					>
						{postTitle}
					</Link>
				</div>
			)}

			{/* Comment Author */}
			<div className="flex items-center space-x-3">
				{comment.author?.profilePictureUrl ? (
					<img
						src={comment.author.profilePictureUrl}
						alt="Comment Author"
						width={30}
						height={30}
						className="rounded-full"
					/>
				) : (
					<img
						src={defaultProfile}
						alt="Comment Author"
						width={30}
						height={30}
						className="rounded-full"
					/>
				)}
				<div>
					{/* Name */}
					<h4 className="text-sm font-semibold text-card-foreground">
						{comment.author.firstName +
							" " +
							comment.author.lastName}
					</h4>
					{/* Date of creation */}
					<p className="text-xs text-muted-foreground">
						<TimeAgo timestamp={comment.createdAt} />
					</p>
				</div>
			</div>

			{/* Comment Content */}
			<div className="w-full wrap-break-word">
				<p
					className="text-card-foreground whitespace-pre-wrap leading-relaxed"
					dir="auto"
				>
					{comment.content}
				</p>
			</div>

			{/* Vote Display */}
			<div className="flex items-center justify-between pt-2">
				<div className="flex items-center gap-1">
					<span
						className={`font-medium text-sm px-2 py-1 rounded-full ${
							netVotes > 0
								? "bg-green-100 text-green-700"
								: netVotes < 0
									? "bg-red-100 text-red-700"
									: "bg-muted text-muted-foreground"
						}`}
					>
						{netVotes > 0 ? "+" : ""}
						{netVotes} votes
					</span>
				</div>

				{/* View post link */}
				<Link
					to={`/feed/${comment.postId}`}
					className="text-xs font-medium text-primary hover:text-primary/80 transition-colors"
				>
					View Discussion →
				</Link>
			</div>
		</div>
	);
};

export default ProfileCommentCard;
