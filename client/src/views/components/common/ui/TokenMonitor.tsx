/**
 * Token Monitor Component
 *
 * Development-only component for monitoring token caching performance.
 * Shows real-time token status and helps verify caching is working.
 */

import React, { useState, useEffect } from 'react';
import { useTokenManager } from '@/lib/services/TokenManager';

const TokenMonitor: React.FC = () => {
  const {
    getToken,
    clearCache,
    ensureFreshToken,
    forceRefresh,
    status,
    isAuthenticated,
    isTokenValid,
    tokenExpiresIn,
  } = useTokenManager();

  const [lastAction, setLastAction] = useState<string>('None');
  const [actionTime, setActionTime] = useState<number | null>(null);
  const [tokenFetches, setTokenFetches] = useState<number>(0);

  // Only show in development
  if (process.env.NODE_ENV !== 'development') {
    return null;
  }

  const formatTime = (ms: number | null): string => {
    if (!ms) return 'N/A';
    const seconds = Math.floor(ms / 1000);
    const minutes = Math.floor(seconds / 60);
    const hours = Math.floor(minutes / 60);

    if (hours > 0) {
      return `${hours}h ${minutes % 60}m`;
    }
    if (minutes > 0) {
      return `${minutes}m ${seconds % 60}s`;
    }
    return `${seconds}s`;
  };

  const handleGetToken = async () => {
    const start = Date.now();
    setLastAction('Getting token...');

    try {
      const token = await getToken();
      const elapsed = Date.now() - start;
      setActionTime(elapsed);
      setTokenFetches(prev => prev + 1);
      setLastAction(token ? 'Token retrieved (cached)' : 'No token available');

      // Log performance
      console.log(`[TokenMonitor] Token fetch took ${elapsed}ms`, {
        cached: elapsed < 10,
        token: token?.substring(0, 20) + '...',
      });
    } catch (error) {
      setLastAction('Error getting token');
      console.error('[TokenMonitor] Error:', error);
    }
  };

  const handleClearCache = () => {
    clearCache();
    setLastAction('Cache cleared');
    setActionTime(null);
    console.log('[TokenMonitor] Cache cleared');
  };

  const handleEnsureFresh = async () => {
    const start = Date.now();
    setLastAction('Ensuring fresh token...');

    try {
      await ensureFreshToken();
      const elapsed = Date.now() - start;
      setActionTime(elapsed);
      setLastAction('Token freshness ensured');
      console.log(`[TokenMonitor] Ensure fresh took ${elapsed}ms`);
    } catch (error) {
      setLastAction('Error ensuring fresh token');
      console.error('[TokenMonitor] Error:', error);
    }
  };

  const handleForceRefresh = async () => {
    const start = Date.now();
    setLastAction('Force refreshing...');

    try {
      const token = await forceRefresh();
      const elapsed = Date.now() - start;
      setActionTime(elapsed);
      setTokenFetches(prev => prev + 1);
      setLastAction(token ? 'Token force refreshed' : 'No token after refresh');
      console.log(`[TokenMonitor] Force refresh took ${elapsed}ms`);
    } catch (error) {
      setLastAction('Error force refreshing');
      console.error('[TokenMonitor] Error:', error);
    }
  };

  // Auto-refresh display every second
  useEffect(() => {
    const interval = setInterval(() => {
      // Force re-render to update expiry time
      setLastAction(prev => prev);
    }, 1000);

    return () => clearInterval(interval);
  }, []);

  return (
    <div className="fixed bottom-4 right-4 w-80 bg-white dark:bg-gray-800 rounded-lg shadow-lg border border-gray-200 dark:border-gray-700 p-4 z-50">
      <div className="flex items-center justify-between mb-3">
        <h3 className="text-sm font-semibold text-gray-900 dark:text-white">
          🔐 Token Monitor
        </h3>
        <span className="text-xs text-gray-500 dark:text-gray-400">
          Fetches: {tokenFetches}
        </span>
      </div>

      <div className="space-y-2 text-xs">
        {/* Status Indicators */}
        <div className="flex items-center justify-between">
          <span className="text-gray-600 dark:text-gray-400">Authenticated:</span>
          <span className={isAuthenticated ? 'text-green-600' : 'text-red-600'}>
            {isAuthenticated ? '✅ Yes' : '❌ No'}
          </span>
        </div>

        <div className="flex items-center justify-between">
          <span className="text-gray-600 dark:text-gray-400">Token Valid:</span>
          <span className={isTokenValid ? 'text-green-600' : 'text-yellow-600'}>
            {isTokenValid ? '✅ Valid' : '⚠️ Invalid/Expired'}
          </span>
        </div>

        <div className="flex items-center justify-between">
          <span className="text-gray-600 dark:text-gray-400">Has Cache:</span>
          <span className={status.hasCachedToken ? 'text-green-600' : 'text-gray-500'}>
            {status.hasCachedToken ? '✅ Yes' : '➖ No'}
          </span>
        </div>

        <div className="flex items-center justify-between">
          <span className="text-gray-600 dark:text-gray-400">Expires In:</span>
          <span className={`font-mono ${
            tokenExpiresIn && tokenExpiresIn < 300000 ? 'text-yellow-600' : 'text-gray-900 dark:text-white'
          }`}>
            {formatTime(tokenExpiresIn)}
          </span>
        </div>

        {status.userId && (
          <div className="flex items-center justify-between">
            <span className="text-gray-600 dark:text-gray-400">User ID:</span>
            <span className="font-mono text-gray-900 dark:text-white text-xs">
              {status.userId.substring(0, 12)}...
            </span>
          </div>
        )}

        {/* Last Action */}
        <div className="pt-2 mt-2 border-t border-gray-200 dark:border-gray-700">
          <div className="flex items-center justify-between">
            <span className="text-gray-600 dark:text-gray-400">Last Action:</span>
            <span className="text-gray-900 dark:text-white">{lastAction}</span>
          </div>
          {actionTime !== null && (
            <div className="flex items-center justify-between mt-1">
              <span className="text-gray-600 dark:text-gray-400">Time Taken:</span>
              <span className={`font-mono ${
                actionTime < 10 ? 'text-green-600' : actionTime < 100 ? 'text-yellow-600' : 'text-red-600'
              }`}>
                {actionTime}ms {actionTime < 10 && '(cached!)'}
              </span>
            </div>
          )}
        </div>

        {/* Action Buttons */}
        <div className="grid grid-cols-2 gap-2 pt-2 mt-2 border-t border-gray-200 dark:border-gray-700">
          <button
            onClick={handleGetToken}
            className="px-2 py-1 text-xs bg-blue-600 text-white rounded hover:bg-blue-700 transition-colors"
          >
            Get Token
          </button>
          <button
            onClick={handleClearCache}
            className="px-2 py-1 text-xs bg-red-600 text-white rounded hover:bg-red-700 transition-colors"
          >
            Clear Cache
          </button>
          <button
            onClick={handleEnsureFresh}
            className="px-2 py-1 text-xs bg-green-600 text-white rounded hover:bg-green-700 transition-colors"
          >
            Ensure Fresh
          </button>
          <button
            onClick={handleForceRefresh}
            className="px-2 py-1 text-xs bg-yellow-600 text-white rounded hover:bg-yellow-700 transition-colors"
          >
            Force Refresh
          </button>
        </div>

        {/* Performance Indicator */}
        <div className="pt-2 mt-2 border-t border-gray-200 dark:border-gray-700">
          <div className="flex items-center justify-between">
            <span className="text-gray-600 dark:text-gray-400">Performance:</span>
            <span className={`text-xs ${
              actionTime && actionTime < 10
                ? 'text-green-600 font-semibold'
                : actionTime && actionTime < 100
                ? 'text-yellow-600'
                : 'text-gray-500'
            }`}>
              {actionTime && actionTime < 10
                ? '⚡ Cached (Excellent)'
                : actionTime && actionTime < 100
                ? '🔄 Fresh (Good)'
                : actionTime
                ? '🐌 Slow (Check network)'
                : '⏸️ Idle'}
            </span>
          </div>
        </div>

        {/* Instructions */}
        <details className="pt-2 mt-2 border-t border-gray-200 dark:border-gray-700">
          <summary className="cursor-pointer text-gray-600 dark:text-gray-400 hover:text-gray-900 dark:hover:text-white">
            ℹ️ How to test
          </summary>
          <div className="mt-2 space-y-1 text-gray-600 dark:text-gray-400">
            <p>1. Click "Get Token" - should be &lt;10ms (cached)</p>
            <p>2. Click "Clear Cache" then "Get Token" - will be slower (fresh)</p>
            <p>3. Watch "Expires In" countdown</p>
            <p>4. Token auto-refreshes 1 min before expiry</p>
          </div>
        </details>
      </div>
    </div>
  );
};

export default TokenMonitor;
