import { useAppSelector } from "@/app/hooks";
import { selectPostById, useGetPostQuery } from "@/features/posts/postsSlice";
import { Comment } from "@/features/posts/types";
import { useParams } from "react-router-dom";
import { Navigate } from "react-router-dom";


const PostFromFeedPage: React.FC = () => {
  const { postId } = useParams();

  const post = useAppSelector((state) => selectPostById(state, postId!));
  // const { data: post } = useGetPostQuery(postId!);

  if (!post) return <Navigate to={`/posts/${postId}`} />;

  return (
    <div className="bg-white shadow-md rounded-lg p-4 border border-gray-200 max-w-3xl mx-auto">
      {/* Author Info */}
      <div className="flex items-center space-x-3">
        <img
          src={post.author.profilePictureUrl || "/assets/default-avatar.png"}
          alt="Author Profile"
          width={40}
          height={40}
          className="rounded-full"
        />
        <div>
          <h3 className="text-md font-semibold">{post.author.firstName + ' ' + post.author.lastName}</h3>
          <p className="text-sm text-gray-500">{post.author.jobTitle || "No job title"}</p>
        </div>
      </div>

      {/* Post Title & Content */}
      <h2 className="text-lg font-semibold mt-3">{post.title}</h2>
      <p className="text-gray-700 mt-2">{post.content}</p>

      {/* Post Metadata */}
      <div className="flex justify-between items-center mt-4 text-sm text-gray-500">
        <span>Posted on {new Date(post.createdAt).toLocaleDateString()}</span>
        <div className="flex space-x-4">
          <span className="text-green-600">⬆ {post.upvotes}</span>
          <span className="text-red-600">⬇ {post.downvotes}</span>
        </div>
      </div>

      {/* Tags */}
      {post.postTags && post.postTags.length > 0 && (
        <div className="flex space-x-2 mt-3">
          {post.postTags.map((tag: any) => (
            <span key={tag.id} className="text-xs bg-gray-200 px-2 py-1 rounded-full">
              {tag.name}
            </span>
          ))}
        </div>
      )}

      {/* Comments Section */}
      {post.comments.length > 0 && (
        <div className="mt-6">
          <h3 className="text-lg font-semibold mb-3">Comments</h3>
          {post.comments.map((comment: Comment) => (
            <div key={comment.id} className="p-3 border-t border-gray-300">
              <div className="flex items-center space-x-3">
                <img
                  src={comment.author.profilePictureUrl || "/default-avatar.png"}
                  alt="Comment Author"
                  width={30}
                  height={30}
                  className="rounded-full"
                />
                <div>
                  <h4 className="text-sm font-semibold">{comment.author.firstName + ' ' + comment.author.lastName}</h4>
                  <p className="text-xs text-gray-500">{comment.author.jobTitle || "No job title"}</p>
                </div>
              </div>
              <p className="text-gray-700 mt-2">{comment.content}</p>

              {/* Replies */}
              {comment.replies && (
                <div className="ml-6 mt-3 border-l-2 border-gray-300 pl-3">
                  <h4 className="text-sm font-semibold">Replies</h4>
                  {comment.replies.map((reply: Comment) => (
                    <div key={reply.id} className="mt-2">
                      <div className="flex items-center space-x-3">
                        <img
                          src={reply.author.profilePictureUrl || "/default-avatar.png"}
                          alt="Reply Author"
                          width={25}
                          height={25}
                          className="rounded-full"
                        />
                        <div>
                          <h5 className="text-xs font-semibold">{reply.author.firstName + ' ' + reply.author.lastName}</h5>
                          <p className="text-xs text-gray-500">{reply.author.jobTitle || "No job title"}</p>
                        </div>
                      </div>
                      <p className="text-gray-700 mt-1">{reply.content}</p>
                    </div>
                  ))}
                </div>
              )}
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

export default PostFromFeedPage;
