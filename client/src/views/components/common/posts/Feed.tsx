import Filters from "./Filters";
import LoadingSkeleton from "./LoadingSkeleton";
import PostCard from "./PostCard";
import { useGetPostsCursorInfiniteQuery, useGetPostsQuery } from "@/features/posts/postsSlice";
import { useAppSelector } from "@/app/hooks";
import CreatePostModal from "./CreatePostModal";
import useRefetchOnLogin from "@/hooks/useRefetchOnLogin";
import React, { useEffect, useRef, useState } from "react";
import useIsUserLoggedIn from "@/hooks/useIsUserLoggedIn";
import { useCallbackOnIntersection } from "@/hooks/useCallbackOnIntersection";
import { PostsCursorPaginatedResponse } from "@/features/posts/types";

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
  } =
    useGetPostsCursorInfiniteQuery(
      undefined, // query param
      {
        initialPageParam: {
          pageSize: limit,
        },
      },
    );

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
    console.log('feed mounting...');
  }, []);

  // const orderedPostIds: string[] = useAppSelector(selectPostIds);
  const [, , , , appUser] = useIsUserLoggedIn();

  useRefetchOnLogin(refetch);

  return (
    <div className="flex flex-col w-2/5 mx-auto p-4 gap-5 max-xl:w-3/5 max-lg:w-4/5 max-sm:w-full dark:bg-gray-900 dark:text-white">
      <CreatePostModal />
      <div className="flex justify-center">
        <Filters />
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
            {
              data?.pages.map((page: PostsCursorPaginatedResponse) => (
                <React.Fragment key={page.pageInfo?.endCursor ?? '123'}>
                  {
                    page.posts.map((post, index, arr) => (
                      <div
                        key={post.id}
                        ref={
                          post.id === startingPost.id
                            ? startingPostRef
                            : null
                        }
                      >
                        <PostCard
                          post={post}
                          currUserId={appUser?.id}
                        />
                      </div>
                    ))
                  }
                </React.Fragment>
              ))
            }

            <div ref={afterRef} >
              {
                isFetchingNextPage
                  ? <LoadingSkeleton count={7} />
                  : null
              }
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
            {
              isFetching && !isFetchingNextPage
                ? <LoadingSkeleton count={3} />
                : null
            }
          </div>
        </>
      )
      }
    </div >
  );
};

export default Feed;