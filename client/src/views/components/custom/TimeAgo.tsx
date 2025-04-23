import { parseISO, formatDistanceToNow, sub } from 'date-fns'

interface TimeAgoProps {
  timestamp: string;
}

export default ({ timestamp }: TimeAgoProps) => {
  let timeAgo = ''
  if (timestamp) {
    const date = parseISO(timestamp);
    if (sub(new Date(), { years: date.getFullYear() }).getFullYear() > 100) {
      timeAgo = 'A long time ago';
    } else {
      const timePeriod = formatDistanceToNow(date);
      timeAgo = `${timePeriod} ago`;
    }
  }

  return (
    <time dateTime={timestamp} title={timestamp}>
      &nbsp; <i>{timeAgo}</i>
    </time>
  );
};
