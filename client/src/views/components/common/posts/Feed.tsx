import Filters from "./Filters";
import LoadingSkeleton from "./LoadingSkeleton";
import PostCard from "./PostCard";
import { useGetPostsQuery } from "@/features/posts/postsSlice";
import { useAppSelector } from "@/app/hooks";
import CreatePostModal from "./CreatePostModal";
import useRefetchOnLogin from "@/hooks/useRefetchOnLogin";
import { useEffect } from "react";
import useIsUserLoggedIn from "@/hooks/useIsUserLoggedIn";

const Feed = () => {
  const {
    data: posts,
    isFetching: postsLoading,
    isSuccess: postsSuccess,
    isError: postsError,
    error: postsErrorMessage,
    refetch
  } = useGetPostsQuery();

  useEffect(() => {
    console.log('feed mounting...');
  }, []);

  // const orderedPostIds: string[] = useAppSelector(selectPostIds);
  const [, , , , appUser] = useIsUserLoggedIn();

  useRefetchOnLogin(refetch);

  const loading = postsLoading;

  return (
    <div className="flex flex-col w-2/5 mx-auto p-4 gap-5 max-xl:w-3/5 max-lg:w-4/5 max-sm:w-full dark:bg-gray-900 dark:text-white">
      <CreatePostModal />
      <div className="flex justify-center">
        <Filters />
      </div>
      {loading ? (
        <LoadingSkeleton count={7} />
      ) : (
        <div className="space-y-4">
          {orderedPostIds.map(postId => (
            <PostCard key={postId} postId={postId} currUserId={appUser?.id} />
          ))}
        </div>
      )}
    </div>
  );
};

export default Feed;