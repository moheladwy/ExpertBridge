import React from "react";
import { useGetSimilarPostsQuery } from "@/features/posts/postsSlice";
import { Link } from "react-router-dom";
import { Clock } from "lucide-react";
import { Skeleton } from "../../ui/skeleton";

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
				<h3 className="text-lg font-semibold">Similar Posts</h3>
				{[...Array(3)].map((_, i) => (
					<Skeleton key={i} className="h-16 w-full" />
				))}
			</div>
		);
	}

	if (error) {
		console.error("Error fetching similar posts:", error);
		return null;
	}

	if (
		similarPosts == null ||
		similarPosts == undefined ||
		similarPosts.length == 0
	) {
		return (
			<div className="text-muted-foreground">No similar posts found.</div>
		);
	}

	return (
		<div className="sticky top-4 bg-card rounded-xl border p-6 hover:shadow-lg transition-shadow duration-300">
			<div className="flex items-center gap-2 mb-4">
				<div className="inline-block rounded-full bg-primary/10 px-3 py-1 text-xs font-medium text-primary">
					Related
				</div>
				<h3 className="font-semibold text-card-foreground">
					Similar Posts
				</h3>
			</div>
			<div className="space-y-3">
				{similarPosts.map((post) => (
					<Link
						key={post.postId}
						to={`/posts/${post.postId}`}
						className="group block p-3 border border-border rounded-lg hover:border-primary/50 hover:bg-muted/50 transition-all duration-200"
						dir="auto"
					>
						<h4 className="font-medium text-sm text-card-foreground mb-1 line-clamp-2 group-hover:text-primary transition-colors">
							{post.title}
						</h4>
						<p className="text-xs text-muted-foreground mb-2 line-clamp-2 leading-relaxed">
							{post.content}
						</p>

						<div className="flex items-center text-xs text-muted-foreground pt-1">
							<Clock className="w-3 h-3 mr-1" />
							{new Date(
								post.createdAt || ""
							).toLocaleDateString()}
						</div>
					</Link>
				))}
			</div>
		</div>
	);
};

export default SimilarPosts;
