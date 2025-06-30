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
  } = useGetPostsCursorInfiniteQuery(
    undefined, // query param
    {
      initialPageParam: {
        pageSize: limit,
        page: 1,
      },
    },
  );

  const [filter, setFilter] = useState("Recommended");

  const handleChange = (
    event: React.MouseEvent<HTMLElement>,
    newFilter: string,
  ) => {
    setFilter(newFilter);
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

  // const {
  //   data: posts,
  //   isFetching: postsLoading,
  //   isSuccess: postsSuccess,
  //   isError: postsError,
  //   error: postsErrorMessage,
  //   refetch
  // } = useGetPostsQuery();

  useEffect(() => {
    console.log("feed mounting...");
  }, []);

  // const orderedPostIds: string[] = useAppSelector(selectPostIds);
  const [, , , , appUser] = useIsUserLoggedIn();

  useRefetchOnLogin(refetch);

  return (
    <div className="flex flex-col w-2/5 mx-auto p-4 gap-5 max-xl:w-3/5 max-lg:w-4/5 max-sm:w-full dark:bg-gray-900 dark:text-white">
      <CreatePostModal />
      
      <div className="flex justify-center">
        <ToggleButtonGroup
          color="primary"
          value={filter}
          exclusive
          onChange={handleChange}
          aria-label="Platform"
        >
          <ToggleButton value="Recommended" className="dark:text-white">
            Recommended
          </ToggleButton>
          <ToggleButton value="Recent" className="dark:text-white">
            Recent
          </ToggleButton>
          <ToggleButton value="Most Upvoted" className="dark:text-white">
            Most Upvoted
          </ToggleButton>
          <ToggleButton value="Trending" className="dark:text-white">
            Trending
          </ToggleButton>
        </ToggleButtonGroup>
      </div>
      {isLoading ? (
        <LoadingSkeleton count={7} />
      ) : isError ? (
        <div className="flex justify-center text-red-500">
          Error: {error.message}
        </div>
      ) : null}

      {isLoading ? (
        <LoadingSkeleton count={7} />
      ) : (
        <>
          <div className="space-y-4">
            {data?.pages.map((page: PostsCursorPaginatedResponse) => {
              // Apply filters to posts
              const filteredPosts = [...page.posts]; // Create a copy to avoid mutating original data

              if (filter === "Recent") {
                // Sort by creation date (newest first)
                filteredPosts.sort((a, b) => {
                  const dateA = new Date(a.createdAt).getTime();
                  const dateB = new Date(b.createdAt).getTime();
                  return dateB - dateA;
                });
              } else if (filter === "Most Upvoted") {
                // Sort by net votes (upvotes - downvotes)
                filteredPosts.sort(
                  (a, b) => b.upvotes - b.downvotes - (a.upvotes - a.downvotes),
                );
              } else if (filter === "Trending") {
                // Sort by engagement (upvotes + downvotes + comments)
                filteredPosts.sort((a, b) => {
                  const aEngagement =
                    a.upvotes + a.downvotes + (a.comments || 0);
                  const bEngagement =
                    b.upvotes + b.downvotes + (b.comments || 0);
                  return bEngagement - aEngagement;
                });
              }
              // "Recommended for you" uses the default order from the API

              return (
                <React.Fragment key={page.pageInfo?.endCursor ?? "123"}>
                  {filteredPosts.map((post) => (
                    <div
                      key={post.id}
                      ref={post.id === startingPost.id ? startingPostRef : null}
                    >
                      <PostCard post={post} currUserId={appUser?.id} />
                    </div>
                  ))}
                </React.Fragment>
              );
            })}

            <div ref={afterRef}>
              {isFetchingNextPage ? <LoadingSkeleton count={7} /> : null}
            </div>
          </div>

          <LoadingSkeleton count={3} />

          <div>
            <button
              onClick={() => fetchNextPage()}
              disabled={!hasNextPage || isFetchingNextPage}
            >
              {isFetchingNextPage
                ? "Loading more..."
                : hasNextPage
                  ? "Load Newer"
                  : "You have reached the end of your Feed!"}
            </button>
          </div>
          <div>
            {isFetching && !isFetchingNextPage ? (
              <LoadingSkeleton count={3} />
            ) : null}
          </div>
        </>
      )}
    </div>
  );
};

export default Feed;
