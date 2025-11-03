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
				? "bg-primary/10 border-primary/20 shadow-md"
				: "bg-card border-border"
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
						<p className="text-sm text-card-foreground">
							{notification.message}
						</p>
						{isNewNotification && (
							<span className="inline-flex items-center px-2 py-0.5 rounded-full text-xs font-medium bg-primary/10 text-primary">
								New
							</span>
						)}
					</div>

					<p className="text-xs text-muted-foreground mt-1">
						{new Date(notification.createdAt).toLocaleString()}
					</p>

					{notification.actionUrl && (
						<a
							onClick={() =>
								navigate(notification.actionUrl ?? "", {
									replace: false,
								})
							}
							className="text-primary hover:text-primary/80 hover:cursor-pointer text-sm font-medium mt-2 inline-block"
						>
							View Details →
						</a>
					)}
				</div>
			</div>
		</li>
	);
}
