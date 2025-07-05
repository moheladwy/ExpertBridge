import React, { useEffect, useRef, useState } from "react";
import { ToggleButtonGroup, ToggleButton } from "@mui/material";
import LoadingSkeleton from "../posts/LoadingSkeleton";
import { useCallbackOnIntersection } from "@/hooks/useCallbackOnIntersection";
import useRefetchOnLogin from "@/hooks/useRefetchOnLogin";
import useIsUserLoggedIn from "@/hooks/useIsUserLoggedIn";
import { useGetJobsCursorInfiniteQuery } from "@/features/jobPostings/jobPostingsSlice";
import CreateJobModal from "./CreateJobModal";
import {
  JobPosting,
  JobPostingPaginatedResponse,
} from "@/features/jobPostings/types";
import JobPostingCard from "./JobPostingCard";
import SuggestedQuestions from "./SuggestedQuestions";
import SuggestedExperts from "../profile/SuggestedExperts";
import TopReputationUsers from "../profile/TopReputationUsers";
import {
  TrendingUp,
  Clock,
  DollarSign,
  Sparkles,
  Heart,
  Briefcase,
} from "lucide-react";
import { StringifyOptions } from "querystring";

const limit = 10;

interface JobPostingsFeedProps {
  startingJob?: { id: string | null };
}

const JobPostingsFeed: React.FC<JobPostingsFeedProps> = ({
  startingJob = { id: null },
}) => {
  const {
    hasNextPage,
    data,
    error,
    isFetching,
    isLoading,
    isError,
    fetchNextPage,
    isFetchingNextPage,
    refetch,
  } = useGetJobsCursorInfiniteQuery(undefined, {
    initialPageParam: {
      pageSize: limit,
      page: 1,
    },
  });

  const [filter, setFilter] = useState("Recommended");

  const handleFilterChange = (
    event: React.MouseEvent<HTMLElement>,
    newFilter: string,
  ) => {
    if (newFilter !== null) {
      setFilter(newFilter);
    }
  };

  const afterRef = useCallbackOnIntersection(fetchNextPage);
  const startingJobRef = useRef<HTMLDivElement>(null);
  const [hasCentered, setHasCentered] = useState<boolean>(false);

  useEffect(() => {
    if (hasCentered) return;
    const startingElement = startingJobRef.current;
    if (startingElement) {
      startingElement.scrollIntoView({
        behavior: "auto",
        block: "center",
      });
      setHasCentered(true);
    }
  }, [data?.pages, hasCentered]);

  const [, , , , appUser] = useIsUserLoggedIn();
  useRefetchOnLogin(refetch);

  const getFilterIcon = (filterName: string) => {
    switch (filterName) {
      case "Recommended":
        return <Sparkles className="w-4 h-4" />;
      case "Recent":
        return <Clock className="w-4 h-4" />;
      case "Highest Budget":
        return <DollarSign className="w-4 h-4" />;
      case "Most Engaged":
        return <Heart className="w-4 h-4" />;
      default:
        return null;
    }
  };

  const applyFilter = (page: JobPostingPaginatedResponse) => {
    const filteredJobs = [...page.jobPostings];

    if (filter === "Recent") {
      filteredJobs.sort((a, b) => {
        const dateA = new Date(a.createdAt).getTime();
        const dateB = new Date(b.createdAt).getTime();
        return dateB - dateA;
      });
    } else if (filter === "Highest Budget") {
      filteredJobs.sort((a, b) => b.budget - a.budget);
    } else if (filter === "Most Engaged") {
      filteredJobs.sort((a, b) => {
        const aEngagement = a.upvotes + a.downvotes + (a.comments || 0);
        const bEngagement = b.upvotes + b.downvotes + (b.comments || 0);
        return bEngagement - aEngagement;
      });
    }

    return filteredJobs;
  };

  if (!data?.pages.length || data?.pages.length < 1) {
    return (
      <div className="min-h-screen bg-gray-50 dark:bg-gray-900">
        <div className="flex gap-4 max-w-9xl mx-2 p-4">
          {/* Left Sidebar */}
          <div className="w-100 max-xl:w-72 max-lg:hidden">
            <div className="space-y-6">
              <TopReputationUsers />
              <SuggestedExperts />
            </div>
          </div>

          {/* Main Content - Loading */}
          <div className="flex-1 max-w-4xl mx-auto space-y-6">
            <div className="bg-white dark:bg-gray-800 rounded-2xl shadow-lg border border-gray-100 dark:border-gray-700 p-8">
              <div className="text-center">
                <Briefcase className="w-12 h-12 text-gray-400 mx-auto mb-4" />
                <h3 className="text-lg font-semibold text-gray-900 dark:text-white mb-2">
                  Loading Job Opportunities
                </h3>
                <p className="text-gray-500 dark:text-gray-400">
                  Please wait while we fetch the latest job postings...
                </p>
              </div>
            </div>
          </div>

          {/* Right Sidebar */}
          <div className="w-80 max-xl:w-72 max-lg:hidden">
            <div className="sticky top-24">
              <SuggestedQuestions />
            </div>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50 dark:bg-gray-900">
      <div className="flex gap-6 max-w-9xl mx-auto p-6">
        {/* Left Sidebar - Users */}
        <div className="w-90 max-xl:w-72 max-lg:hidden">
          <div className="sticky top-24 space-y-6">
            <TopReputationUsers />
            <SuggestedExperts />
          </div>
        </div>

        {/* Main Jobs Feed Content */}
        <div className="flex-1 max-w-4xl mx-auto space-y-6">
          {/* Create Job Section */}
          <div className="bg-white dark:bg-gray-800 rounded-2xl shadow-lg border border-gray-100 dark:border-gray-700 overflow-hidden">
            <CreateJobModal />
          </div>

          {/* Filter Section */}
          <div className="bg-white dark:bg-gray-800 rounded-2xl shadow-lg border border-gray-100 dark:border-gray-700 p-4">
            <div className="flex flex-col justify-center items-center gap-3">
              <div className="flex flex-row text-center items-center justify-center sm:justify-start gap-3">
                <div className="p-2 bg-emerald-100 dark:bg-emerald-900/50 rounded-lg">
                  <Briefcase className="w-6 h-6 text-emerald-600 dark:text-emerald-400" />
                </div>
                <div className="flex flex-col justify-center text-center sm:text-left">
                  <h2 className="text-lg text-center font-semibold text-gray-900 dark:text-white">
                    Job Opportunities
                  </h2>
                  <p className="text-sm text-gray-500 dark:text-gray-400">
                    Discover your next career move
                  </p>
                </div>
              </div>

              <div className="flex flex-wrap items-center justify-center w-full gap-9 p-1 bg-gray-100 dark:bg-gray-700 rounded-xl">
                {[
                  "Recommended",
                  "Recent",
                  "Highest Budget",
                  "Most Engaged",
                ].map((filterOption) => (
                  <button
                    key={filterOption}
                    onClick={() => setFilter(filterOption)}
                    className={`flex items-center justify-center gap-2 px-4 py-2 rounded-lg text-sm font-medium transition-all duration-200 ${
                      filter === filterOption
                        ? "text-emerald-600 dark:text-emerald-400"
                        : "hover:text-emerald-600 dark:hover:text-emerald-400 hover:bg-white/50 dark:hover:bg-gray-600/50"
                    }`}
                  >
                    {getFilterIcon(filterOption)}
                    <span>{filterOption}</span>
                  </button>
                ))}
              </div>
            </div>
          </div>

          {/* Jobs Section */}
          {isLoading ? (
            <div className="space-y-6">
              <LoadingSkeleton count={7} />
            </div>
          ) : isError ? (
            <div className="bg-white dark:bg-gray-800 rounded-2xl shadow-lg border border-red-100 dark:border-red-900 p-8">
              <div className="text-center">
                <div className="text-red-400 text-4xl mb-4">⚠️</div>
                <div className="text-red-600 dark:text-red-400 font-medium">
                  Unable to load job postings
                </div>
                <p className="text-gray-500 dark:text-gray-400 text-sm mt-2">
                  {error?.message || "Failed to load job postings"}
                </p>
                <button
                  onClick={() => refetch()}
                  className="mt-4 px-6 py-2 bg-red-100 dark:bg-red-900/30 text-red-600 dark:text-red-400 rounded-lg hover:bg-red-200 dark:hover:bg-red-900/50 transition-colors"
                >
                  Try Again
                </button>
              </div>
            </div>
          ) : (
            <>
              <div className="space-y-6">
                {data?.pages.map(
                  (page: JobPostingPaginatedResponse, pageIndex) => {
                    const filteredJobs = applyFilter(page);

                    return (
                      <React.Fragment
                        key={
                          page.pageInfo?.endCursor ?? `job-page-${pageIndex}`
                        }
                      >
                        {filteredJobs.map((job, jobIndex) => (
                          <div
                            key={job.id}
                            ref={
                              job.id === startingJob.id ? startingJobRef : null
                            }
                            className="animate-fade-in"
                            style={{ animationDelay: `${jobIndex * 100}ms` }}
                          >
                            <JobPostingCard
                              job={job}
                              currUserId={appUser?.id}
                            />
                          </div>
                        ))}
                      </React.Fragment>
                    );
                  },
                )}

                <div ref={afterRef}>
                  {isFetchingNextPage && (
                    <div className="space-y-6">
                      <LoadingSkeleton count={3} />
                    </div>
                  )}
                </div>
              </div>

              {!isFetchingNextPage && (
                <div className="flex justify-center pt-8">
                  <button
                    onClick={() => fetchNextPage()}
                    disabled={!hasNextPage || isFetchingNextPage}
                    className={`px-8 py-4 rounded-2xl font-medium transition-all duration-300 transform hover:scale-105 ${
                      hasNextPage && !isFetchingNextPage
                        ? "bg-gradient-to-r from-emerald-600 to-teal-600 hover:from-emerald-700 hover:to-teal-700 text-white shadow-lg hover:shadow-xl"
                        : "bg-gray-200 dark:bg-gray-700 text-gray-500 dark:text-gray-400 cursor-not-allowed"
                    }`}
                  >
                    {isFetchingNextPage
                      ? "Loading more opportunities..."
                      : hasNextPage
                        ? "Discover More Jobs"
                        : "🎯 You've explored all available opportunities! Check back soon for new listings."}
                  </button>
                </div>
              )}

              {isFetching && !isFetchingNextPage && (
                <div className="space-y-6">
                  <LoadingSkeleton count={2} />
                </div>
              )}
            </>
          )}
        </div>

        {/* Right Sidebar - Suggested Questions */}
        <div className="w-80 max-xl:w-72 max-lg:hidden">
          <div className="sticky top-24">
            <SuggestedQuestions />
          </div>
        </div>
      </div>
    </div>
  );
};

export default JobPostingsFeed;
