import { Post } from "./types";

interface PostCardProps {
  post: Post;
}

const PostCard = ({ post }: PostCardProps) => {
  return (
    <div className="bg-white shadow-md rounded-lg p-4 border border-gray-200">
      <div className="flex justify-between items-center">
        <h2 className="text-lg font-semibold">{post.title}</h2>
        <span className="text-sm text-gray-500">{post.author}</span>
      </div>
      <p className="text-gray-700 mt-2">{post.content}</p>
      <div className="flex justify-between items-center mt-4">
        <div className="flex space-x-2">
          {post.tags.map((tag, index) => (
            <span key={index} className="text-xs bg-gray-200 px-2 py-1 rounded-full">
              {tag}
            </span>
          ))}
        </div>
        <div className="flex space-x-4">
          <span className="text-green-600">⬆ {post.upvotes}</span>
          <span className="text-red-600">⬇ {post.downvotes}</span>
        </div>
      </div>
    </div>
  );
};

export default PostCard;