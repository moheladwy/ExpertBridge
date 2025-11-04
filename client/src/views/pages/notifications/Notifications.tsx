// Enhanced Notifications Component with highlight management
import {
	useGetNotificationsQuery,
	useReadNotificationsMutation,
} from "@/features/notifications/notificationsSlice";
import { NotificationResponse } from "@/features/notifications/types";
import { useGetCurrentUserProfileQuery } from "@/features/profiles/profilesSlice";
import { NotificationCard } from "@/views/components/common/notifications/NotificationCard";
import { useEffect, useState, useRef } from "react";

function Notifications() {
	const { data: userProfile } = useGetCurrentUserProfileQuery();
	const { data: notifications = [], isFetching } = useGetNotificationsQuery(
		userProfile!.id
	);
	const [readAll] = useReadNotificationsMutation();

	// Track which notifications were unread when component first loaded
	const [initialUnreadIds, setInitialUnreadIds] = useState<Set<string>>(
		new Set()
	);
	const hasInitialized = useRef(false);

	// Capture initial unread notifications
	useEffect(() => {
		if (
			!isFetching &&
			notifications.length > 0 &&
			!hasInitialized.current
		) {
			const unreadIds = new Set(
				notifications.filter((n) => !n.isRead).map((n) => n.id)
			);
			setInitialUnreadIds(unreadIds);
			hasInitialized.current = true;
		}
	}, [notifications, isFetching]);

	// Mark all as read after a delay to allow user to see new notifications
	useEffect(() => {
		if (!isFetching && initialUnreadIds.size > 0) {
			const timer = setTimeout(() => {
				readAll(userProfile!.id);
			}, 3000); // 3 second delay - adjust as needed

			return () => clearTimeout(timer);
		}
	}, [
		isFetching,
		initialUnreadIds.size,
		readAll,
		userProfile?.id,
		userProfile,
	]);

	return (
		<div className="max-w-3xl mx-auto px-4 py-6">
			<div className="mb-6">
				<h1 className="text-3xl font-bold text-card-foreground mb-2">
					Notifications
				</h1>
				<p className="text-muted-foreground">
					Stay updated with your latest activity
				</p>
			</div>
			{isFetching ? (
				<div className="flex flex-col items-center justify-center py-16 bg-card rounded-xl border border-border">
					<div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary"></div>
					<p className="mt-4 text-muted-foreground font-medium">
						Loading notifications...
					</p>
				</div>
			) : notifications.length === 0 ? (
				<div className="flex flex-col items-center justify-center py-16 bg-card rounded-xl border border-border">
					<div className="inline-flex items-center justify-center w-16 h-16 rounded-full bg-muted/50 mb-4">
						<svg
							className="w-8 h-8 text-muted-foreground"
							fill="none"
							stroke="currentColor"
							viewBox="0 0 24 24"
						>
							<path
								strokeLinecap="round"
								strokeLinejoin="round"
								strokeWidth={2}
								d="M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9"
							/>
						</svg>
					</div>
					<p className="text-center text-lg font-medium text-card-foreground mb-1">
						No notifications yet
					</p>
					<p className="text-center text-muted-foreground text-sm">
						We'll notify you when something new happens
					</p>
				</div>
			) : (
				<ul className="space-y-3">
					{[...notifications]
						.sort((a, b) => b.createdAt.localeCompare(a.createdAt))
						.map((notification: NotificationResponse) => (
							<NotificationCard
								key={notification.id}
								notification={notification}
								isNewNotification={initialUnreadIds.has(
									notification.id
								)}
							/>
						))}
				</ul>
			)}
		</div>
	);
}

export default Notifications;
