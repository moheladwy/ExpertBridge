import React from "react";
import { useGetSimilarPostsQuery } from "@/features/posts/postsSlice";
import { Link } from "react-router-dom";
import { Clock } from "lucide-react";

interface SimilarPostsProps {
	currentPostId: string;
}

const SimilarPosts: React.FC<SimilarPostsProps> = ({ currentPostId }) => {
	const {
		data: similarPosts,
		error,
		isLoading,
	} = useGetSimilarPostsQuery(currentPostId, { skip: !currentPostId });

	if (isLoading) {
		return (
			<div className="space-y-4">
				<h3 className="text-lg font-semibold text-gray-900">
					Similar Posts
				</h3>
				{[...Array(3)].map((_, i) => (
					<div key={i} className="animate-pulse">
						<div className="h-4 bg-gray-200 rounded w-3/4 mb-2"></div>
						<div className="h-3 bg-gray-200 rounded w-full mb-1"></div>
						<div className="h-3 bg-gray-200 rounded w-2/3"></div>
					</div>
				))}
			</div>
		);
	}

	if (error || !similarPosts?.length) {
		return null;
	}

	return (
		<div className="bg-white rounded-lg shadow p-6">
			<h3 className="text-lg font-semibold text-gray-900 mb-4">
				Similar Posts
			</h3>
			<div className="space-y-4">
				{similarPosts.map((post) => (
					<Link
						key={post.postId}
						to={`/posts/${post.postId}`}
						className="block p-4 border border-gray-200 rounded-lg hover:border-blue-300 hover:shadow-md transition-all"
					>
						<h4 className="font-medium text-gray-900 mb-2 line-clamp-2">
							{post.title}
						</h4>
						<p className="text-sm text-gray-600 mb-3 line-clamp-2">
							{post.content}
						</p>

						<div className="flex items-center justify-between text-xs text-gray-500">
							<span className="flex items-center">
								<Clock className="w-3 h-3 mr-1" />
								{new Date(
									post.createdAt || ""
								).toLocaleDateString()}
							</span>
						</div>
					</Link>
				))}
			</div>
		</div>
	);
};

export default SimilarPosts;
