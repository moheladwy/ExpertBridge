import { useGetSuggestedExpertsQuery } from "@/features/profiles/profilesSlice";
import Avatar from "@mui/material/Avatar";
import { Skeleton } from "@mui/material";
import { ProfileResponse } from "@/features/profiles/types";
import StarIcon from "@mui/icons-material/Star";
import useRefetchOnLogin from "@/hooks/useRefetchOnLogin";
import { Trophy, Crown, Medal } from "lucide-react";
import { Link } from "react-router";

const TopReputationUsers = ({ limit = 5 }) => {
  const { data, isLoading, isError, error, refetch } =
    useGetSuggestedExpertsQuery(limit);

  useRefetchOnLogin(refetch);

  const getRankIcon = (index: number) => {
    switch (index) {
      case 0:
        return <Crown className="w-5 h-5 text-yellow-500" />;
      case 1:
        return <Medal className="w-5 h-5 text-gray-400" />;
      case 2:
        return <Medal className="w-5 h-5 text-amber-600" />;
      default:
        return <Trophy className="w-4 h-4 text-blue-500" />;
    }
  };

  const getRankBadge = (index: number) => {
    const badges = [
      "bg-gradient-to-r from-yellow-400 to-yellow-600 text-white",
      "bg-gradient-to-r from-gray-300 to-gray-500 text-white",
      "bg-gradient-to-r from-amber-400 to-amber-600 text-white",
      "bg-gradient-to-r from-blue-400 to-blue-600 text-white",
      "bg-gradient-to-r from-purple-400 to-purple-600 text-white",
    ];
    return (
      badges[index] || "bg-gradient-to-r from-gray-400 to-gray-600 text-white"
    );
  };

  if (isLoading) {
    return (
      <div className="bg-white dark:bg-gray-800 rounded-2xl shadow-lg border border-gray-100 dark:border-gray-700 overflow-hidden">
        <div className="p-6 border-b border-gray-100 dark:border-gray-700">
          <div className="flex items-center gap-3">
            <Skeleton variant="text" width={120} height={24} />
          </div>
        </div>
        <div className="p-6 space-y-4">
          {[...Array(limit)].map((_, i) => (
            <div key={i} className="flex items-center gap-3">
              <Skeleton variant="circular" width={48} height={48} />
              <div className="flex-1">
                <Skeleton variant="text" width="70%" height={20} />
                <Skeleton variant="text" width="50%" height={16} />
              </div>
              <Skeleton
                variant="rectangular"
                width={60}
                height={24}
                className="rounded-full"
              />
            </div>
          ))}
        </div>
      </div>
    );
  }

  if (isError) {
    return (
      <div className="bg-white dark:bg-gray-800 rounded-2xl shadow-lg border border-red-100 dark:border-red-900 p-6">
        <div className="text-red-500 text-center">
          <div className="text-red-400 mb-2">⚠️</div>
          <div className="text-sm">Unable to load top users</div>
        </div>
      </div>
    );
  }

  return (
    <div className="bg-white dark:bg-gray-800 rounded-2xl shadow-lg border border-gray-100 dark:border-gray-700 overflow-hidden transition-all duration-300 hover:shadow-xl">
      <div className="p-6 border-b text-center border-gray-100 dark:border-gray-700 bg-gradient-to-r from-yellow-50 to-amber-50 dark:from-yellow-900/20 dark:to-amber-900/20">
        <div className="flex items-center justify-center text-center">
          <div className="flex text-center justify-center items-center gap-3">
            <h3 className="font-semibold justify-center text-center text-gray-900 dark:text-white">
              Experts in Your Area of Interest
            </h3>
          </div>
        </div>
      </div>

      <div className="p-6">
        <div className="space-y-4">
          {data
            ?.filter((user: ProfileResponse) => (user.reputation || 0) >= 0)
            ?.sort(
              (a: ProfileResponse, b: ProfileResponse) =>
                (b.reputation || 0) - (a.reputation || 0),
            )
            ?.map((user: ProfileResponse, index) => (
              <Link to={`/profile/${user.id}`}>
                <div
                  key={user.id}
                  className="group flex items-center gap-4 p-3 rounded-xl hover:bg-gray-50 dark:hover:bg-gray-700/50 transition-all duration-200 cursor-pointer relative overflow-hidden"
                  style={{ animationDelay: `${index * 100}ms` }}
                >
                  {index < 3 && (
                    <div className="absolute inset-0 bg-gradient-to-r from-transparent via-yellow-50/30 to-transparent dark:via-yellow-900/10 opacity-0 group-hover:opacity-100 transition-opacity duration-300"></div>
                  )}

                  <div className="relative flex items-center gap-3">
                    <div className="relative">
                      <Avatar
                        src={user.profilePictureUrl}
                        className="w-12 h-12 ring-2 ring-white dark:ring-gray-700 shadow-md group-hover:ring-yellow-200 dark:group-hover:ring-yellow-700 transition-all duration-200"
                      />
                    </div>
                  </div>

                  <div className="flex-1 min-w-0 relative z-10">
                    <div className="flex items-center gap-2">
                      <h4 className="font-medium text-gray-900 dark:text-white truncate group-hover:text-yellow-600 dark:group-hover:text-yellow-400 transition-colors">
                        {user.firstName} {user.lastName}
                      </h4>
                    </div>
                    <p className="text-sm text-gray-500 dark:text-gray-400 truncate">
                      {user.jobTitle || "No title"}
                    </p>
                  </div>

                  <div className="relative z-10 flex items-center gap-2">
                    <div className="flex items-center gap-1 px-3 py-1 bg-gradient-to-r from-yellow-100 to-amber-100 dark:from-yellow-900/50 dark:to-amber-900/50 rounded-full">
                      <span className="text-sm font-semibold text-yellow-700 dark:text-yellow-300">
                        {user.reputation?.toLocaleString() || 0}
                      </span>
                    </div>
                  </div>
                </div>
              </Link>
            ))}
        </div>

        <div className="mt-6 pt-4 border-t border-gray-100 dark:border-gray-700"></div>
      </div>
    </div>
  );
};

export default TopReputationUsers;
