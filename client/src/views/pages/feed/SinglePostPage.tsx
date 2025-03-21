import { useAppSelector } from "@/app/hooks";
import { selectPostById, useGetPostQuery } from "@/features/posts/postsSlice";
import { useParams } from "react-router";


const SinglePostPage: React.FC = () => {
  const { postId } = useParams();

  // const post = useAppSelector((state) => selectPostById(state, postId!));
  const { data: post } = useGetPostQuery(postId!);

  if (!post) return null;

  return (
    <div className="bg-white shadow-md rounded-lg p-4 border border-gray-200">
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
          <h3 className="text-md font-semibold">{post.author.jobTitle}</h3>
          <p className="text-sm text-gray-500">{post.author.jobTitle || "No job title"}</p>
        </div>
      </div>

      {/* Post Title */}
      <h2 className="text-lg font-semibold mt-3">{post.title}</h2>

      {/* Post Content */}
      <p className="text-gray-700 mt-2">{post.content}</p>

      {/* Post Metadata */}
      <div className="flex justify-between items-center mt-4 text-sm text-gray-500">
        <span>{new Date(post.createdAt).toLocaleDateString()}</span>
      </div>

      {/* Tags */}
      {post.postTags.length > 0 && (
        <div className="flex space-x-2 mt-3">
          {post.postTags.map((tag: any, index: number) => (
            <span key={index} className="text-xs bg-gray-200 px-2 py-1 rounded-full">
              {tag.name}
            </span>
          ))}
        </div>
      )}
    </div>
  );
};

export default SinglePostPage;
