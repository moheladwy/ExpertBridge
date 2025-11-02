import { useGetSuggestedPostsQuery } from "@/features/posts/postsSlice";
import useIsUserLoggedIn from "@/hooks/useIsUserLoggedIn";
import { Link } from "react-router";
import useRefetchOnLogin from "@/hooks/useRefetchOnLogin";
import LoadingSkeleton from "../posts/LoadingSkeleton";

// Suggested Posts/Questions Component
const SuggestedQuestions = () => {
	const [isLoggedIn] = useIsUserLoggedIn();
	const { data: posts, isLoading, refetch } = useGetSuggestedPostsQuery(5);

	useRefetchOnLogin(refetch);

	if (isLoading) {
		return (
			<div className="bg-white dark:bg-gray-800 rounded-lg shadow-md p-4 sticky top-4">
				<h3 className="text-lg font-semibold mb-4 text-gray-900 dark:text-white">
					{isLoggedIn
						? "Questions You Might Answer"
						: "Suggested Questions"}
				</h3>
				<LoadingSkeleton count={3} />
			</div>
		);
	}

	const suggestedPosts = posts || [];

	return (
		<div className="bg-white dark:bg-gray-800 rounded-lg shadow-md p-4 sticky top-4">
			<h3 className="text-lg text-center font-semibold mb-4 text-gray-900 dark:text-white">
				{isLoggedIn
					? "Questions You Might Answer"
					: "Suggested Questions"}
			</h3>
			<div className="space-y-3">
				{suggestedPosts.map((post) => (
					<div
						key={post.postId}
						className="p-3 border border-gray-200 dark:border-gray-700 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 cursor-pointer transition-colors"
					>
						<Link to={`/posts/${post.postId}`}>
							<h4 className="font-medium text-sm text-gray-900 dark:text-white mb-2 line-clamp-2">
								{post.title ||
									post.content.substring(0, 80) +
										(post.content.length > 80 ? "..." : "")}
							</h4>
							<p className="text-xs text-gray-600 dark:text-gray-400 mb-1">
								By {post.authorName}
							</p>
							{post.createdAt && (
								<p className="text-xs text-gray-500 dark:text-gray-500 mb-2">
									{new Date(
										post.createdAt
									).toLocaleDateString()}
								</p>
							)}
						</Link>
					</div>
				))}
			</div>
			<Link
				to="/posts"
				className="block w-full mt-4 text-sm text-blue-600 dark:text-blue-400 hover:underline text-center"
			>
				View all questions →
			</Link>
		</div>
	);
};

export default SuggestedQuestions;
