/**
 * Legacy hook for backward compatibility
 *
 * This hook now uses the centralized auth state manager instead of creating
 * its own onAuthStateChanged listener. This eliminates duplicate listeners
 * and improves performance.
 *
 * @deprecated Prefer using useCurrentUser or useAuthState from AuthStateManager
 */
import { type User } from "firebase/auth";
import { useSingletonAuth } from "@/lib/services/SingletonAuthSubscription";

export function useCurrentAuthUser(): User | null {
  // Using singleton subscription - prevents memory leak!
  // This ensures only ONE global auth subscription exists
  return useSingletonAuth();
}
