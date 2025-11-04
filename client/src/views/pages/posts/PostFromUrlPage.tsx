import {
	useDeletePostMutation,
	useGetPostQuery,
} from "@/features/posts/postsSlice";
import { useNavigate, useParams } from "react-router-dom";
import FullPostWithComments from "../../components/common/posts/FullPostWithComments";
import { useEffect } from "react";
import useRefetchOnLogin from "@/hooks/useRefetchOnLogin";
import toast from "react-hot-toast";
import { Skeleton } from "@/views/components/ui/skeleton";

const PostFromUrlPage: React.FC = () => {
	const { postId } = useParams();
	const {
		data: post,
		isFetching,
		error,
		refetch,
	} = useGetPostQuery(postId ?? "");

	useRefetchOnLogin(refetch);

	const navigate = useNavigate();
	const [deletePost, deleteResult] = useDeletePostMutation();

	useEffect(() => {
		console.log("use effecting!!!.......................");
		console.log(deleteResult.isSuccess);
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

	if (isFetching) {
		return (
			<div className="w-full flex justify-center">
				<div className="w-2/5 mx-auto p-4 gap-5 max-xl:w-3/5 max-lg:w-4/5 max-sm:w-full">
					<div className="flex flex-col gap-3 bg-card rounded-xl p-6 border border-border shadow-lg">
						{/* Post Header Skeleton */}
						<div className="flex items-center justify-between pb-3 border-b border-border">
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
						<Skeleton className="h-72 w-full rounded-xl mt-2" />

						{/* Post Actions Skeleton */}
						<div className="flex justify-between mt-2">
							<Skeleton className="h-8 w-24 rounded-full" />
							<Skeleton className="h-8 w-24 rounded-full" />
						</div>

						{/* Comments Section Skeleton */}
						<div className="mt-6 space-y-4">
							<Skeleton className="h-5 w-32" />
							<div className="flex gap-3">
								<Skeleton className="h-10 w-10 rounded-full" />
								<Skeleton className="h-20 w-full rounded-xl" />
							</div>
						</div>
					</div>
				</div>
			</div>
		);
	}

	if (error || !post) {
		return (
			<div className="w-full flex justify-center mt-10">
				<div className="w-2/5 mx-auto p-4 max-xl:w-3/5 max-lg:w-4/5 max-sm:w-full">
					<div className="bg-card rounded-xl p-8 border border-border shadow-lg text-center">
						<div className="mb-4">
							<div className="inline-flex items-center justify-center w-16 h-16 rounded-full bg-muted/50 mb-4">
								<svg
									className="w-8 h-8 text-muted-foreground"
									fill="none"
									stroke="currentColor"
									viewBox="0 0 24 24"
								>
									<path
										strokeLinecap="round"
										strokeLinejoin="round"
										strokeWidth={2}
										d="M9.172 16.172a4 4 0 015.656 0M9 10h.01M15 10h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
									/>
								</svg>
							</div>
						</div>
						<h2 className="text-2xl font-bold text-card-foreground mb-3">
							Post Not Found
						</h2>
						<p className="text-muted-foreground mb-6">
							The post you're looking for might have been removed
							or is temporarily unavailable.
						</p>
						<button
							onClick={() => navigate("/home")}
							className="px-6 py-2.5 bg-primary text-primary-foreground rounded-full hover:bg-primary/90 transition-all duration-200 font-medium"
						>
							Return to Home
						</button>
					</div>
				</div>
			</div>
		);
	}

	return (
		<div>
			<FullPostWithComments post={post} deletePost={deletePost} />
		</div>
	);
};

export default PostFromUrlPage;
