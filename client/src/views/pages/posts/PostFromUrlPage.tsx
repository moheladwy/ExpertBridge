import { useGetPostQuery } from "@/features/posts/postsSlice";
import { Comment } from "@/features/comments/types";
import { useParams } from "react-router-dom";
import FullPostWithComments from "../../components/common/posts/FullPostWithComments";
import useIsUserLoggedIn from "@/hooks/useIsUserLoggedIn";
import { useEffect } from "react";
import useRefetchOnLogin from "@/hooks/useRefetchOnLogin";

const PostFromUrlPage: React.FC = () => {

  const { postId } = useParams();
  const { data: post, isLoading, error, refetch } = useGetPostQuery(postId ?? '');

  useRefetchOnLogin(refetch);

  if (isLoading) return <p>Loading...</p>;
  if (error || !post) return <p>Post not found.</p>;

  return (
    <div>
      <FullPostWithComments post={post}/>
    </div>
  );
};

export default PostFromUrlPage;
