/**
 * Auth State Monitor Component
 *
 * Development-only component for monitoring centralized auth state.
 * Helps verify that duplicate onAuthStateChanged listeners have been eliminated.
 */

import React, { useState, useEffect } from 'react';
import {
  useAuthState,
  useAuthOperations,
  authStateManager,
} from '@/lib/services/AuthStateManager';

const AuthStateMonitor: React.FC = () => {
  const { user, loading, error, initialized, isAuthenticated } = useAuthState();
  const { refreshUser, getStats } = useAuthOperations();
  const [stats, setStats] = useState(getStats());
  const [lastAction, setLastAction] = useState<string>('Monitoring...');
  const [isExpanded, setIsExpanded] = useState(true);

  // Only show in development
  if (process.env.NODE_ENV !== 'development') {
    return null;
  }

  // Update stats periodically
  useEffect(() => {
    const interval = setInterval(() => {
      setStats(getStats());
    }, 1000);

    return () => clearInterval(interval);
  }, [getStats]);

  // Log listener count changes
  useEffect(() => {
    const currentCount = stats.listenerCount + stats.userChangeListenerCount;
    console.log(`[AuthStateMonitor] Total listeners: ${currentCount}`, {
      stateListeners: stats.listenerCount,
      userChangeListeners: stats.userChangeListenerCount,
    });
  }, [stats.listenerCount, stats.userChangeListenerCount]);

  const handleRefreshUser = async () => {
    setLastAction('Refreshing user...');
    try {
      const refreshedUser = await refreshUser();
      setLastAction(
        refreshedUser ? `User refreshed: ${refreshedUser.email}` : 'No user'
      );
    } catch (error) {
      setLastAction('Error refreshing user');
      console.error('[AuthStateMonitor] Refresh error:', error);
    }
  };

  const totalListeners = stats.listenerCount + stats.userChangeListenerCount;
  const isHealthy = totalListeners <= 5; // Should have very few listeners
  const isDanger = totalListeners > 10; // Too many listeners indicates a problem

  if (!isExpanded) {
    return (
      <div className="fixed bottom-4 left-4 z-50">
        <button
          onClick={() => setIsExpanded(true)}
          className={`px-3 py-2 rounded-lg shadow-lg text-xs font-medium transition-all ${
            isDanger
              ? 'bg-red-600 text-white animate-pulse'
              : isHealthy
              ? 'bg-green-600 text-white'
              : 'bg-yellow-600 text-white'
          }`}
        >
          👁️ Auth: {totalListeners} listeners
        </button>
      </div>
    );
  }

  return (
    <div className="fixed bottom-4 left-4 w-80 bg-white dark:bg-gray-800 rounded-lg shadow-lg border border-gray-200 dark:border-gray-700 p-4 z-50">
      <div className="flex items-center justify-between mb-3">
        <h3 className="text-sm font-semibold text-gray-900 dark:text-white flex items-center gap-2">
          <span>👁️ Auth State Monitor</span>
          {isDanger && (
            <span className="text-xs bg-red-600 text-white px-2 py-0.5 rounded animate-pulse">
              PROBLEM
            </span>
          )}
        </h3>
        <button
          onClick={() => setIsExpanded(false)}
          className="text-gray-500 hover:text-gray-700 dark:text-gray-400 dark:hover:text-gray-200"
        >
          ✕
        </button>
      </div>

      <div className="space-y-2 text-xs">
        {/* Listener Count - Most Important Metric */}
        <div
          className={`p-2 rounded-md ${
            isDanger
              ? 'bg-red-100 dark:bg-red-900/30 border border-red-300 dark:border-red-700'
              : isHealthy
              ? 'bg-green-100 dark:bg-green-900/30 border border-green-300 dark:border-green-700'
              : 'bg-yellow-100 dark:bg-yellow-900/30 border border-yellow-300 dark:border-yellow-700'
          }`}
        >
          <div className="flex items-center justify-between mb-1">
            <span className="font-semibold">Total Listeners:</span>
            <span
              className={`font-bold text-lg ${
                isDanger
                  ? 'text-red-600 dark:text-red-400'
                  : isHealthy
                  ? 'text-green-600 dark:text-green-400'
                  : 'text-yellow-600 dark:text-yellow-400'
              }`}
            >
              {totalListeners}
            </span>
          </div>
          <div className="text-gray-600 dark:text-gray-400">
            <div className="flex justify-between">
              <span>State listeners:</span>
              <span>{stats.listenerCount}</span>
            </div>
            <div className="flex justify-between">
              <span>User change listeners:</span>
              <span>{stats.userChangeListenerCount}</span>
            </div>
          </div>
          {isDanger && (
            <div className="mt-2 text-red-600 dark:text-red-400 font-semibold">
              ⚠️ Too many listeners! Check for duplicate hooks.
            </div>
          )}
          {isHealthy && (
            <div className="mt-2 text-green-600 dark:text-green-400">
              ✅ Single listener pattern working correctly!
            </div>
          )}
        </div>

        {/* Auth Status */}
        <div className="flex items-center justify-between">
          <span className="text-gray-600 dark:text-gray-400">Status:</span>
          <span className="font-medium">
            {loading ? (
              <span className="text-yellow-600">⏳ Loading...</span>
            ) : isAuthenticated ? (
              <span className="text-green-600">✅ Authenticated</span>
            ) : (
              <span className="text-gray-500">❌ Not authenticated</span>
            )}
          </span>
        </div>

        <div className="flex items-center justify-between">
          <span className="text-gray-600 dark:text-gray-400">Initialized:</span>
          <span className={initialized ? 'text-green-600' : 'text-yellow-600'}>
            {initialized ? '✅ Yes' : '⏳ No'}
          </span>
        </div>

        {/* User Info */}
        {user && (
          <>
            <div className="pt-2 mt-2 border-t border-gray-200 dark:border-gray-700">
              <div className="flex items-center justify-between">
                <span className="text-gray-600 dark:text-gray-400">Email:</span>
                <span className="font-mono text-gray-900 dark:text-white text-xs truncate ml-2">
                  {user.email}
                </span>
              </div>
              <div className="flex items-center justify-between">
                <span className="text-gray-600 dark:text-gray-400">UID:</span>
                <span className="font-mono text-gray-900 dark:text-white text-xs">
                  {user.uid.substring(0, 12)}...
                </span>
              </div>
              <div className="flex items-center justify-between">
                <span className="text-gray-600 dark:text-gray-400">Verified:</span>
                <span
                  className={
                    user.emailVerified ? 'text-green-600' : 'text-yellow-600'
                  }
                >
                  {user.emailVerified ? '✅' : '⚠️'}
                </span>
              </div>
            </div>
          </>
        )}

        {/* Error State */}
        {error && (
          <div className="pt-2 mt-2 border-t border-red-200 dark:border-red-700">
            <div className="text-red-600 dark:text-red-400">
              <span className="font-semibold">Error:</span> {error.message}
            </div>
          </div>
        )}

        {/* Last Action */}
        <div className="pt-2 mt-2 border-t border-gray-200 dark:border-gray-700">
          <div className="flex items-center justify-between">
            <span className="text-gray-600 dark:text-gray-400">Last Action:</span>
            <span className="text-gray-900 dark:text-white text-xs">{lastAction}</span>
          </div>
        </div>

        {/* Actions */}
        <div className="flex gap-2 pt-2">
          <button
            onClick={handleRefreshUser}
            className="flex-1 px-2 py-1 text-xs bg-blue-600 text-white rounded hover:bg-blue-700 transition-colors"
          >
            Refresh User
          </button>
          <button
            onClick={() => {
              const currentStats = authStateManager.getStats();
              console.log('[AuthStateMonitor] Current stats:', currentStats);
              setLastAction('Stats logged to console');
            }}
            className="flex-1 px-2 py-1 text-xs bg-gray-600 text-white rounded hover:bg-gray-700 transition-colors"
          >
            Log Stats
          </button>
        </div>

        {/* Performance Summary */}
        <div className="pt-2 mt-2 border-t border-gray-200 dark:border-gray-700">
          <div className="text-center">
            {totalListeners === 1 ? (
              <div className="text-green-600 dark:text-green-400 font-semibold">
                🎉 Perfect! Single listener pattern achieved!
              </div>
            ) : totalListeners <= 3 ? (
              <div className="text-green-600 dark:text-green-400">
                ✅ Excellent - Minimal listeners
              </div>
            ) : totalListeners <= 5 ? (
              <div className="text-yellow-600 dark:text-yellow-400">
                ⚠️ Good - But could be optimized
              </div>
            ) : (
              <div className="text-red-600 dark:text-red-400 font-semibold">
                ❌ Problem - Multiple duplicate listeners detected
              </div>
            )}
          </div>
        </div>

        {/* Instructions */}
        <details className="pt-2 mt-2 border-t border-gray-200 dark:border-gray-700">
          <summary className="cursor-pointer text-gray-600 dark:text-gray-400 hover:text-gray-900 dark:hover:text-white">
            ℹ️ What to look for
          </summary>
          <div className="mt-2 space-y-1 text-gray-600 dark:text-gray-400">
            <p className="font-semibold">Goal: 1-3 total listeners</p>
            <p>✅ 1 listener = Perfect (single source)</p>
            <p>⚠️ 2-5 = Acceptable (some components)</p>
            <p>❌ 10+ = Problem (duplicate hooks)</p>
            <p className="mt-2">
              If you see many listeners, check for components using old
              useCurrentAuthUser() hook.
            </p>
          </div>
        </details>
      </div>
    </div>
  );
};

export default AuthStateMonitor;
