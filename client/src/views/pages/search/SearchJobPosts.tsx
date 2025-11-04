import { JobPosting } from "@/features/jobPostings/types";
import { useSearchJobPostsQuery } from "@/features/search/searchSlice";
import JobPostingCard from "@/views/components/common/jobPostings/JobPostingCard";
import { useEffect, useState } from "react";
import { useSearchParams } from "react-router";
import {
	MagnifyingGlassIcon,
	ExclamationCircleIcon,
} from "@heroicons/react/24/outline";
import SearchSidebar from "@/views/components/common/search/SearchSidebar";

const POSTS_LIMIT = 100;

const SearchJobPosts = () => {
	const [searchParams] = useSearchParams();
	const searchQuery: string = searchParams.get("query") || "";
	const area: string = searchParams.get("area") || "";
	const minBudget: number = parseInt(searchParams.get("minBudget") || "0");
	const maxBudget: number = parseInt(searchParams.get("maxBudget") || "0");
	const isRemote: boolean = searchParams.get("isRemote") === "true";
	const [sortedPosts, setSortedPosts] = useState<JobPosting[]>([]);

	const {
		data: posts,
		isLoading: isPostsLoading,
		isError: isPostsError,
		error: postsError,
		isFetching: isPostsFetching,
	} = useSearchJobPostsQuery({
		query: searchQuery,
		limit: POSTS_LIMIT,
		area: area,
		minBudget: minBudget,
		maxBudget: maxBudget,
		isRemote: isRemote,
	});

	useEffect(() => {
		if (!posts) return;
		const data = Object.values(posts.entities);
		const sorted = data.sort((a, b) => {
			// First sort by relevance score (if available)
			if (
				a.relevanceScore !== undefined &&
				b.relevanceScore !== undefined
			) {
				// Higher relevance score first
				if (b.relevanceScore !== a.relevanceScore) {
					return a.relevanceScore - b.relevanceScore;
				}
			}
			// Then sort by creation date (newest first)
			return b.createdAt.localeCompare(a.createdAt);
		});
		setSortedPosts(sorted);
	}, [posts]);

	if (isPostsLoading || isPostsFetching) {
		return (
			<div className="container mx-auto px-4 py-8 flex flex-col md:flex-row gap-6">
				<div className="md:w-1/4 bg-card rounded-xl border border-border shadow-lg p-4 h-fit">
					<div className="h-8 w-3/4 bg-muted rounded animate-pulse mb-4"></div>
					{[...Array(5)].map((_, i) => (
						<div key={i} className="mb-6">
							<div className="h-5 w-full bg-muted rounded animate-pulse mb-2"></div>
							<div className="h-8 w-full bg-muted rounded animate-pulse"></div>
						</div>
					))}
				</div>
				<div className="md:w-3/4">
					<div className="h-10 w-2/3 bg-muted rounded animate-pulse mb-8"></div>
					{[...Array(3)].map((_, i) => (
						<div
							key={i}
							className="bg-card rounded-xl border border-border shadow-lg p-6 mb-4"
						>
							<div className="flex items-start justify-between mb-4">
								<div className="flex items-center space-x-3">
									<div className="w-12 h-12 rounded-full bg-muted animate-pulse"></div>
									<div>
										<div className="h-5 w-32 bg-muted rounded animate-pulse mb-2"></div>
										<div className="h-4 w-24 bg-muted rounded animate-pulse"></div>
									</div>
								</div>
								<div className="h-8 w-16 bg-muted rounded-full animate-pulse"></div>
							</div>
							<div className="h-6 w-4/5 bg-muted rounded animate-pulse mb-3"></div>
							<div className="h-20 w-full bg-muted rounded animate-pulse mb-4"></div>
							<div className="flex flex-wrap gap-2 mb-4">
								{[...Array(4)].map((_, j) => (
									<div
										key={j}
										className="h-6 w-16 bg-muted rounded-full animate-pulse"
									></div>
								))}
							</div>
						</div>
					))}
				</div>
			</div>
		);
	}
	if (isPostsError || postsError) {
		return (
			<div className="container flex flex-col items-center mx-auto px-4 py-8">
				<div className="bg-card border border-destructive/20 rounded-xl p-8 shadow-lg max-w-md">
					<div className="flex flex-col items-center text-center mb-4">
						<div className="inline-flex items-center justify-center w-16 h-16 rounded-full bg-destructive/10 mb-4">
							<ExclamationCircleIcon className="h-8 w-8 text-destructive" />
						</div>
						<h2 className="text-xl font-bold text-card-foreground mb-2">
							Error Fetching Search Results
						</h2>
					</div>
					<p className="text-muted-foreground mb-6 text-center">
						We encountered a problem while retrieving job posts.
						This could be due to network issues or server problems.
					</p>
					<button
						onClick={() => window.location.reload()}
						className="w-full bg-destructive hover:bg-destructive/90 text-destructive-foreground font-medium py-2.5 px-4 rounded-full transition-all duration-200"
					>
						Try Again
					</button>
				</div>
			</div>
		);
	}
	if (!posts || !posts.entities || sortedPosts.length === 0) {
		console.log("Posts state", posts);
		return (
			<div className="container mx-auto px-4 py-8 flex flex-col md:flex-row gap-6">
				<div className="md:w-1/4">
					<SearchSidebar
						currentQuery={searchQuery}
						currentArea={area}
						currentMinBudget={minBudget}
						currentMaxBudget={maxBudget}
						currentIsRemote={isRemote}
					/>
				</div>
				<div className="md:w-3/4">
					<div className="bg-card rounded-xl border border-border shadow-lg p-8 text-center">
						<div className="inline-flex items-center justify-center w-16 h-16 rounded-full bg-muted/50 mb-4">
							<MagnifyingGlassIcon className="h-8 w-8 text-muted-foreground" />
						</div>
						<h2 className="text-xl font-bold text-card-foreground mb-2">
							No Results Found
						</h2>
						<p className="text-muted-foreground mb-2">
							No job posts match your search criteria:{" "}
							<span className="font-semibold text-card-foreground">
								{searchQuery || "All Jobs"}
							</span>
						</p>
						<p className="text-muted-foreground mb-6">
							Try adjusting your filters or using more general
							keywords.
						</p>
						<button
							onClick={() =>
								(window.location.href = `/search/jobs?query=${searchQuery}`)
							}
							className="bg-primary hover:bg-primary/90 text-primary-foreground font-medium py-2.5 px-6 rounded-full transition-all duration-200"
						>
							Clear All Filters
						</button>
					</div>
				</div>
			</div>
		);
	}
	return (
		<div className="container mx-auto px-4 py-8 flex flex-col md:flex-row gap-6">
			{/* Search parameters sidebar */}
			<div className="md:w-1/4">
				<SearchSidebar
					currentQuery={searchQuery}
					currentArea={area}
					currentMinBudget={minBudget}
					currentMaxBudget={maxBudget}
					currentIsRemote={isRemote}
				/>
			</div>

			{/* Job posts content */}
			<div className="md:w-3/4">
				<div className="mb-6 bg-card rounded-xl border border-border p-6 shadow-sm">
					<h1 className="text-2xl font-bold text-card-foreground mb-2">
						{searchQuery
							? `Search Results for: ${searchQuery}`
							: "All Job Posts"}
					</h1>
					<div className="flex items-center gap-2">
						<span className="inline-flex items-center px-3 py-1 rounded-full text-sm font-medium bg-primary/10 text-primary">
							{sortedPosts.length}{" "}
							{sortedPosts.length === 1 ? "result" : "results"}
						</span>
						{area && (
							<span className="text-sm text-muted-foreground">
								in{" "}
								<span className="font-medium text-card-foreground">
									{area}
								</span>
							</span>
						)}
						{isRemote && (
							<span className="inline-flex items-center px-2.5 py-1 rounded-full text-xs font-medium bg-green-500/10 text-green-600">
								Remote
							</span>
						)}
					</div>
				</div>{" "}
				<div className="space-y-4">
					{sortedPosts.map((post) => (
						<JobPostingCard key={post.id} job={post} />
					))}
				</div>
			</div>
		</div>
	);
};

export default SearchJobPosts;
