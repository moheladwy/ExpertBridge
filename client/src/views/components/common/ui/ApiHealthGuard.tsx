import React, { useState } from "react";
import { useApiHealth } from "@/hooks/useApiHealth";
import ApiHealthErrorPage from "@/views/pages/error/ApiHealthErrorPage";

interface ApiHealthGuardProps {
  children: React.ReactNode;
}

const ApiHealthGuard: React.FC<ApiHealthGuardProps> = ({ children }) => {
  const [isRetrying, setIsRetrying] = useState(false);
  const [showErrorPage, setShowErrorPage] = useState(false);

  const {
    isHealthy,
    isLoading,
    lastChecked,
    checkHealth,
    resetHealth,
    healthData,
  } = useApiHealth({
    checkOnStartup: true, // Only check on startup
    maxRetries: 5,
    enabled: true,
    onHealthChange: (healthy) => {
      if (!healthy) {
        setShowErrorPage(true);
      } else if (showErrorPage) {
        // API is healthy again, hide error page
        setShowErrorPage(false);
        setIsRetrying(false);
      }
    },
  });

  const handleRetry = async () => {
    setIsRetrying(true);

    try {
      const healthCheckResult = await checkHealth();

      if (healthCheckResult) {
        setShowErrorPage(false);
        resetHealth();
      }
    } catch (error) {
      console.error("Manual health check failed:", error);
    } finally {
      // Keep showing retry state for a moment to provide feedback
      setTimeout(() => {
        setIsRetrying(false);
      }, 1000);
    }
  };

  // Show error page when API is unhealthy
  if (showErrorPage && !isHealthy) {
    return (
      <ApiHealthErrorPage
        onRetry={handleRetry}
        isRetrying={isRetrying}
        healthData={healthData}
      />
    );
  }

  // Show loading state during initial health check
  if (isLoading && lastChecked === null) {
    return (
      <div className="min-h-screen bg-gray-100 dark:bg-gray-900 flex items-center justify-center">
        <div className="text-center">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-blue-600 mx-auto mb-4"></div>
          <p className="text-gray-600 dark:text-gray-400">
            Checking API connection...
          </p>
        </div>
      </div>
    );
  }

  // Render children when API is healthy or health check hasn't failed yet
  return <>{children}</>;
};

export default ApiHealthGuard;
