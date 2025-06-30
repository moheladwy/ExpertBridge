import React from 'react';
import { useGetSimilarJobsQuery } from '@/features/jobPostings/jobPostingsSlice';
import { Link } from 'react-router-dom';
import { CurrencyDollarIcon, MapPinIcon } from '@heroicons/react/24/outline';

interface SimilarJobsProps {
  currentJobId: string;
}

const SimilarJobs: React.FC<SimilarJobsProps> = ({ currentJobId }) => {
  const {
    data: similarJobs,
    error,
    isLoading,
  } = useGetSimilarJobsQuery(currentJobId, { skip: !currentJobId });

  const formatBudget = (budget: number) => {
    if (budget >= 1000) {
      return `$${(budget / 1000).toFixed(1)}k`;
    }
    return `$${budget}`;
  };

  if (isLoading) {
    return (
      <div className="bg-white dark:bg-gray-800 rounded-lg shadow-md p-4 border border-gray-200 dark:border-gray-700">
        <h3 className="text-lg font-semibold text-gray-900 dark:text-white mb-4">
          Similar Jobs
        </h3>
        <div className="space-y-3">
          {[...Array(3)].map((_, index) => (
            <div key={index} className="animate-pulse">
              <div className="h-4 bg-gray-200 dark:bg-gray-700 rounded mb-2"></div>
              <div className="h-3 bg-gray-200 dark:bg-gray-700 rounded w-3/4 mb-2"></div>
              <div className="h-3 bg-gray-200 dark:bg-gray-700 rounded w-1/2"></div>
            </div>
          ))}
        </div>
      </div>
    );
  }

  if (error || !similarJobs || similarJobs.length === 0) {
    return (
      <div className="bg-white dark:bg-gray-800 rounded-lg shadow-md p-4 border border-gray-200 dark:border-gray-700">
        <h3 className="text-lg font-semibold text-gray-900 dark:text-white mb-4">
          Similar Jobs
        </h3>
        <p className="text-gray-500 dark:text-gray-400 text-sm">
          No similar jobs found.
        </p>
      </div>
    );
  }

  return (
    <div className="bg-white dark:bg-gray-800 rounded-lg shadow-md p-4 border border-gray-200 dark:border-gray-700">
      <h3 className="text-lg font-semibold text-gray-900 dark:text-white mb-4">
        Similar Jobs
      </h3>
      <div className="space-y-4">
        {similarJobs.map((job) => (
          <Link
            key={job.jobPostingId}
            to={`/jobs/${job.jobPostingId}`}
            className="block p-3 rounded-lg border border-gray-100 dark:border-gray-700 hover:bg-gray-50 dark:hover:bg-gray-700 transition-colors duration-200"
          >
            <div className="space-y-2">
              {/* Job Title */}
              <h4 className="font-medium text-gray-900 dark:text-white text-sm line-clamp-2">
                {job.title}
              </h4>

              {/* Author */}
              <p className="text-xs text-gray-600 dark:text-gray-400">
                by {job.authorName}
              </p>

              {/* Content Preview */}
              <p className="text-xs text-gray-500 dark:text-gray-400 line-clamp-2">
                {job.content}
              </p>

              {/* Relevance Score */}
              <div className="flex items-center justify-between">
                <span className="text-xs text-blue-600 dark:text-blue-400">
                  {Math.round((1.0 - job.relevanceScore) * 100)}% match
                </span>
                {job.createdAt && (
                  <span className="text-xs text-gray-400">
                    {new Date(job.createdAt).toLocaleDateString()}
                  </span>
                )}
              </div>
            </div>
          </Link>
        ))}
      </div>
    </div>
  );
};

export default SimilarJobs;
