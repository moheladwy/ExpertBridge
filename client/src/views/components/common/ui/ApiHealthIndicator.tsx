import React from "react";
import { useApiHealth } from "@/hooks/useApiHealth";
import { Wifi, WifiOff, RefreshCw, AlertTriangle } from "lucide-react";
import { cn } from "@/lib/util/utils";

interface ApiHealthIndicatorProps {
  className?: string;
  showLabel?: boolean;
  size?: "sm" | "md" | "lg";
}

const ApiHealthIndicator: React.FC<ApiHealthIndicatorProps> = ({
  className,
  showLabel = false,
  size = "sm",
}) => {
  const { isHealthy, isLoading, error, lastChecked, retryCount } = useApiHealth(
    {
      checkOnStartup: true, // Only check on startup
      maxRetries: 3,
      enabled: true,
    },
  );

  const sizeClasses = {
    sm: "w-4 h-4",
    md: "w-5 h-5",
    lg: "w-6 h-6",
  };

  const getStatusIcon = () => {
    if (isLoading) {
      return (
        <RefreshCw
          className={cn(sizeClasses[size], "animate-spin text-blue-500")}
        />
      );
    }

    if (!isHealthy) {
      return retryCount > 0 ? (
        <AlertTriangle className={cn(sizeClasses[size], "text-yellow-500")} />
      ) : (
        <WifiOff className={cn(sizeClasses[size], "text-red-500")} />
      );
    }

    return <Wifi className={cn(sizeClasses[size], "text-green-500")} />;
  };

  const getStatusText = () => {
    if (isLoading) return "Checking API...";
    if (!isHealthy) {
      return retryCount > 0
        ? `API Issues (Retry ${retryCount})`
        : "API Unavailable";
    }
    return "API Online";
  };

  const getStatusColor = () => {
    if (isLoading) return "text-blue-600 dark:text-blue-400";
    if (!isHealthy) {
      return retryCount > 0
        ? "text-yellow-600 dark:text-yellow-400"
        : "text-red-600 dark:text-red-400";
    }
    return "text-green-600 dark:text-green-400";
  };

  const formatLastChecked = () => {
    if (!lastChecked) return "Never";
    const now = new Date();
    const diff = now.getTime() - lastChecked.getTime();
    const minutes = Math.floor(diff / 60000);

    if (minutes < 1) return "Just now";
    if (minutes === 1) return "1 minute ago";
    if (minutes < 60) return `${minutes} minutes ago`;

    const hours = Math.floor(minutes / 60);
    if (hours === 1) return "1 hour ago";
    return `${hours} hours ago`;
  };

  return (
    <div
      className={cn(
        "flex items-center gap-1.5 transition-all duration-200",
        className,
      )}
      title={`API Status: ${getStatusText()}${lastChecked ? ` • Last checked: ${formatLastChecked()}` : ""}${error ? ` • ${error}` : ""}`}
    >
      <div className="flex items-center justify-center">{getStatusIcon()}</div>

      {showLabel && (
        <span className={cn("text-xs font-medium truncate", getStatusColor())}>
          {getStatusText()}
        </span>
      )}

      {/* Optional pulse effect for loading state */}
      {isLoading && (
        <div className="absolute inset-0 rounded-full animate-ping opacity-20 bg-blue-500" />
      )}

      {/* Optional dot indicator for compact mode */}
      {!showLabel && (
        <div
          className={cn(
            "w-2 h-2 rounded-full transition-colors duration-200",
            isLoading
              ? "bg-blue-500 animate-pulse"
              : !isHealthy
                ? retryCount > 0
                  ? "bg-yellow-500"
                  : "bg-red-500"
                : "bg-green-500",
          )}
        />
      )}
    </div>
  );
};

export default ApiHealthIndicator;
