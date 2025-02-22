import db from '@/lib/dexie/db-scheme';
import { JobStatusEnum, MediaTypeEnum } from '@/lib/api/interfaces';

export class DatabaseService {
  private static instance: DatabaseService;

  private constructor() {}

  public static getInstance(): DatabaseService {
    if (!DatabaseService.instance) {
      DatabaseService.instance = new DatabaseService();
    }
    return DatabaseService.instance;
  }

  async initializeDatabase() {
    try {
      // Check if database exists and is accessible
      await db.open();
      console.log('Database opened successfully');

      // Check if this is first run (could check for specific tables or data)
      const needsInitialization = await this.checkIfNeedsInitialization();
      
      if (needsInitialization) {
        await this.populateInitialData();
      }

      return true;
    } catch (error) {
      console.error('Failed to initialize database:', error);
      throw error;
    }
  }

  private async checkIfNeedsInitialization(): Promise<boolean> {
    try {
      // Check for essential data like users, profiles, posts
      const userCount = await db.users.count();
      const profileCount = await db.profiles.count();
      const postCount = await db.posts.count();
      const needsInitialization = 
        postCount === 0
        || userCount === 0 
        || profileCount === 0;
      
      return needsInitialization;
    } catch (error) {
      console.error('Error checking database state:', error);
      return true;
    }
  }

  private async populateInitialData() {
        try {
          // Begin transaction for initial data
          await db.transaction('rw', 
            [db.mediaTypes, db.jobStatuses], 
            async () => {
              // Populate MediaTypes
              await db.mediaTypes.bulkPut([
                { id: crypto.randomUUID(), type: MediaTypeEnum.Video },
                { id: crypto.randomUUID(), type: MediaTypeEnum.Image },
                { id: crypto.randomUUID(), type: MediaTypeEnum.Audio },
                { id: crypto.randomUUID(), type: MediaTypeEnum.Document }
              ]);

              // Populate JobStatuses
              await db.jobStatuses.bulkPut([
                { id: crypto.randomUUID(), status: JobStatusEnum.Pending },
                { id: crypto.randomUUID(), status: JobStatusEnum.InProgress },
                { id: crypto.randomUUID(), status: JobStatusEnum.Completed },
                { id: crypto.randomUUID(), status: JobStatusEnum.Cancelled }
              ]);
          });

          console.log('Initial data populated successfully');
        } catch (error) {
          console.error('Error populating initial data:', error);
          throw error;
        }
      }

  async syncWithServer() {
    try {
      // TODO: Implement server synchronization logic here
      // This would typically involve:
      // 1. Fetching updates from server
      // 2. Merging with local data
      // 3. Handling conflicts
      console.log('Database synced with server');
    } catch (error) {
      console.error('Error syncing with server:', error);
      throw error;
    }
  }
}