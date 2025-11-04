import { useSearchParams } from "react-router-dom";
import { useSearchPostsQuery } from "@/features/search/searchSlice";
import { Skeleton } from "@/views/components/ui/skeleton";
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

	const [, , , , userProfile] = useIsUserLoggedIn();

	// Render loading skeletons
	const renderSkeletons = () => {
		return Array(3)
			.fill(0)
			.map((_, index) => (
				<Skeleton key={index} className="h-48 w-full rounded-lg mb-4" />
			));
	};

	// Render search results or appropriate message
	const renderContent = useCallback(() => {
		if (isLoading) {
			return renderSkeletons();
		}

		if (isError) {
			return (
				<div className="p-8 text-center bg-card rounded-xl border border-destructive/20 shadow-lg">
					<div className="inline-flex items-center justify-center w-16 h-16 rounded-full bg-destructive/10 mb-4">
						<svg
							className="w-8 h-8 text-destructive"
							fill="none"
							stroke="currentColor"
							viewBox="0 0 24 24"
						>
							<path
								strokeLinecap="round"
								strokeLinejoin="round"
								strokeWidth={2}
								d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"
							/>
						</svg>
					</div>
					<h3 className="text-xl font-bold text-card-foreground mb-2">
						Error loading search results
					</h3>
					<p className="text-muted-foreground">
						{error ? String(error) : "An unknown error occurred"}
					</p>
				</div>
			);
		}

		const posts = searchResults
			? Object.values(searchResults.entities).filter(
					(post) => post && post.author
				)
			: [];

		if (posts.length === 0 && searchQuery) {
			return (
				<div className="p-8 text-center bg-card rounded-xl border border-border shadow-lg">
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
								d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"
							/>
						</svg>
					</div>
					<h3 className="text-xl font-bold text-card-foreground mb-2">
						No results found
					</h3>
					<p className="text-muted-foreground">
						We couldn't find any posts matching:{" "}
						<span className="font-semibold text-card-foreground">
							"{searchQuery}"
						</span>
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
			<div className="p-6 bg-card rounded-xl border border-border shadow-lg">
				<div className="mb-6">
					<h2 className="text-2xl font-bold text-card-foreground mb-2">
						Search Results for:{" "}
						<span className="text-primary">{searchQuery}</span>
					</h2>
					{!isLoading && searchResults && (
						<span className="inline-flex items-center px-3 py-1 rounded-full text-sm font-medium bg-primary/10 text-primary">
							{
								Object.values(searchResults.entities).filter(
									(p) => p && p.author
								).length
							}{" "}
							{Object.values(searchResults.entities).filter(
								(p) => p && p.author
							).length === 1
								? "result"
								: "results"}
						</span>
					)}
				</div>
				<div className="space-y-4">{renderContent()}</div>
			</div>
		</div>
	);
};

export default SearchPosts;
