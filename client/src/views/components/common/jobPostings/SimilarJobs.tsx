import React from "react";
import { useGetSimilarJobsQuery } from "@/features/jobPostings/jobPostingsSlice";
import { Link } from "react-router-dom";

interface SimilarJobsProps {
	currentJobId: string;
}

const SimilarJobs: React.FC<SimilarJobsProps> = ({ currentJobId }) => {
	const {
		data: similarJobs,
		error,
		isLoading,
	} = useGetSimilarJobsQuery(currentJobId, { skip: !currentJobId });

	if (isLoading) {
		return (
			<div className="bg-card rounded-xl border p-6">
				<h3 className="text-lg font-semibold text-card-foreground mb-4">
					Similar Jobs
				</h3>
				<div className="space-y-3">
					{[...Array(3)].map((_, index) => (
						<div key={index} className="animate-pulse space-y-2">
							<div className="h-4 bg-muted rounded"></div>
							<div className="h-3 bg-muted rounded w-3/4"></div>
							<div className="h-3 bg-muted rounded w-1/2"></div>
						</div>
					))}
				</div>
			</div>
		);
	}

	if (error || !similarJobs || similarJobs.length === 0) {
		return (
			<div className="bg-card rounded-xl border p-6">
				<h3 className="text-lg font-semibold text-card-foreground mb-4">
					Similar Jobs
				</h3>
				<p className="text-muted-foreground text-sm">
					No similar jobs found.
				</p>
			</div>
		);
	}

	return (
		<div className="bg-card rounded-xl border p-6 hover:shadow-lg transition-shadow duration-300">
			<div className="flex items-center gap-2 mb-4">
				<div className="inline-block rounded-full bg-primary/10 px-3 py-1 text-xs font-medium text-primary">
					Recommended
				</div>
				<h3 className="text-lg font-semibold text-card-foreground">
					Similar Jobs
				</h3>
			</div>
			<div className="space-y-3">
				{similarJobs.map((job) => (
					<Link
						key={job.jobPostingId}
						to={`/jobs/${job.jobPostingId}`}
						className="group block p-4 rounded-lg border border-border hover:border-primary/50 hover:bg-muted/50 transition-all duration-200"
					>
						<div className="space-y-2">
							{/* Job Title */}
							<h4 className="font-medium text-card-foreground text-sm line-clamp-2 group-hover:text-primary transition-colors">
								{job.title}
							</h4>

							{/* Author */}
							<p className="text-xs text-muted-foreground">
								by{" "}
								<span className="font-medium">
									{job.authorName}
								</span>
							</p>

							{/* Content Preview */}
							<p className="text-xs text-muted-foreground line-clamp-2 leading-relaxed">
								{job.content}
							</p>

							{/* Relevance Score */}
							<div className="flex items-center justify-between pt-2">
								<span className="inline-flex items-center gap-1 text-xs font-medium text-primary bg-primary/10 px-2 py-1 rounded-full">
									{Math.round(
										(1.0 - job.relevanceScore) * 100
									)}
									% match
								</span>
								{job.createdAt && (
									<span className="text-xs text-muted-foreground">
										{new Date(
											job.createdAt
										).toLocaleDateString()}
									</span>
								)}
							</div>
						</div>
					</Link>
				))}
			</div>
		</div>
	);
};

export default SimilarJobs;
