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
      <div className="text-gray-500 dark:text-gray-400">
        No similar posts found.
      </div>
    );
  }

  return (
    <div className="sticky top-4 bg-white dark:bg-gray-800 p-2 rounded-xl shadow-md">
      <h3 className="text-center font-semibold text-gray-900 dark:text-gray-100 mb-3">
        Similar Posts
      </h3>
      <div className="space-y-3">
        {similarPosts.map((post) => (
          <Link
            key={post.postId}
            to={`/posts/${post.postId}`}
            className="block p-3 border border-gray-200 dark:border-gray-700 rounded-lg hover:border-blue-300 dark:hover:border-blue-600 hover:shadow-sm dark:hover:shadow-gray-900/30 transition-all"
            dir="auto"
          >
            <h4 className="font-medium text-sm text-gray-900 dark:text-gray-100 mb-1 line-clamp-2">
              {post.title}
            </h4>
            <p className="text-xs text-gray-600 dark:text-gray-300 mb-2 line-clamp-2">
              {post.content}
            </p>

            <div className="flex items-center justify-between text-xs text-gray-500 dark:text-gray-400">
              <span className="flex items-center">
                <Clock className="w-3 h-3 mr-1" />
                {new Date(post.createdAt || "").toLocaleDateString()}
              </span>
            </div>
          </Link>
        ))}
      </div>
    </div>
  );
};

export default SimilarPosts;
