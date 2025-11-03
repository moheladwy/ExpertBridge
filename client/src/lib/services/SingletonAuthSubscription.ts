/**
 * SingletonAuthSubscription - Global singleton to prevent multiple auth subscriptions
 *
 * CRITICAL FIX: This singleton ensures only ONE auth subscription exists globally,
 * preventing the memory leak where each component creates its own subscription.
 *
 * Problem it solves:
 * - Before: 82+ subscriptions after loading 80 posts (memory leak!)
 * - After: 1 subscription shared by all components
 */

import { User } from "firebase/auth";
import { useEffect, useState } from "react";

type Listener = (user: User | null) => void;
type UnsubscribeFn = () => void;

// Import authStateManager dynamically to avoid circular dependency
let authStateManager: any = null;

class SingletonAuthSubscription {
  private static instance: SingletonAuthSubscription | null = null;
  private currentUser: User | null = null;
  private listeners = new Set<Listener>();
  private subscription: UnsubscribeFn | null = null;
  private subscriptionCount = 0;
  private initialized = false;

  private constructor() {
    // Private constructor to enforce singleton
  }

  /**
   * Get the singleton instance
   */
  public static getInstance(): SingletonAuthSubscription {
    if (!SingletonAuthSubscription.instance) {
      SingletonAuthSubscription.instance = new SingletonAuthSubscription();
    }
    return SingletonAuthSubscription.instance;
  }

  /**
   * Initialize the single subscription (lazy initialization)
   */
  private async initialize(): Promise<void> {
    if (this.initialized) return;

    // Lazy import to avoid circular dependency
    if (!authStateManager) {
      const module = await import("./AuthStateManager");
      authStateManager = module.authStateManager;
    }

    // Create ONE subscription that will be shared by all components
    this.subscription = authStateManager.subscribeToUserChanges((user: User | null) => {
      this.currentUser = user;
      this.notifyListeners(user);
    });

    // Get initial user
    this.currentUser = authStateManager.getCurrentUser();
    this.initialized = true;

    if (process.env.NODE_ENV === "development") {
      console.log(
        "[SingletonAuthSubscription] Initialized with single subscription to AuthStateManager"
      );
    }
  }

  /**
   * Subscribe to auth changes (does NOT create a new auth subscription)
   */
  public async subscribe(listener: Listener): Promise<UnsubscribeFn> {
    // Ensure initialized
    await this.initialize();

    this.listeners.add(listener);
    this.subscriptionCount++;

    if (process.env.NODE_ENV === "development") {
      console.log(
        `[SingletonAuthSubscription] Component subscribed (${this.subscriptionCount} total components)`,
        {
          stackTrace: new Error().stack?.split("\n").slice(2, 4).join("\n"),
        }
      );
    }

    // Immediately notify with current user
    listener(this.currentUser);

    // Return unsubscribe function
    return () => {
      this.listeners.delete(listener);
      this.subscriptionCount--;

      if (process.env.NODE_ENV === "development") {
        console.log(
          `[SingletonAuthSubscription] Component unsubscribed (${this.subscriptionCount} remaining)`
        );
      }
    };
  }

  /**
   * Notify all listeners of user change
   */
  private notifyListeners(user: User | null): void {
    this.listeners.forEach((listener) => {
      try {
        listener(user);
      } catch (error) {
        console.error("[SingletonAuthSubscription] Listener error:", error);
      }
    });
  }

  /**
   * Get current user synchronously
   */
  public getCurrentUser(): User | null {
    return this.currentUser;
  }

  /**
   * Get statistics for debugging
   */
  public getStats(): {
    componentCount: number;
    hasAuthSubscription: boolean;
    currentUser: string | null;
  } {
    return {
      componentCount: this.subscriptionCount,
      hasAuthSubscription: this.subscription !== null,
      currentUser: this.currentUser?.email || null,
    };
  }

  /**
   * Cleanup (should never be called in normal operation)
   */
  public dispose(): void {
    if (this.subscription) {
      this.subscription();
      this.subscription = null;
    }
    this.listeners.clear();
    this.subscriptionCount = 0;
    this.initialized = false;
    SingletonAuthSubscription.instance = null;

    if (process.env.NODE_ENV === "development") {
      console.log("[SingletonAuthSubscription] Disposed");
    }
  }
}

// Export singleton instance
export const singletonAuth = SingletonAuthSubscription.getInstance();

// ============================================================================
// REACT HOOKS
// ============================================================================

/**
 * Hook that uses the singleton subscription
 * This replaces useCurrentUser/useCurrentAuthUser to prevent multiple subscriptions
 *
 * @example
 * ```tsx
 * // OLD - Creates a subscription per component
 * const authUser = useCurrentAuthUser();
 *
 * // NEW - Uses shared singleton subscription
 * const authUser = useSingletonAuth();
 * ```
 */
export function useSingletonAuth(): User | null {
  const [user, setUser] = useState<User | null>(singletonAuth.getCurrentUser());
  const [isInitialized, setIsInitialized] = useState(false);

  useEffect(() => {
    let unsubscribe: UnsubscribeFn | null = null;

    // Async initialization to avoid circular dependency
    const setupSubscription = async () => {
      unsubscribe = await singletonAuth.subscribe(setUser);
      setIsInitialized(true);
    };

    setupSubscription();

    return () => {
      if (unsubscribe) {
        unsubscribe();
      }
    };
  }, []);

  return user;
}

/**
 * Hook with additional auth info
 */
export function useSingletonAuthState() {
  const user = useSingletonAuth();

  return {
    user,
    isAuthenticated: user !== null,
    userId: user?.uid || null,
    email: user?.email || null,
  };
}

// ============================================================================
// MONITORING
// ============================================================================

if (process.env.NODE_ENV === "development") {
  // Expose to window for debugging
  (window as any).__singletonAuth = singletonAuth;

  // Monitor for leaks
  let previousCount = 0;
  setInterval(() => {
    const stats = singletonAuth.getStats();
    if (stats.componentCount !== previousCount) {
      console.log("[SingletonAuth] Component count changed:", {
        from: previousCount,
        to: stats.componentCount,
        delta: stats.componentCount - previousCount,
      });

      // Warn if growing too fast
      if (stats.componentCount > previousCount + 10) {
        console.error(
          "⚠️ POSSIBLE LEAK: Component count grew by",
          stats.componentCount - previousCount
        );
      }

      previousCount = stats.componentCount;
    }
  }, 5000);
}
