import { useDeletePostMutation, useGetPostQuery } from "@/features/posts/postsSlice";
import { Comment } from "@/features/comments/types";
import { useNavigate, useParams } from "react-router-dom";
import FullPostWithComments from "../../components/common/posts/FullPostWithComments";
import useIsUserLoggedIn from "@/hooks/useIsUserLoggedIn";
import { useEffect } from "react";
import useRefetchOnLogin from "@/hooks/useRefetchOnLogin";
import toast from "react-hot-toast";
import { Skeleton } from "@/views/components/ui/skeleton";

const PostFromUrlPage: React.FC = () => {

  const { postId } = useParams();
  const { data: post, isFetching, error, refetch } = useGetPostQuery(postId ?? '');

  useRefetchOnLogin(refetch);


  const navigate = useNavigate();
  const [deletePost, deleteResult] = useDeletePostMutation();

  useEffect(() => {
    console.log('use effecting!!!.......................');
    console.log(deleteResult.isSuccess);
    if (deleteResult.isSuccess) {
      toast.success("Your post was deleted successfully.");
      navigate('/home');
    }
    if (deleteResult.isError) {
      toast.error("An error occurred while deleting you post.");
      console.log(deleteResult.error);
    }
  }, [deleteResult.isSuccess, deleteResult.isError, deleteResult.error, navigate]);

  if (isFetching) {
    return (
      <div className="w-full flex justify-center">
        <div className="w-2/5 mx-auto p-4 gap-5 max-xl:w-3/5 max-lg:w-4/5 max-sm:w-full">
          <div className="flex flex-col gap-3 bg-white shadow-md rounded-lg p-4 border border-gray-200">
            {/* Post Header Skeleton */}
            <div className="flex items-center justify-between pb-3 border-b border-gray-300">
              <Skeleton className="h-8 w-8 rounded-full" />
              <Skeleton className="h-8 w-8 rounded-full" />
            </div>

            {/* Author Info Skeleton */}
            <div className="flex items-center space-x-3">
              <Skeleton className="h-10 w-10 rounded-full" />
              <div className="space-y-2">
                <Skeleton className="h-4 w-32" />
                <Skeleton className="h-3 w-20" />
              </div>
            </div>

            {/* Post Content Skeleton */}
            <Skeleton className="h-6 w-3/4 mt-2" />
            <div className="space-y-2">
              <Skeleton className="h-4 w-full" />
              <Skeleton className="h-4 w-full" />
              <Skeleton className="h-4 w-4/5" />
            </div>

            {/* Media Skeleton */}
            <Skeleton className="h-72 w-full rounded-md mt-2" />

            {/* Post Actions Skeleton */}
            <div className="flex justify-between mt-2">
              <Skeleton className="h-8 w-24" />
              <Skeleton className="h-8 w-24" />
            </div>

            {/* Comments Section Skeleton */}
            <div className="mt-6 space-y-4">
              <Skeleton className="h-5 w-32" />
              <div className="flex gap-3">
                <Skeleton className="h-10 w-10 rounded-full" />
                <Skeleton className="h-20 w-full rounded-md" />
              </div>
            </div>
          </div>
        </div>
      </div>
    );
  }

  if (error || !post) {
    return (
      <div className="w-full flex justify-center mt-10">
        <div className="w-2/5 mx-auto p-4 max-xl:w-3/5 max-lg:w-4/5 max-sm:w-full">
          <div className="bg-white shadow-md rounded-lg p-8 border border-gray-200 text-center">
            <h2 className="text-2xl font-bold text-gray-700 mb-3">Post Not Found</h2>
            <p className="text-gray-600 mb-6">The post you're looking for might have been removed or is temporarily unavailable.</p>
            <button 
              onClick={() => navigate('/home')}
              className="px-4 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 transition duration-200"
            >
              Return to Home
            </button>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div>
      <FullPostWithComments post={post} deletePost={deletePost} />
    </div>
  );
};

export default PostFromUrlPage;
