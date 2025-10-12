import { useState, useEffect, useCallback, useRef } from "react";
import { HEALTH_CHECK_URL } from "@/lib/api/endpoints";

export interface HealthCheckEntry {
  data: Record<string, unknown>;
  duration: string;
  status: string;
  tags: string[];
}

export interface HealthCheckResponse {
  status: string;
  totalDuration: string;
  entries: Record<string, HealthCheckEntry>;
}

export interface ApiHealthStatus {
  isHealthy: boolean;
  isLoading: boolean;
  error: string | null;
  lastChecked: Date | null;
  retryCount: number;
  healthData: HealthCheckResponse | null;
}

interface UseApiHealthOptions {
  checkOnStartup?: boolean; // Only check on startup
  maxRetries?: number;
  enabled?: boolean;
  onHealthChange?: (isHealthy: boolean) => void;
}

export const useApiHealth = (options: UseApiHealthOptions = {}) => {
  const {
    checkOnStartup = true,
    maxRetries = 3,
    enabled = true,
    onHealthChange,
  } = options;

  const [status, setStatus] = useState<ApiHealthStatus>({
    isHealthy: true,
    isLoading: false,
    error: null,
    lastChecked: null,
    retryCount: 0,
    healthData: null,
  });

  const isCheckingRef = useRef(false);
  const hasCheckedRef = useRef(false);

  const checkHealth = useCallback(async (): Promise<boolean> => {
    if (isCheckingRef.current) return status.isHealthy;

    isCheckingRef.current = true;

    setStatus((prev) => ({
      ...prev,
      isLoading: true,
      error: null,
    }));

    try {
      const controller = new AbortController();
      const timeoutId = setTimeout(() => controller.abort(), 10000); // 10 second timeout

      const response = await fetch(HEALTH_CHECK_URL, {
        method: "GET",
        signal: controller.signal,
        headers: {
          "Content-Type": "application/json",
        },
      });

      clearTimeout(timeoutId);

      if (response.ok) {
        // Parse the health check response
        const healthData: HealthCheckResponse = await response.json();
        const isHealthy = healthData.status === "Healthy";
        const now = new Date();

        setStatus((prev) => ({
          isHealthy,
          isLoading: false,
          error: isHealthy ? null : `API status is: ${healthData.status}`,
          lastChecked: now,
          retryCount: isHealthy ? 0 : prev.retryCount,
          healthData,
        }));

        // Call the health change callback if provided
        if (onHealthChange && status.isHealthy !== isHealthy) {
          onHealthChange(isHealthy);
        }

        isCheckingRef.current = false;
        return isHealthy;
      } else {
        const now = new Date();
        setStatus((prev) => ({
          isHealthy: false,
          isLoading: false,
          error: `Health check failed with status: ${response.status}`,
          lastChecked: now,
          retryCount: prev.retryCount + 1,
          healthData: null,
        }));

        // Call the health change callback if provided
        if (onHealthChange && status.isHealthy !== false) {
          onHealthChange(false);
        }

        isCheckingRef.current = false;
        return false;
      }
    } catch (error) {
      const now = new Date();
      const errorMessage =
        error instanceof Error ? error.message : "Unknown error occurred";

      setStatus((prev) => ({
        isHealthy: false,
        isLoading: false,
        error: errorMessage,
        lastChecked: now,
        retryCount: prev.retryCount + 1,
        healthData: null,
      }));

      // Call the health change callback if provided
      if (onHealthChange && status.isHealthy !== false) {
        onHealthChange(false);
      }

      isCheckingRef.current = false;
      return false;
    }
  }, [onHealthChange, status.isHealthy]);

  const retryHealthCheck = useCallback(async () => {
    if (status.retryCount < maxRetries) {
      await checkHealth();
    }
  }, [checkHealth, status.retryCount, maxRetries]);

  const resetHealth = useCallback(() => {
    setStatus({
      isHealthy: true,
      isLoading: false,
      error: null,
      lastChecked: null,
      retryCount: 0,
      healthData: null,
    });
    hasCheckedRef.current = false;
  }, []);

  // Initial health check on startup only
  useEffect(() => {
    if (!enabled || !checkOnStartup || hasCheckedRef.current) return;

    hasCheckedRef.current = true;
    checkHealth();
  }, [enabled, checkOnStartup, checkHealth]);

  // Clean up on unmount
  useEffect(() => {
    return () => {
      isCheckingRef.current = false;
    };
  }, []);

  return {
    ...status,
    checkHealth,
    retryHealthCheck,
    resetHealth,
    canRetry: status.retryCount < maxRetries,
  };
};
