import { useState } from "react";

interface PostFormProps {
  onPostSubmit: (post: { title: string; content: string; tag: string }) => void;
}

const PostForm: React.FC<PostFormProps> = ({ onPostSubmit }) => {
  const [title, setTitle] = useState<string>("");
  const [content, setContent] = useState<string>("");
  const [tag, setTag] = useState<string>("General");
  const [error, setError] = useState<string>("");

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    if (!title.trim() || !content.trim()) {
      setError("Title and content are required.");
      return;
    }

    onPostSubmit({ title, content, tag });
    setTitle("");
    setContent("");
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
          value={content}
          onChange={(e) => setContent(e.target.value)}
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
        >
          Post
        </button>
      </form>
    </div>
  );
};

export default PostForm;
