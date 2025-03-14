import { useEffect, useState } from "react";
import Filters from "./Filters";
import LoadingSkeleton from "./LoadingSkeleton";
import { AddPostRequest, Post } from "./types";
import PostCard from "./PostCard";
import PostForm from "./AddPostForm";
import { useGetCurrentUserQuery } from "../users/usersSlice";
import useAuthSubscribtion from "@/lib/firebase/useAuthSubscribtion";
import { auth } from "@/lib/firebase";
import { randomInt } from "crypto";
import { selectPostIds, useGetPostsQuery } from "./postsSlice";
import { useAppSelector } from "@/app/hooks";

const Feed = () => {
  // const [posts, setPosts] = useState<Post[]>([]);

  // This is only temporary
  const [authUser] = useAuthSubscribtion(auth);
  
  const { data: user } = useGetCurrentUserQuery(authUser?.email);
  
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
    <div className="max-w-4xl mx-auto p-4">
      <PostForm userId={1} />
      <Filters />
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
