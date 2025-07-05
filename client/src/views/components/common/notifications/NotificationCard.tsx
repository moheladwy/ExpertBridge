// Enhanced NotificationCard that accepts isNewNotification prop
import { NotificationResponse } from "@/features/notifications/types";

interface NotificationCardProps {
  notification: NotificationResponse;
  isNewNotification?: boolean;
}

export function NotificationCard({ notification, isNewNotification = false }: NotificationCardProps) {
  return (
    <li className={`
      p-4 border rounded-lg transition-all duration-300
      ${isNewNotification 
        ? 'bg-blue-50 border-blue-200 shadow-md' 
        : 'bg-white border-gray-200'
      }
    `}>
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
            <p className="text-sm text-gray-900">{notification.message}</p>
            {isNewNotification && (
              <span className="inline-flex items-center px-2 py-0.5 rounded-full text-xs font-medium bg-blue-100 text-blue-800">
                New
              </span>
            )}
          </div>
          
          <p className="text-xs text-gray-500 mt-1">
            {new Date(notification.createdAt).toLocaleString()}
          </p>
          
          {notification.actionUrl && (
            <a 
              href={notification.actionUrl}
              className="text-blue-600 hover:text-blue-800 text-sm font-medium mt-2 inline-block"
            >
              View Details →
            </a>
          )}
        </div>
      </div>
    </li>
  );
}
