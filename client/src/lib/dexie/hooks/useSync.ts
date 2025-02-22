import { useEffect, useCallback } from 'react';
import { SyncService } from '@/lib/dexie/db-sync';

export function useSync(userId: string | null) {
  const syncService = SyncService.getInstance();

  const syncData = useCallback(async () => {
    if (!userId) return;
    
    try {
      await syncService.syncUserData(userId);
      await syncService.syncPosts(userId);
    } catch (error) {
      console.error('Sync failed:', error);
    }
  }, [syncService, userId]);

  useEffect(() => {
    if (!userId) return;

    // Initial sync
    syncData();

    // Set up periodic sync
    const syncInterval = setInterval(syncData, 5000); // Sync every 5 seconds

    // Cleanup on unmount
    return () => {
      clearInterval(syncInterval);
    };
  }, [userId, syncData]);

  return { syncNow: syncData };
}