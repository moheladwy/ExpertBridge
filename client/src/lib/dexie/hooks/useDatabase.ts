import { useEffect, useState } from 'react';
import { DatabaseService } from '@/lib/dexie/db-service';

export function useDatabase() {
  const [isInitialized, setIsInitialized] = useState(false);
  const [error, setError] = useState<Error | null>(null);

  useEffect(() => {
    const initDb = async () => {
      try {
        const dbService = DatabaseService.getInstance();
        await dbService.initializeDatabase();
        setIsInitialized(true);
      } catch (err) {
        setError(err as Error);
      }
    };

    initDb();
  }, []);

  return { isInitialized, error };
}