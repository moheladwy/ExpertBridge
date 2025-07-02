import LoadingSkeleton from "./LoadingSkeleton";
import PostCard from "./PostCard";
import { useGetPostsCursorInfiniteQuery } from "@/features/posts/postsSlice";
import CreatePostModal from "./CreatePostModal";
import useRefetchOnLogin from "@/hooks/useRefetchOnLogin";
import React, { useEffect, useRef, useState } from "react";
import useIsUserLoggedIn from "@/hooks/useIsUserLoggedIn";
import { useCallbackOnIntersection } from "@/hooks/useCallbackOnIntersection";
import { PostsCursorPaginatedResponse } from "@/features/posts/types";
import { ToggleButtonGroup, ToggleButton } from "@mui/material";
import SuggestedJobs from "./SuggestedJobs";
import SuggestedExperts from "../profile/SuggestedExperts";
import TopReputationUsers from "../profile/TopReputationUsers";
import { TrendingUp, Clock, ThumbsUp, Sparkles } from "lucide-react";

const limit = 10;

const Feed = ({ startingPost = { id: null } }) => {
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
  } = useGetPostsCursorInfiniteQuery(undefined, {
    initialPageParam: {
      pageSize: limit,
      page: 1,
    },
  });

  const [filter, setFilter] = useState("Recommended");

  const handleChange = (
    event: React.MouseEvent<HTMLElement>,
    newFilter: string,
  ) => {
    if (newFilter) {
      setFilter(newFilter);
    }
  };

  const afterRef = useCallbackOnIntersection(fetchNextPage);
  const startingPostRef = useRef<HTMLDivElement>(null);
  const [hasCentered, setHasCentered] = useState<boolean>(false);

  useEffect(() => {
    if (hasCentered) return;
    const startingElement = startingPostRef.current;
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
      case "Most Upvoted":
        return <ThumbsUp className="w-4 h-4" />;
      case "Trending":
        return <TrendingUp className="w-4 h-4" />;
      default:
        return null;
    }
  };

  return (
    <div className="min-h-screen bg-gray-50 dark:bg-gray-900">
      <div className="flex gap-6 max-w-9xl mx-auto p-6">
        {/* Left Sidebar - Users */}
        <div className="w-90 max-xl:w-72 max-lg:hidden">
          <div className="space-y-6">
            <TopReputationUsers />
            <SuggestedExperts />
          </div>
        </div>

        {/* Main Feed Content */}
        <div className="flex-1 max-w-4xl mx-auto space-y-6">
          {/* Create Post Section */}
          <div className="bg-white dark:bg-gray-800 rounded-2xl shadow-lg border border-gray-100 dark:border-gray-700 overflow-hidden">
            <CreatePostModal />
          </div>

          {/* Filter Section */}
          <div className="bg-white dark:bg-gray-800 rounded-2xl shadow-lg border border-gray-100 dark:border-gray-700 p-6">
            {/* <div className="flex flex-col sm:flex-row items-center justify-between gap-4"> */}
            <div className="flex justify-between items-center gap-2 p-1 bg-gray-100 dark:bg-gray-700 rounded-xl">
              {["Recommended", "Recent", "Most Upvoted", "Trending"].map(
                (filterOption) => (
                  <button
                    key={filterOption}
                    onClick={() => setFilter(filterOption)}
                    className={`flex items-center gap-2 px-4 py-2 rounded-lg text-sm font-medium transition-all duration-200 ${
                      filter === filterOption
                        ? "text-blue-600 dark:text-blue-400"
                        : "hover:text-blue-600 dark:hover:text-blue-400 hover:bg-white/50 dark:hover:bg-gray-600/50"
                    }`}
                  >
                    {getFilterIcon(filterOption)}
                    <span className="hidden sm:inline">{filterOption}</span>
                  </button>
                ),
              )}
            </div>
            {/* </div> */}
          </div>

          {/* Posts Section */}
          {isLoading ? (
            <div className="space-y-6">
              <LoadingSkeleton count={7} />
            </div>
          ) : isError ? (
            <div className="bg-white dark:bg-gray-800 rounded-2xl shadow-lg border border-red-100 dark:border-red-900 p-8">
              <div className="text-center">
                <div className="text-red-400 text-4xl mb-4">⚠️</div>
                <div className="text-red-600 dark:text-red-400 font-medium">
                  Unable to load posts
                </div>
                <p className="text-gray-500 dark:text-gray-400 text-sm mt-2">
                  Please try refreshing the page
                </p>
              </div>
            </div>
          ) : (
            <>
              <div className="space-y-6">
                {data?.pages.map((page: PostsCursorPaginatedResponse) => {
                  const filteredPosts = [...page.posts];
                  if (filter === "Recent") {
                    filteredPosts.sort(
                      (a, b) =>
                        new Date(b.createdAt).getTime() -
                        new Date(a.createdAt).getTime(),
                    );
                  } else if (filter === "Most Upvoted") {
                    filteredPosts.sort(
                      (a, b) =>
                        b.upvotes - b.downvotes - (a.upvotes - a.downvotes),
                    );
                  } else if (filter === "Trending") {
                    filteredPosts.sort(
                      (a, b) =>
                        b.upvotes +
                        b.downvotes +
                        (b.comments || 0) -
                        (a.upvotes + a.downvotes + (a.comments || 0)),
                    );
                  }

                  return (
                    <React.Fragment key={page.pageInfo?.endCursor ?? "123"}>
                      {filteredPosts.map((post, index) => (
                        <div
                          key={post.id}
                          ref={
                            post.id === startingPost.id ? startingPostRef : null
                          }
                          className="animate-fade-in"
                          style={{ animationDelay: `${index * 100}ms` }}
                        >
                          <PostCard post={post} currUserId={appUser?.id} />
                        </div>
                      ))}
                    </React.Fragment>
                  );
                })}

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
                        ? "bg-gradient-to-r from-blue-600 to-indigo-600 hover:from-blue-700 hover:to-indigo-700 text-white shadow-lg hover:shadow-xl"
                        : "bg-gray-200 dark:bg-gray-700 text-gray-500 dark:text-gray-400 cursor-not-allowed"
                    }`}
                  >
                    {isFetchingNextPage
                      ? "Loading more posts..."
                      : hasNextPage
                        ? "Load More Posts"
                        : "🎉 You've reached the end! Great job staying connected."}
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

        {/* Right Sidebar - Jobs */}
        <div className="w-100 max-xl:w-72 max-lg:hidden">
          <div className="sticky top-24">
            <SuggestedJobs />
          </div>
        </div>
      </div>
    </div>
  );
};

export default Feed;
