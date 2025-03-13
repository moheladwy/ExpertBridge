import { useEffect, useState } from "react";
import Filters from "./Filters";
import LoadingSkeleton from "./LoadingSkeleton";
import { Post } from "./types";
import PostCard from "./PostCard";
import PostForm from "./AddPostForm";
import { useGetCurrentUserQuery } from "../users/usersSlice";
import useAuthSubscribtion from "@/lib/firebase/useAuthSubscribtion";
import { auth } from "@/lib/firebase";

const Feed = () => {
  const [posts, setPosts] = useState<Post[]>([]);
  const [loading, setLoading] = useState(true);
  
  // This is only temporary
  const [authUser] = useAuthSubscribtion(auth);
  
  const {data: user} = useGetCurrentUserQuery(authUser?.email);

  useEffect(() => {
    // Simulating fetch
    setTimeout(() => {
      setPosts([
        {
          id: 1,
          author: { email: "John Doe" },
          title: "How to optimize React performance?",
          content: "Some preview of the post content...",
          upvotes: 120,
          downvotes: 5,
          tags: ["React", "Performance"],
        },
        {
          id: 2,
          author: { email: "Jane Smith" },
          title: "Best practices for Tailwind CSS",
          content: "Tailwind CSS allows for rapid UI development...",
          upvotes: 95,
          downvotes: 3,
          tags: ["Tailwind", "CSS"],
        },
      ]);
      setLoading(false);
    }, 2000);
  }, []);

  const handlePostSubmit = (newPost: { title: string; content: string; tag: string }) => {
    const postWithMeta: Post = {
      ...newPost,
      id: posts.length + 1,
      author: user!,
      upvotes: 0,
      downvotes: 0,
      tags: [newPost.tag],
    };
    setPosts([postWithMeta, ...posts]);
  };

  return (
    <div className="max-w-4xl mx-auto p-4">
      <PostForm onPostSubmit={handlePostSubmit} />
      <Filters />
      {loading ? (
        <LoadingSkeleton count={3} />
      ) : (
        <div className="space-y-4">
          {posts.map((post) => (
            <PostCard key={post.id} post={post} />
          ))}
        </div>
      )}
    </div>
  );
};

export default Feed;
