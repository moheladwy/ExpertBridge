interface LoadingSkeletonProps {
	count: number;
}

/**
 * Displays animated loading skeleton placeholders for posts.
 * @param count Number of skeleton items to display (max 100).
 */
export default function LoadingSkeleton({ count }: LoadingSkeletonProps) {
	const validCount = Math.max(0, Math.min(count, 100)); // Cap at reasonable maximum
	return (
		<div className="space-y-4">
			{[...Array(validCount)].map((_, index) => (
				<div
					key={index}
					className="bg-muted animate-pulse h-24 rounded-md"
				/>
			))}
		</div>
	);
}
