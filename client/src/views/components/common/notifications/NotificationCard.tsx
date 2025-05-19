import { NotificationResponse } from "@/features/notifications/types";
import { Link } from "react-router-dom";
import TimeAgo from "../../custom/TimeAgo";

export function NotificationCard({ notification }: { notification: NotificationResponse }) {
  return (
    <li
      className={
        `p-4 rounded-md shadow-sm border transition hover:bg-gray-50" ${notification.isRead ? "bg-white" : "bg-blue-50"}`
      }
    >
      <Link to={notification.actionUrl ?? '#'} className="flex justify-between">
        <div>
          <p className="text-sm text-gray-800">{notification.message}</p>
          <TimeAgo timestamp={notification.createdAt} />
        </div>
        {!notification.isRead && <span className="w-2 h-2 bg-blue-500 rounded-full self-center" />}
      </Link>
    </li >
  );
}
