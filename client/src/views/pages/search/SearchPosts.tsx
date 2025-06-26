import { useSearchParams } from "react-router-dom";
import { useSearchPostsQuery } from "@/features/search/searchSlice";
import { Skeleton } from "@mui/material";
import PostCard from "@/views/components/common/posts/PostCard";
import useIsUserLoggedIn from "@/hooks/useIsUserLoggedIn";
import { useCallback } from "react";

const SearchPosts = () => {
	const [searchParams] = useSearchParams();
	const searchQuery = searchParams.get("query") || "";

	const {
		data: searchResults,
		isLoading,
		isError,
		error,
	} = useSearchPostsQuery(searchQuery, { skip: !searchQuery });

	const [isLoggedIn, loginLoading, loginError, authUser, userProfile] =
		useIsUserLoggedIn();

	// Render loading skeletons
	const renderSkeletons = () => {
		return Array(3)
			.fill(0)
			.map((_, index) => <Skeleton key={index} />);
	};

	// Render search results or appropriate message
	const renderContent = useCallback(() => {
		if (isLoading) {
			return renderSkeletons();
		}

		if (isError) {
			return (
				<div className="p-6 text-center bg-white dark:bg-gray-800 rounded-lg shadow">
					<h3 className="text-lg font-medium text-red-600">
						Error loading search results
					</h3>
					<p className="mt-2 text-gray-600 dark:text-gray-300">
						{error ? String(error) : "An unknown error occurred"}
					</p>
				</div>
			);
		}

		const posts = searchResults
			? Object.values(searchResults.entities)
			: [];

		if (posts.length === 0 && searchQuery) {
			return (
				<div className="p-6 text-center bg-white dark:bg-gray-800 rounded-lg shadow">
					<h3 className="text-lg font-medium">No results found</h3>
					<p className="mt-2 text-gray-600 dark:text-gray-300">
						We couldn't find any posts matching: "{searchQuery}"
					</p>
				</div>
			);
		}

		return posts.map((post) => (
			<PostCard key={post.id} post={post} currUserId={userProfile?.id} />
		));
	}, [
		error,
		isError,
		isLoading,
		searchQuery,
		searchResults,
		userProfile?.id,
	]);

	return (
		<div className="container mx-auto px-4 py-8 max-w-5xl">
			<div className="p-6 bg-white dark:bg-gray-800 rounded-lg shadow">
				<h2 className="text-2xl font-semibold mb-4">
					Search Results for: {searchQuery}
				</h2>
				<div className="space-y-4">{renderContent()}</div>
			</div>
		</div>
	);
};

export default SearchPosts;
