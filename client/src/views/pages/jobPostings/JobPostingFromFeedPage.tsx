import { useAppSelector } from "@/app/hooks";
import { useNavigate, useParams } from "react-router-dom";
import { Navigate } from "react-router-dom";
import { useEffect } from "react";
import toast from "react-hot-toast";
import { Skeleton } from "@/views/components/ui/skeleton";
import {
	selectJobPostingById,
	useDeleteJobPostingMutation,
} from "@/features/jobPostings/jobPostingsSlice";
import FullJobPostingWithComments from "@/views/components/common/jobPostings/FullJobPostingWithComments";

const JobPostingFromFeedPage: React.FC = () => {
	const { jobPostingId } = useParams();

	const jobPosting = useAppSelector((state) =>
		selectJobPostingById(state, jobPostingId!)
	);

	const navigate = useNavigate();
	const [deleteJobPosting, deleteResult] = useDeleteJobPostingMutation();

	useEffect(() => {
		if (deleteResult.isSuccess) {
			toast.success("Your post was deleted successfully.");
			navigate("/home");
		}
		if (deleteResult.isError) {
			toast.error("An error occurred while deleting you post.");
			console.log(deleteResult.error);
		}
	}, [
		deleteResult.isSuccess,
		deleteResult.isError,
		deleteResult.error,
		navigate,
	]);

	if (!jobPosting) {
		return <Navigate to={`/job/${jobPostingId}`} />;
	}

	// Loading skeleton while checking if post exists in state
	if (!jobPosting && !deleteResult.isSuccess) {
		// We'll show a quick loading skeleton before redirecting
		return (
			<div className="w-full flex justify-center">
				<div className="w-2/5 mx-auto p-4 gap-5 max-xl:w-3/5 max-lg:w-4/5 max-sm:w-full">
					<div className="flex flex-col gap-3 bg-white shadow-md rounded-lg p-4 border border-gray-200">
						{/* Post Header Skeleton */}
						<div className="flex items-center justify-between pb-3 border-b border-gray-300">
							<Skeleton className="h-8 w-8 rounded-full" />
							<Skeleton className="h-8 w-8 rounded-full" />
						</div>

						{/* Author Info Skeleton */}
						<div className="flex items-center space-x-3">
							<Skeleton className="h-10 w-10 rounded-full" />
							<div className="space-y-2">
								<Skeleton className="h-4 w-32" />
								<Skeleton className="h-3 w-20" />
							</div>
						</div>

						{/* Post Content Skeleton */}
						<Skeleton className="h-6 w-3/4 mt-2" />
						<div className="space-y-2">
							<Skeleton className="h-4 w-full" />
							<Skeleton className="h-4 w-full" />
							<Skeleton className="h-4 w-4/5" />
						</div>

						{/* Media Skeleton */}
						<Skeleton className="h-72 w-full rounded-md mt-2" />
					</div>
				</div>
			</div>
		);
	}
	return (
		<FullJobPostingWithComments
			jobPosting={jobPosting}
			deleteJobPosting={deleteJobPosting}
		/>
	);
};

export default JobPostingFromFeedPage;
