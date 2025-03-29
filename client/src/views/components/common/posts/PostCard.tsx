import { useAppSelector } from "@/app/hooks";
import { selectPostById } from "@/features/posts/postsSlice";
import { Link } from "react-router-dom";
import { Post } from "@/features/posts/types";

import ArrowUpwardIcon from '@mui/icons-material/ArrowUpward';

interface PostCardProps {
  post: Post;
}

const PostCard: React.FC<PostCardProps> = ({ post }) => {
  // const post = useAppSelector((state) => selectPostById(state, postId));

  if (!post) return null;

  const handleUpVote = () => {
    console.log("Up")
  };

  const handleDownVote = () => {
    console.log("Down")
  };

  const voteDifference = post.upvotes - post.downvotes;

  return (
    <div className="flex flex-col gap-3 bg-white shadow-md rounded-lg p-4 border border-gray-200">
      {/* Author Info */}
      <div className="flex items-center space-x-3">
        <img
          src={post.author.profilePictureUrl}
          // alt={`${post.author.id} Profile`}
          width={40}
          height={40}
          className="rounded-full"
        />
        <div>
          {/* Name */}
          <h3 className="text-md font-semibold">{post.author.firstName + ' ' + post.author.lastName}</h3>
          {/* Publish Date */}
          <div className="flex justify-between items-center text-sm text-gray-500">
            <span>{new Date(post.createdAt).toLocaleDateString()}</span>
          </div>
        </div>
      </div>

      {/* Post Title */}
      <Link to={`/feed/${post.id}`}>
        <h2 className="text-lg font-bold text-gray-700">{post.title}</h2>
      </Link>

      {/* Post Content */}
      <p className="text-gray-600">{post.content}</p>

      {/* Post Metadata */}

      {/* Tags */}
      {post.postTags?.length > 0 && (
        <div className="flex space-x-2">
          {post.postTags.map((tag: any, index: number) => (
            <span key={index} className="text-xs bg-gray-200 px-2 py-1 rounded-full">
              {tag.name}
            </span>
          ))}
        </div>
      )}

      {/* Interactions */}
      {/* Votes */}
      <div className="flex gap-2 items-stretch  bg-gray-200 rounded-full w-fit">
        <div className="rounded-l-full p-1 hover:bg-green-100  hover:cursor-pointer" onClick={handleUpVote}>
          <ArrowUpwardIcon className="text-gray-500 hover:text-green-400"/>
        </div>
        
        <div className={`flex justify-center items-center text-sm font-bold ${voteDifference >= 0 ? "text-green-600" : "text-red-600"}`}>
          {voteDifference}
        </div>

        <div className="rounded-l-full p-1 rotate-180 hover:bg-red-100  hover:cursor-pointer" onClick={handleDownVote}>

          <ArrowUpwardIcon className="text-gray-500 hover:text-red-400"/>
        </div>
      </div>
    </div>
  );
};

export default PostCard;



















// import { useAppSelector } from "@/app/hooks";
// import { selectPostById } from "@/features/posts/postsSlice";
// import { Link } from "react-router-dom";


// interface PostCardProps {
//   postId: string;
// }

// const PostCard: React.FC<PostCardProps> = ({ postId }) => {
//   const post = useAppSelector((state) => selectPostById(state, postId));

//   if (!post) return null;

//   return (
//     <div className="bg-white shadow-md rounded-lg p-4 border border-gray-200">
//       {/* Author Info */}
//       <div className="flex items-center space-x-3">
//         <img
//           src={post.author.profilePictureUrl}
//           // alt={`${post.author.id} Profile`}
//           width={40}
//           height={40}
//           className="rounded-full"
//         />
//         <div>
//           <h3 className="text-md font-semibold">{post.author.firstName + ' ' + post.author.lastName}</h3>
//           <p className="text-sm text-gray-500">{post.author.jobTitle || "No job title"}</p>
//         </div>
//       </div>

//       {/* Post Title */}
//       <Link to={`/feed/${post.id}`}>
//         <h2 className="text-lg font-semibold mt-3">{post.title}</h2>
//       </Link>

//       {/* Post Content */}
//       <p className="text-gray-700 mt-2">{post.content}</p>

//       {/* Post Metadata */}
//       <div className="flex justify-between items-center mt-4 text-sm text-gray-500">
//         <span>{new Date(post.createdAt).toLocaleDateString()}</span>
//       </div>

//       {/* Tags */}
//       {post.postTags?.length > 0 && (
//         <div className="flex space-x-2 mt-3">
//           {post.postTags.map((tag: any, index: number) => (
//             <span key={index} className="text-xs bg-gray-200 px-2 py-1 rounded-full">
//               {tag.name}
//             </span>
//           ))}
//         </div>
//       )}
//     </div>
//   );
// };

// export default PostCard;
