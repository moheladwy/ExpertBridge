import { useState } from "react";
import { AddPostRequest } from "./types";
import { useAddNewPostMutation } from "./postsSlice";

interface PostFormProps {
  onPostSubmit?: (post: AddPostRequest) => void;
  userId: number;
}

const PostForm: React.FC<PostFormProps> = ({ userId }) => {

  const [addNewPost, { isLoading }] = useAddNewPostMutation();


  const [title, setTitle] = useState<string>("");
  const [body, setBody] = useState<string>("");
  const [tag, setTag] = useState<string>("General");
  const [error, setError] = useState<string>("");



  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!title.trim() || !body.trim()) {
      setError("Title and content are required.");
      return;
    }

    await addNewPost({ title, body, userId, tags: [tag] })

    setTitle("");
    setBody("");
    setTag("General");
    setError("");
  };

  return (
    <div className="bg-white shadow-md rounded-lg p-4 border border-gray-200 mb-4">
      <h2 className="text-lg font-semibold mb-2">Create a Post</h2>

      {error && <p className="text-red-500 text-sm">{error}</p>}

      <form onSubmit={handleSubmit} className="space-y-4">
        <input
          type="text"
          placeholder="Title"
          value={title}
          onChange={(e) => setTitle(e.target.value)}
          className="w-full border px-3 py-2 rounded-md"
        />

        <textarea
          placeholder="Write your post..."
          value={body}
          onChange={(e) => setBody(e.target.value)}
          className="w-full border px-3 py-2 rounded-md"
          rows={4}
        ></textarea>

        <select
          value={tag}
          onChange={(e) => setTag(e.target.value)}
          className="w-full border px-3 py-2 rounded-md"
        >
          <option value="General">General</option>
          <option value="React">React</option>
          <option value="Tailwind">Tailwind</option>
          <option value="AI">AI</option>
        </select>

        <button
          type="submit"
          className="bg-blue-500 text-white px-4 py-2 rounded-md w-full"
          disabled={isLoading}
        >
          Post
        </button>
      </form>
    </div>
  );
};

export default PostForm;
