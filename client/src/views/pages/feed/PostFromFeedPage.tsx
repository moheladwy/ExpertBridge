import { useAppSelector } from "@/app/hooks";
import { selectPostById, useGetPostQuery } from "@/features/posts/postsSlice";
import { Comment } from "@/features/posts/types";
import { useParams } from "react-router-dom";
import { Navigate } from "react-router-dom";
import FullPostWithComments from "../../components/common/posts/FullPostWithComments";


const PostFromFeedPage: React.FC = () => {
  const { postId } = useParams();

  const post = useAppSelector((state) => selectPostById(state, postId!));
  // const { data: post } = useGetPostQuery(postId!);

  if (!post) return <Navigate to={`/posts/${postId}`} />;

  return (
    <FullPostWithComments post={post} />
  );
};

export default PostFromFeedPage;
