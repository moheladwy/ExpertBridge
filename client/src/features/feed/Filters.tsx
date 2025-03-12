
const Filters = () => {
  return (
    <div className="mb-4 flex space-x-4">
      <button className="bg-blue-500 text-white px-4 py-2 rounded">Recent</button>
      <button className="bg-gray-200 px-4 py-2 rounded">Most Upvoted</button>
      <button className="bg-gray-200 px-4 py-2 rounded">Trending</button>
    </div>
  );
};

export default Filters;