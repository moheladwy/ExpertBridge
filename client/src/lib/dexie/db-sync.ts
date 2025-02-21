import db from '@/lib/dexie/db-scheme';
import type { User } from '@/lib/api/interfaces';
import { USER_ENDPOINTS } from '@/lib/api/endpoints';

export class SyncService {
  private static instance: SyncService;

  private constructor() {}
  
  public static getInstance(): SyncService {
    if (!SyncService.instance) {
      SyncService.instance = new SyncService();
    }
    return SyncService.instance;
  }

  async syncUserData(firebaseId: string) {
    try {
      // Fetch user data from server
      const response = await fetch(`${USER_ENDPOINTS.GET_USER_BY_FIREBASE_ID}/${firebaseId}`);
      const userData: User = await response.json();

      // Update local database
      await db.transaction('rw', [db.users, db.profiles], async () => {
        await db.users.put(userData);
        if (userData.profile) {
          await db.profiles.put(userData.profile);
        }
      });
    } catch (error) {
      console.error('Error syncing user data:', error);
      throw error;
    }
  }

  // eslint-disable-next-line @typescript-eslint/no-unused-vars
  async syncPosts(userId: string) {
    try {
      // TODO: Fetch posts from server
      // and update local database
    } catch (error) {
      console.error('Error syncing posts:', error);
      throw error;
    }
  }

  // Add more sync methods as needed
}
