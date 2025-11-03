import { useGetSuggestedJobsQuery } from "@/features/jobPostings/jobPostingsSlice";
import useIsUserLoggedIn from "@/hooks/useIsUserLoggedIn";
import { Link } from "react-router";
import LoadingSkeleton from "./LoadingSkeleton";
import useRefetchOnLogin from "@/hooks/useRefetchOnLogin";

// Suggested Jobs Component
const SuggestedJobs = () => {
	const [isLoggedIn] = useIsUserLoggedIn();
	const { data: jobs, isLoading, refetch } = useGetSuggestedJobsQuery(5);

	useRefetchOnLogin(refetch);

	if (isLoading) {
		return (
			<div className="bg-card rounded-lg shadow-md p-4 sticky top-4">
				<h3 className="text-lg font-semibold mb-4 text-card-foreground">
					{isLoggedIn
						? "See Jobs That Match Your Profile"
						: "Suggested Jobs"}
				</h3>
				<LoadingSkeleton count={3} />
			</div>
		);
	}

	const jobPostings = jobs || [];

	return (
		<div className="bg-card rounded-lg shadow-md p-4 sticky top-4">
			<h3 className="text-lg text-center font-semibold mb-4 text-card-foreground">
				{isLoggedIn
					? "See Jobs That Match Your Profile"
					: "Suggested Jobs"}
			</h3>
			<div className="space-y-3">
				{jobPostings.map((job) => (
					<div
						key={job.jobPostingId}
						className="p-3 border border-border rounded-lg hover:bg-accent cursor-pointer transition-colors"
					>
						<Link to={`/job/${job.jobPostingId}`}>
							<h4 className="font-medium text-sm text-card-foreground mb-2 line-clamp-2">
								{job.title}
							</h4>
							<p className="text-xs text-muted-foreground mb-1">
								By {job.authorName}
							</p>
							<p className="text-xs text-muted-foreground mb-2">
								{job.area} • ${job.budget}
							</p>
						</Link>
					</div>
				))}
			</div>
			<Link
				to="/jobs"
				className="block w-full mt-4 text-sm text-blue-600 dark:text-blue-400 hover:underline text-center"
			>
				View all jobs →
			</Link>
		</div>
	);
};

export default SuggestedJobs;
