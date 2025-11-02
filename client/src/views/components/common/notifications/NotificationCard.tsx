// Enhanced NotificationCard that accepts isNewNotification prop
import { NotificationResponse } from "@/features/notifications/types";
import { useNavigate } from "react-router";

interface NotificationCardProps {
	notification: NotificationResponse;
	isNewNotification?: boolean;
}

export function NotificationCard({
	notification,
	isNewNotification = false,
}: NotificationCardProps) {
	const navigate = useNavigate();
	return (
		<li
			className={`
      p-4 border rounded-lg transition-all duration-300
      ${
			isNewNotification
				? "bg-blue-50 dark:bg-blue-900/20 border-blue-200 dark:border-blue-800 shadow-md"
				: "bg-white dark:bg-gray-800 border-gray-200 dark:border-gray-700"
		}
    `}
		>
			<div className="flex items-start gap-3">
				{notification.iconUrl && (
					<img
						src={notification.iconUrl}
						alt="Notification icon"
						className="w-8 h-8 rounded-full flex-shrink-0"
					/>
				)}

				<div className="flex-1 min-w-0">
					<div className="flex items-center gap-2">
						<p className="text-sm text-gray-900 dark:text-gray-100">
							{notification.message}
						</p>
						{isNewNotification && (
							<span className="inline-flex items-center px-2 py-0.5 rounded-full text-xs font-medium bg-blue-100 dark:bg-blue-900/40 text-blue-800 dark:text-blue-300">
								New
							</span>
						)}
					</div>

					<p className="text-xs text-gray-500 dark:text-gray-400 mt-1">
						{new Date(notification.createdAt).toLocaleString()}
					</p>

					{notification.actionUrl && (
						<a
							onClick={() =>
								navigate(notification.actionUrl ?? "", {
									replace: false,
								})
							}
							className="text-blue-600 dark:text-blue-400 hover:text-blue-800 hover:cursor-pointer dark:hover:text-blue-300 text-sm font-medium mt-2 inline-block"
						>
							View Details →
						</a>
					)}
				</div>
			</div>
		</li>
	);
}
