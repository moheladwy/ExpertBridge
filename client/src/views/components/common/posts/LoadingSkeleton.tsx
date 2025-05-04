const LoadingSkeleton = ({ count }: { count: number }) => {
  return (
    <div className="space-y-4">
      {[...Array(count)].map((_, index) => (
        <div key={index} className="bg-gray-200 dark:bg-gray-700 animate-pulse h-24 rounded-md"></div>
      ))}
    </div>
  );
};


export default LoadingSkeleton;
