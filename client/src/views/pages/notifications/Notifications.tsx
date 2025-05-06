import { useGetNotificationsQuery, useReadNotificationsMutation } from "@/features/notifications/notificationsSlice";
import { useGetCurrentUserProfileQuery } from "@/features/profiles/profilesSlice";
import { NotificationCard } from "@/views/components/common/notifications/NotificationCard";
import { formatDistanceToNow } from "date-fns";
import { useEffect } from "react";

function Notifications() {
  const { data: userProfile } = useGetCurrentUserProfileQuery();
  const { data: notifications = [], isFetching } = useGetNotificationsQuery(userProfile!.id);
  const [readAll] = useReadNotificationsMutation();

  useEffect(() => {
    if (!isFetching) {
      readAll(userProfile!.id);
    }
  }, [isFetching]);

  // TODO: Figure out a way to keep the new notifications highlighted 
  // event after the readAll have been fired.

  return (
    <div className="max-w-3xl mx-auto px-4 py-6">
      <h1 className="text-2xl font-semibold mb-4">Notifications</h1>

      {isFetching ? (
        <p>Loading...</p>
      ) : notifications.length === 0 ? (
        <p>No notifications yet.</p>
      ) : (
        <ul className="space-y-4">
          {notifications.map((notification) => (
            <NotificationCard key={notification.id} notification={notification} />
          ))}
        </ul>
      )}
    </div>
  );
}

export default Notifications;

