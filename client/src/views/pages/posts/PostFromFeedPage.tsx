import { useAppSelector } from "@/app/hooks";
import { selectPostById, useDeletePostMutation, useGetPostQuery } from "@/features/posts/postsSlice";
import { Comment } from "@/features/comments/types";
import { useNavigate, useParams } from "react-router-dom";
import { Navigate } from "react-router-dom";
import FullPostWithComments from "../../components/common/posts/FullPostWithComments";
import useIsUserLoggedIn from "@/hooks/useIsUserLoggedIn";
import { useEffect } from "react";
import toast from "react-hot-toast";


const PostFromFeedPage: React.FC = () => {
  const { postId } = useParams();

  const post = useAppSelector((state) => selectPostById(state, postId!));
  // const { data: post } = useGetPostQuery(postId!);

  const navigate = useNavigate();
  const [deletePost, deleteResult] = useDeletePostMutation();

  useEffect(() => {
    if (deleteResult.isSuccess) {
      toast.success("Your post was deleted successfully.");
      navigate('/home');
    }
    if (deleteResult.isError) {
      toast.error("An error occurred while deleting you post.");
      console.log(deleteResult.error);
    }
  }, [deleteResult.isSuccess, deleteResult.isError, deleteResult.error, navigate]);

  if (!post && !deleteResult.isSuccess) return <Navigate to={`/posts/${postId}`} />;

  return (
    <FullPostWithComments post={post} deletePost={deletePost} />
  );
};

export default PostFromFeedPage;
