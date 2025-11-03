import TimeAgo from "@/views/components/custom/TimeAgo";

function PostTimeStamp({
	createdAt,
	lastModified,
}: {
	createdAt: string;
	lastModified?: string | null | undefined;
}) {
	return (
		<div className="flex justify-between items-center text-sm text-muted-foreground">
			<span>
				<TimeAgo timestamp={createdAt} />
				{lastModified && (
					<span
						className="text-xs italic ml-1"
						title={`Last modified: ${new Date(lastModified).toLocaleString()}`}
					>
						(edited)
					</span>
				)}
			</span>
		</div>
	);
}

export default PostTimeStamp;
