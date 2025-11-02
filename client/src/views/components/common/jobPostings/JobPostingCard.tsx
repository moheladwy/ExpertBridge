import React from "react";
import { Link } from "react-router-dom";
import {
	MapPinIcon,
	CurrencyDollarIcon,
	StarIcon,
	ChatBubbleLeftIcon,
} from "@heroicons/react/24/outline";
import { StarIcon as StarIconSolid } from "@heroicons/react/24/solid";
import { JobPosting } from "@/features/jobPostings/types";
import TimeAgo from "../../custom/TimeAgo";
import JobPostingVoteButtons from "./JobPostingVoteButtons";
import PostingTags from "../tags/PostingTags";

interface JobCardProps {
	job: JobPosting;
	currUserId?: string;
}

const JobPostingCard: React.FC<JobCardProps> = ({ job, currUserId }) => {
	const formatBudget = (budget: number) => {
		if (budget >= 1000) {
			return `$${(budget / 1000).toFixed(1)}k`;
		}
		return `$${budget}`;
	};

	const renderStars = (rating: number) => {
		const stars = [];
		const fullStars = Math.floor(rating);
		const hasHalfStar = rating % 1 !== 0;

		for (let i = 0; i < fullStars; i++) {
			stars.push(
				<StarIconSolid key={i} className="h-4 w-4 text-yellow-400" />
			);
		}

		if (hasHalfStar) {
			stars.push(
				<div key="half" className="relative">
					<StarIcon className="h-4 w-4 text-gray-300" />
					<StarIconSolid
						className="absolute inset-0 h-4 w-4 text-yellow-400 overflow-hidden"
						style={{ width: "50%" }}
					/>
				</div>
			);
		}

		const remainingStars = 5 - Math.ceil(rating);
		for (let i = 0; i < remainingStars; i++) {
			stars.push(
				<StarIcon
					key={`empty-${i}`}
					className="h-4 w-4 text-gray-300"
				/>
			);
		}

		return stars;
	};

	return (
		<div className="bg-white dark:bg-gray-800 rounded-lg shadow-md hover:shadow-lg transition-shadow duration-200 p-6 border border-gray-100 dark:border-gray-700">
			<Link to={`/jobs/${job.id}`}>
				{/* Header with Client Info */}
				<div className="flex items-start justify-between mb-4">
					<div className="flex items-center space-x-3">
						<img
							src={
								job.author.profilePictureUrl ||
								`https://images.pexels.com/photos/220453/pexels-photo-220453.jpeg?auto=compress&cs=tinysrgb&w=64&h=64&dpr=1`
							}
							alt={job.author.firstName + job.author.lastName}
							className="w-12 h-12 rounded-full object-cover"
						/>
						<div>
							<h4 className="font-semibold text-gray-900 dark:text-white">
								{job.author.firstName + job.author.lastName}
							</h4>
							<div className="flex items-center space-x-2">
								{job.author.rating && (
									<div className="flex items-center space-x-1">
										<div className="flex">
											{renderStars(job.author.rating)}
										</div>
										<span className="text-sm text-gray-600 dark:text-gray-400">
											{job.author.rating.toFixed(1)}
										</span>
									</div>
								)}
								<span className="text-sm text-gray-500 dark:text-gray-400">
									<TimeAgo timestamp={job.createdAt} />
								</span>
							</div>
						</div>
					</div>

					{/* Budget Badge */}
					<div className="flex items-center bg-green-100 dark:bg-green-900 text-green-800 dark:text-green-200 px-3 py-1 rounded-full">
						<CurrencyDollarIcon className="h-4 w-4 mr-1" />
						<span className="font-semibold">
							{formatBudget(job.budget)}
						</span>
					</div>
				</div>

				{/* Job Title */}
				<h3 className="text-xl font-bold text-gray-900 dark:text-white mb-3 line-clamp-2">
					{job.title}
				</h3>

				{/* Job Description */}
				<p className="text-gray-700 dark:text-gray-300 mb-4 line-clamp-3">
					{job.content.substring(0, 100)}...
				</p>

				{/* Area/Location */}
				{job.area && (
					<div className="flex items-center text-gray-600 dark:text-gray-400 mb-4">
						<MapPinIcon className="h-4 w-4 mr-1" />
						<span className="text-sm">{job.area}</span>
					</div>
				)}

				{/* Skills/Tags */}
				<PostingTags tags={job.tags} language={job.language} />
			</Link>

			{/* Footer with Actions and Stats */}
			<div className="flex items-center justify-between pt-4 border-t border-gray-100 dark:border-gray-700">
				<div className="flex items-center space-x-4 text-sm text-gray-500 dark:text-gray-400">
					<JobPostingVoteButtons jobPosting={job} />
					<div className="flex items-center space-x-1">
						<ChatBubbleLeftIcon className="h-4 w-4" />
						<span>{job.comments}</span>
					</div>
				</div>
			</div>
		</div>
	);
};

export default JobPostingCard;
