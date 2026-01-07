"use client";

import TimeAgo from "@/components/shared/TimeAgo";
import {
	Tooltip,
	TooltipContent,
	TooltipTrigger,
} from "@/components/ui/tooltip";

interface PostTimeStampProps {
	createdAt: string;
	lastModified?: string | null;
}

/**
 * Displays the post creation time with optional edited indicator.
 */
export default function PostTimeStamp({
	createdAt,
	lastModified,
}: PostTimeStampProps) {
	return (
		<div className="flex justify-between items-center text-sm text-muted-foreground">
			<span>
				<TimeAgo timestamp={createdAt} />
				{lastModified &&
					(() => {
						const date = new Date(lastModified);
						const isValidDate = !isNaN(date.getTime());

						if (!isValidDate) {
							return (
								<span className="text-xs italic ml-1">
									(edited)
								</span>
							);
						}

						return (
							<Tooltip>
								<TooltipTrigger
									className="text-xs italic ml-1 cursor-help"
									aria-label="Post was edited"
								>
									(edited)
								</TooltipTrigger>
								<TooltipContent>
									Last modified: {date.toLocaleString()}
								</TooltipContent>
							</Tooltip>
						);
					})()}
			</span>
		</div>
	);
}
