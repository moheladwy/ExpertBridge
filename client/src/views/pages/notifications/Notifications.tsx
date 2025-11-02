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
			<h1 className="text-2xl text-center font-semibold mb-4">
				Notifications
			</h1>
			{isFetching ? (
				<div className="flex flex-col items-center justify-center py-12">
					<div className="animate-spin rounded-full h-12 w-12 border-b-2 border-indigo-600 dark:border-indigo-400"></div>
					<p className="mt-4 text-gray-600 dark:text-gray-300">
						Loading notifications...
					</p>
				</div>
			) : notifications.length === 0 ? (
				<p className="text-center">No notifications yet.</p>
			) : (
				<ul className="space-y-4">
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
