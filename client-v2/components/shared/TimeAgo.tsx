"use client";

import { parseISO, formatDistanceToNow } from "date-fns";
import { useState } from "react";

interface TimeAgoProps {
	timestamp: string;
}

/**
 * Displays a human-readable time ago string with hover to show full date.
 * Uses date-fns for date formatting.
 */
export default function TimeAgo({ timestamp }: TimeAgoProps) {
	const [isHovered, setIsHovered] = useState(false);
	let content = "";
	let dateFormatted = "";

	if (timestamp) {
		const date = parseISO(timestamp);
		if (isNaN(date.getTime())) {
			content = "Invalid date";
			dateFormatted = "Invalid date";
		} else {
			dateFormatted = date.toLocaleString(undefined, {
				year: "numeric",
				month: "short",
				day: "numeric",
				hour: "2-digit",
				minute: "2-digit",
			});

			const now = new Date();
			const diffInYears =
				(now.getTime() - date.getTime()) /
				(1000 * 60 * 60 * 24 * 365.25);
			if (diffInYears > 100) {
				content = "A long time ago";
			} else {
				content = formatDistanceToNow(date) + " ago";
			}
		}
	}

	return (
		<time
			dateTime={timestamp}
			title={dateFormatted}
			onMouseEnter={() => setIsHovered(true)}
			onMouseLeave={() => setIsHovered(false)}
			className="transition-all duration-200 relative cursor-pointer text-muted-foreground hover:text-foreground"
		>
			<span className="inline-block after:content-[''] after:block after:h-px after:bg-muted-foreground/40 after:scale-x-0 hover:after:scale-x-100 after:transition-transform after:duration-200 after:origin-left">
				<i>{isHovered ? dateFormatted : content}</i>
			</span>
		</time>
	);
}
