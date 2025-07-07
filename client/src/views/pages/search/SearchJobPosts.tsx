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
    const sorted = data.sort((a, b) => b.createdAt.localeCompare(a.createdAt));
    setSortedPosts(sorted);
  }, [posts]);

  if (isPostsLoading || isPostsFetching) {
    return (
      <div className="container mx-auto px-4 py-8 flex flex-col md:flex-row gap-6">
        <div className="md:w-1/4 bg-white dark:bg-gray-800 rounded-lg shadow-md p-4 h-fit">
          <div className="h-8 w-3/4 bg-gray-200 dark:bg-gray-700 rounded animate-pulse mb-4"></div>
          {[...Array(5)].map((_, i) => (
            <div key={i} className="mb-6">
              <div className="h-5 w-full bg-gray-200 dark:bg-gray-700 rounded animate-pulse mb-2"></div>
              <div className="h-8 w-full bg-gray-200 dark:bg-gray-700 rounded animate-pulse"></div>
            </div>
          ))}
        </div>
        <div className="md:w-3/4">
          <div className="h-10 w-2/3 bg-gray-200 dark:bg-gray-700 rounded animate-pulse mb-8"></div>
          {[...Array(3)].map((_, i) => (
            <div
              key={i}
              className="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6 mb-4"
            >
              <div className="flex items-start justify-between mb-4">
                <div className="flex items-center space-x-3">
                  <div className="w-12 h-12 rounded-full bg-gray-200 dark:bg-gray-700 animate-pulse"></div>
                  <div>
                    <div className="h-5 w-32 bg-gray-200 dark:bg-gray-700 rounded animate-pulse mb-2"></div>
                    <div className="h-4 w-24 bg-gray-200 dark:bg-gray-700 rounded animate-pulse"></div>
                  </div>
                </div>
                <div className="h-8 w-16 bg-gray-200 dark:bg-gray-700 rounded-full animate-pulse"></div>
              </div>
              <div className="h-6 w-4/5 bg-gray-200 dark:bg-gray-700 rounded animate-pulse mb-3"></div>
              <div className="h-20 w-full bg-gray-200 dark:bg-gray-700 rounded animate-pulse mb-4"></div>
              <div className="flex flex-wrap gap-2 mb-4">
                {[...Array(4)].map((_, j) => (
                  <div
                    key={j}
                    className="h-6 w-16 bg-gray-200 dark:bg-gray-700 rounded-full animate-pulse"
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
        <div className="bg-red-50 dark:bg-red-900/20 border border-red-200 dark:border-red-800 rounded-lg p-6 mb-6">
          <div className="flex items-center text-center justify-center mb-3">
            <ExclamationCircleIcon className="h-6 w-6 text-red-500 dark:text-red-400 mr-2" />
            <h2 className="text-lg font-semibold text-red-700 dark:text-red-300">
              Error Fetching Search Results
            </h2>
          </div>
          <p className="text-red-600 dark:text-red-300 mb-4">
            We encountered a problem while retrieving job posts. This could be
            due to network issues or server problems.
          </p>
          <button
            onClick={() => window.location.reload()}
            className="flex justify-center items-center text-center bg-red-100 hover:bg-red-200 dark:bg-red-800 dark:hover:bg-red-700 text-red-700 dark:text-red-200 font-medium py-2 px-4 rounded-md transition duration-150"
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
          <div className="bg-white dark:bg-gray-800 rounded-lg shadow-md p-6 mb-6">
            <div className="flex items-center mb-3">
              <MagnifyingGlassIcon className="h-6 w-6 text-gray-500 dark:text-gray-400 mr-2" />
              <h2 className="text-lg font-semibold text-gray-700 dark:text-gray-300">
                No Results Found
              </h2>
            </div>
            <p className="text-gray-600 dark:text-gray-400 mb-4">
              No job posts match your search criteria:{" "}
              <span className="font-medium">{searchQuery || "All Jobs"}</span>
            </p>
            <p className="text-gray-600 dark:text-gray-400 mb-4">
              Try adjusting your filters or using more general keywords.
            </p>
            <button
              onClick={() =>
                (window.location.href = `/search/jobs?query=${searchQuery}`)
              }
              className="bg-blue-100 hover:bg-blue-200 dark:bg-blue-900 dark:hover:bg-blue-800 text-blue-700 dark:text-blue-200 font-medium py-2 px-4 rounded-md transition duration-150"
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
        <div className="mb-6">
          <h1 className="text-2xl font-bold text-gray-900 dark:text-white mb-2">
            {searchQuery
              ? `Search Results for: ${searchQuery}`
              : "All Job Posts"}
          </h1>
          <p className="text-gray-600 dark:text-gray-400">
            Found {sortedPosts.length} matching job posts
            {area && ` in ${area}`}
            {isRemote && " (Remote)"}
          </p>
        </div>

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
