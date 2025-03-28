import Filters from "./Filters";
import LoadingSkeleton from "./LoadingSkeleton";
import PostCard from "./PostCard";
import PostForm from "./AddPostForm";
import useAuthSubscribtion from "@/lib/firebase/useAuthSubscribtion";
import { auth } from "@/lib/firebase";
import { randomInt } from "crypto";
import { selectPostIds, useGetPostsQuery } from "@/features/posts/postsSlice";
import { useAppSelector } from "@/app/hooks";
import CreatePostModal from "./CreatePostModal";

const Feed = () => {
  // const [posts, setPosts] = useState<Post[]>([]);

  const {
    data: posts,
    isLoading: postsLoading,
    isSuccess: postsSuccess,
    isError: postsError,
    error: postsErrorMessage,
  } = useGetPostsQuery();

  const orderedPostIds = useAppSelector(selectPostIds);

  // useEffect(() => {
  //   // Simulating fetch
  //   setTimeout(() => {
  //     setPosts([
  //       {
  //         id: 1,
  //         author: { email: "John Doe" },
  //         title: "How to optimize React performance?",
  //         content: "Some preview of the post content...",
  //         upvotes: 120,
  //         downvotes: 5,
  //         tags: ["React", "Performance"],
  //       },
  //       {
  //         id: 2,
  //         author: { email: "Jane Smith" },
  //         title: "Best practices for Tailwind CSS",
  //         content: "Tailwind CSS allows for rapid UI development...",
  //         upvotes: 95,
  //         downvotes: 3,
  //         tags: ["Tailwind", "CSS"],
  //       },
  //     ]);
  //     setLoading(false);
  //   }, 2000);
  // }, []);

  const loading = postsLoading;

  return (
    <div className="w-2/5 mx-auto p-4">
      {/* <PostForm userId={1} /> */}
      <CreatePostModal />
      <div className="flex justify-center">
        <Filters />
      </div>
      {loading ? (
        <LoadingSkeleton count={3} />
      ) : (
        <div className="space-y-4">
          {orderedPostIds.map(postId => (
            <PostCard key={postId} postId={postId} />
          ))}
        </div>
      )}
    </div>
  );
};

export default Feed;
