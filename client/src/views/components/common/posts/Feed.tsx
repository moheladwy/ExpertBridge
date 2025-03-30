import Filters from "./Filters";
import LoadingSkeleton from "./LoadingSkeleton";
import PostCard from "./PostCard";
import CreatePostModal from "./CreatePostModal";
import { Post } from "@/features/posts/types";

const Feed = () => {
  const dummyPosts: Post[] = [
    {
      id: "1",
      title: "How to optimize React performance?",
      content: "I'm having issues with my React app performance...",
      author: {
        id: "1",
        userId: "user1",
        firstName: "John",
        lastName: "Doe",
        username: "johndoe",
        jobTitle: "Software Engineer",
        profilePictureUrl: "https://example.com/avatar1.jpg"
      },
      createdAt: "2023-05-15T10:30:00Z",
      upvotes: 24,
      downvotes: 2,
      isUpvoted: false,
      isDownvoted: false,
      medias: [],
      comments: [],
      postTags: [
        { id: "1", name: "JavaScript" },
        { id: "3", name: "React" }
      ]
    },
    {
      id: "2",
      title: "TypeScript best practices in 2023",
      content: "What are the current best practices when using TypeScript with React? Looking for advice on typing patterns and project structure.",
      author: {
        id: "2",
        userId: "user2",
        firstName: "Jane",
        lastName: "Smith",
        username: "janesmith",
        jobTitle: "Frontend Developer",
        profilePictureUrl: "https://example.com/avatar2.jpg"
      },
      createdAt: "2023-05-16T14:45:00Z",
      upvotes: 1,
      downvotes: 18,
      isUpvoted: true,
      isDownvoted: false,
      medias: [{url: "https://www.alleycat.org/wp-content/uploads/2019/03/FELV-cat.jpg", type: "Picture"}],
      comments: [],
      postTags: [
        { id: "2", name: "TypeScript" }
      ]
    },
    {
      id: "3",
      title: "State management solutions comparison",
      content: "How does Zustand compare to Redux Toolkit in terms of performance and developer experience? Looking for real-world insights.",
      author: {
        id: "3",
        userId: "user3",
        firstName: "Alex",
        lastName: "Johnson",
        username: "alexj",
        jobTitle: "UI/UX Engineer",
        profilePictureUrl: "https://example.com/avatar3.jpg"
      },
      createdAt: "2023-05-17T09:15:00Z",
      upvotes: 32,
      downvotes: 3,
      isUpvoted: false,
      isDownvoted: false,
      medias: [{url: "https://commondatastorage.googleapis.com/gtv-videos-bucket/sample/BigBuckBunny.mp4", type: "Video"}],
      comments: [],
      postTags: [
        { id: "1", name: "JavaScript" },
        { id: "4", name: "State Management" }
      ]
    }
  ];

  return (
    <div className="flex flex-col w-2/5 mx-auto p-4 gap-5">
      <CreatePostModal />
      <div className="flex justify-center">
        <Filters />
      </div>
      <div className="space-y-4">
        {dummyPosts.map(post => (
          <PostCard key={post.id} post={post} />
        ))}
      </div>
    </div>
  );
};

export default Feed;
































// import Filters from "./Filters";
// import LoadingSkeleton from "./LoadingSkeleton";
// import PostCard from "./PostCard";
// import PostForm from "./AddPostForm";
// import useAuthSubscribtion from "@/lib/firebase/useAuthSubscribtion";
// import { auth } from "@/lib/firebase";
// import { randomInt } from "crypto";
// import { selectPostIds, useGetPostsQuery } from "@/features/posts/postsSlice";
// import { useAppSelector } from "@/app/hooks";
// import CreatePostModal from "./CreatePostModal";

// const Feed = () => {
//   // const [posts, setPosts] = useState<Post[]>([]);

//   const {
//     data: posts,
//     isLoading: postsLoading,
//     isSuccess: postsSuccess,
//     isError: postsError,
//     error: postsErrorMessage,
//   } = useGetPostsQuery();

//   const orderedPostIds = useAppSelector(selectPostIds);




//   // useEffect(() => {
//   //   // Simulating fetch
//   //   setTimeout(() => {
//   //     setPosts([
//   //       {
//   //         id: 1,
//   //         author: { email: "John Doe" },
//   //         title: "How to optimize React performance?",
//   //         content: "Some preview of the post content...",
//   //         upvotes: 120,
//   //         downvotes: 5,
//   //         tags: ["React", "Performance"],
//   //       },
//   //       {
//   //         id: 2,
//   //         author: { email: "Jane Smith" },
//   //         title: "Best practices for Tailwind CSS",
//   //         content: "Tailwind CSS allows for rapid UI development...",
//   //         upvotes: 95,
//   //         downvotes: 3,
//   //         tags: ["Tailwind", "CSS"],
//   //       },
//   //     ]);
//   //     setLoading(false);
//   //   }, 2000);
//   // }, []);


//   const loading = postsLoading;

//   return (
//     <div className="flex flex-col w-2/5 mx-auto p-4 gap-5">
//       {/* <PostForm userId={1} /> */}
//       <CreatePostModal />
//       <div className="flex justify-center">
//         <Filters />
//       </div>
//       {loading ? (
//         <LoadingSkeleton count={3} />
//       ) : (
//         <div className="space-y-4">
//           {orderedPostIds.map(postId => (
//             <PostCard key={postId} postId={postId} />
//           ))}
//         </div>
//       )}
//     </div>
//   );
// };

// export default Feed;
