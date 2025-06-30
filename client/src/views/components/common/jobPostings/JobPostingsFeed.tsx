import React, { useEffect, useRef, useState } from 'react';
import { ToggleButtonGroup, ToggleButton } from '@mui/material';
import LoadingSkeleton from '../posts/LoadingSkeleton';
import { useCallbackOnIntersection } from '@/hooks/useCallbackOnIntersection';
import useRefetchOnLogin from '@/hooks/useRefetchOnLogin';
import useIsUserLoggedIn from '@/hooks/useIsUserLoggedIn';
import { useGetJobsCursorInfiniteQuery } from '@/features/jobPostings/jobPostingsSlice';
import CreateJobModal from './CreateJobModal';
import { JobPosting, JobPostingPaginatedResponse } from '@/features/jobPostings/types';
import JobPostingCard from './JobPostingCard';

const limit = 10;

interface JobPostingsFeedProps {
  startingJob?: { id: string | null };
}

const JobPostingsFeed: React.FC<JobPostingsFeedProps> = ({
  startingJob = { id: null }
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
  } = useGetJobsCursorInfiniteQuery(
    undefined, // query param
    {
      initialPageParam: {
        pageSize: limit,
        page: 1,
      },
    },
  );

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

  const applyFilter = (page: JobPostingPaginatedResponse) => {
    const filteredJobs = [...page.jobPostings]; // Create a copy to avoid mutating original data

    if (filter === "Recent") {
      // Sort by creation date (newest first)
      filteredJobs.sort((a, b) => {
        const dateA = new Date(a.createdAt).getTime();
        const dateB = new Date(b.createdAt).getTime();
        return dateB - dateA;
      });
    } else if (filter === "Highest Budget") {
      // Sort by budget (highest first)
      filteredJobs.sort((a, b) => b.budget - a.budget);
    } else if (filter === "Most Engaged") {
      // Sort by engagement (upvotes + downvotes + comments)
      filteredJobs.sort((a, b) => {
        const aEngagement = a.upvotes + a.downvotes + (a.comments || 0);
        const bEngagement = b.upvotes + b.downvotes + (b.comments || 0);
        return bEngagement - aEngagement;
      });
    }
    // "Recommended" uses the default order from the API

    return filteredJobs;
  };

  if (!data?.pages.length || data?.pages.length < 1) {
    return null;
  }

  return (
    <div className="flex flex-col w-2/5 mx-auto p-4 gap-5 max-xl:w-3/5 max-lg:w-4/5 max-sm:w-full dark:bg-gray-900 dark:text-white">
      <CreateJobModal />

      <div className="flex justify-center">
        <ToggleButtonGroup
          color="primary"
          value={filter}
          exclusive
          onChange={handleFilterChange}
          aria-label="Job Filter"
          className="bg-white dark:bg-gray-800 rounded-lg"
        >
          <ToggleButton
            value="Recommended"
            className="dark:text-white hover:bg-gray-50 dark:hover:bg-gray-700"
          >
            Recommended
          </ToggleButton>
          <ToggleButton
            value="Recent"
            className="dark:text-white hover:bg-gray-50 dark:hover:bg-gray-700"
          >
            Recent
          </ToggleButton>
          <ToggleButton
            value="Highest Budget"
            className="dark:text-white hover:bg-gray-50 dark:hover:bg-gray-700"
          >
            Highest Budget
          </ToggleButton>
          <ToggleButton
            value="Most Engaged"
            className="dark:text-white hover:bg-gray-50 dark:hover:bg-gray-700"
          >
            Most Engaged
          </ToggleButton>
        </ToggleButtonGroup>
      </div>

      {isLoading ? (
        <LoadingSkeleton count={7} />
      ) : isError ? (
        <div className="flex justify-center text-red-500 bg-red-50 dark:bg-red-900/20 p-4 rounded-lg">
          <p>Error: {error?.message || 'Failed to load job postings'}</p>
        </div>
      ) : null}

      {isLoading ? (
        <LoadingSkeleton count={7} />
      ) : (
        <>
          <div className="space-y-6">
            {data?.pages.map((page: JobPostingPaginatedResponse, index) => {
              const filteredJobs = applyFilter(page);
              
              return (
                <React.Fragment key={page.pageInfo?.endCursor ?? `job-page-${index}`}>
                  {filteredJobs.map((job) => (
                    <div
                      key={job.id}
                      ref={job.id === startingJob.id ? startingJobRef : null}
                    >
                      <JobPostingCard job={job} currUserId={appUser?.id} />
                    </div>
                  ))}
                </React.Fragment>
              );
            })}

            <div ref={afterRef}>
              {isFetchingNextPage ? <LoadingSkeleton count={3} /> : null}
            </div>
          </div>

          {!isFetchingNextPage && (
            <div className="flex justify-center">
              <button
                onClick={() => fetchNextPage()}
                disabled={!hasNextPage || isFetchingNextPage}
                className={`px-6 py-3 rounded-lg font-medium transition-colors duration-200 ${hasNextPage && !isFetchingNextPage
                  ? 'bg-blue-600 hover:bg-blue-700 text-white'
                  : 'bg-gray-200 dark:bg-gray-700 text-gray-500 dark:text-gray-400 cursor-not-allowed'
                  }`}
              >
                {isFetchingNextPage
                  ? "Loading more jobs..."
                  : hasNextPage
                    ? "Load More Jobs"
                    : "You've reached the end of available jobs!"}
              </button>
            </div>
          )}

          {isFetching && !isFetchingNextPage ? (
            <LoadingSkeleton count={2} />
          ) : null}
        </>
      )}
    </div>
  );
};

export default JobPostingsFeed;
