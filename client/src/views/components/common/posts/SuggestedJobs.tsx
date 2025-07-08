import { useGetSuggestedJobsQuery } from "@/features/jobPostings/jobPostingsSlice";
import useIsUserLoggedIn from "@/hooks/useIsUserLoggedIn";
import { Link } from "react-router";
import LoadingSkeleton from "./LoadingSkeleton";
import useRefetchOnLogin from "@/hooks/useRefetchOnLogin";

// Suggested Jobs Component
const SuggestedJobs = () => {
  const [isLoggedIn] = useIsUserLoggedIn();
  const { data: jobs, isLoading, refetch } = useGetSuggestedJobsQuery(5);

  useRefetchOnLogin(refetch);

  if (isLoading) {
    return (
      <div className="bg-white dark:bg-gray-800 rounded-lg shadow-md p-4 sticky top-4">
        <h3 className="text-lg font-semibold mb-4 text-gray-900 dark:text-white">
          {isLoggedIn ? "See Jobs That Match Your Profile" : "Suggested Jobs"}
        </h3>
        <LoadingSkeleton count={3} />
      </div>
    );
  }

  const jobPostings = jobs || [];

  return (
    <div className="bg-white dark:bg-gray-800 rounded-lg shadow-md p-4 sticky top-4">
      <h3 className="text-lg text-center font-semibold mb-4 text-gray-900 dark:text-white">
        {isLoggedIn ? "See Jobs That Match Your Profile" : "Suggested Jobs"}
      </h3>
      <div className="space-y-3">
        {jobPostings.map((job) => (
          <div
            key={job.jobPostingId}
            className="p-3 border border-gray-200 dark:border-gray-700 rounded-lg hover:bg-gray-50 dark:hover:bg-gray-700 cursor-pointer transition-colors"
          >
            <Link to={`/job/${job.jobPostingId}`}>
              <h4 className="font-medium text-sm text-gray-900 dark:text-white mb-2 line-clamp-2">
                {job.title}
              </h4>
              <p className="text-xs text-gray-600 dark:text-gray-400 mb-1">
                By {job.authorName}
              </p>
              <p className="text-xs text-gray-500 dark:text-gray-500 mb-2">
                {job.area} • ${job.budget}
              </p>
              {/* <div className="flex items-center text-xs text-gray-500 dark:text-gray-400 space-x-3">
                <span>{job.upvotes - job.downvotes} votes</span>
                <span>{job.comments} comments</span>
              </div> */}
              {/* {job.postTags && job.postTags.length > 0 && (
                <div className="flex flex-wrap gap-1 mt-2">
                  {job.postTags.slice(0, 2).map((tag, index) => (
                    <span
                      key={index}
                      className="px-2 py-1 text-xs bg-green-100 dark:bg-green-900 text-green-800 dark:text-green-200 rounded"
                    >
                      {tag.name}
                    </span>
                  ))}
                </div>
              )} */}
            </Link>
          </div>
        ))}
      </div>
      <Link
        to="/jobs"
        className="block w-full mt-4 text-sm text-blue-600 dark:text-blue-400 hover:underline text-center"
      >
        View all jobs →
      </Link>
    </div>
  );
};

export default SuggestedJobs;
