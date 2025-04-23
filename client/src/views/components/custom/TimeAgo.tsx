import { parseISO, formatDistanceToNow } from 'date-fns';
import { useState } from 'react';

interface TimeAgoProps {
  timestamp: string;
}

export default function TimeAgo({ timestamp }: TimeAgoProps) {
  const [isHovered, setIsHovered] = useState(false);
  let content = '';
  let dateFormatted = '';

  if (timestamp) {
    const date = parseISO(timestamp);
    dateFormatted = date.toLocaleString(undefined, {
      year: 'numeric',
      month: 'short',
      day: 'numeric',
      hour: '2-digit',
      minute: '2-digit',
    });

    const yearsDiff = new Date().getFullYear() - date.getFullYear();
    if (yearsDiff > 100) {
      content = 'A long time ago';
    } else {
      content = formatDistanceToNow(date) + ' ago';
    }
  }

  return (
    <time
      dateTime={timestamp}
      title={dateFormatted}
      onMouseEnter={() => setIsHovered(true)}
      onMouseLeave={() => setIsHovered(false)}
      className="transition-all duration-200 relative cursor-pointer text-gray-500 hover:text-black"
    >
      <span
        className="inline-block after:content-[''] after:block after:h-[1px] after:bg-gray-400 after:scale-x-0 hover:after:scale-x-100 after:transition-transform after:duration-200 after:origin-left"
      >
        <i>{isHovered ? dateFormatted : content}</i>
      </span>
    </time>
  );
}
