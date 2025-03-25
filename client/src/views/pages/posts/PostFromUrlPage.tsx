import { useGetPostQuery } from "@/features/posts/postsSlice";
import { Comment } from "@/features/comments/types";
import { useParams } from "react-router-dom";
import FullPostWithComments from "../../components/common/posts/FullPostWithComments";

const PostFromUrlPage: React.FC = () => {
  const { postId } = useParams();
  const { data: post, isLoading, error } = useGetPostQuery(postId!);

  if (isLoading) return <p>Loading...</p>;
  if (error || !post) return <p>Post not found.</p>;

  return (
    <FullPostWithComments post={post} />
  );
};

export default PostFromUrlPage;
