import { useDeletePostMutation, useGetPostQuery } from "@/features/posts/postsSlice";
import { Comment } from "@/features/comments/types";
import { useNavigate, useParams } from "react-router-dom";
import FullPostWithComments from "../../components/common/posts/FullPostWithComments";
import useIsUserLoggedIn from "@/hooks/useIsUserLoggedIn";
import { useEffect } from "react";
import useRefetchOnLogin from "@/hooks/useRefetchOnLogin";
import toast from "react-hot-toast";

const PostFromUrlPage: React.FC = () => {

  const { postId } = useParams();
  const { data: post, isLoading, error, refetch } = useGetPostQuery(postId ?? '');

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

  if (isLoading) return <p>Loading...</p>;
  if (error || !post) return <p>Post not found.</p>;

  return (
    <div>
      <FullPostWithComments post={post} deletePost={deletePost} />
    </div>
  );
};

export default PostFromUrlPage;
