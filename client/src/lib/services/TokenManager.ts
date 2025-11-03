/**
 * Hybrid Token Management System
 *
 * Combines the best of both worlds:
 * - Class-based singleton for use anywhere (API, middleware, utilities)
 * - React hook wrapper for component convenience
 * - Shared cache across all usage patterns
 */

import { auth } from '@/lib/firebase';
import { User, onAuthStateChanged } from 'firebase/auth';
import { useEffect, useState, useCallback, useMemo } from 'react';

// ============================================================================
// CORE TOKEN MANAGER CLASS
// Used by: API layer, RTK Query, middleware, non-React code
// ============================================================================

interface TokenCache {
    token: string;
    expiry: number;
    userId: string;
}

interface TokenStatus {
    hasCachedToken: boolean;
    isValid: boolean;
    expiresIn: number | null;
    userId: string | null;
}

class TokenManagerClass {
    private cache: TokenCache | null = null;
    private refreshPromise: Promise<string> | null = null;
    private authStateUnsubscribe: (() => void) | null = null;
    private listeners: Set<(status: TokenStatus) => void> = new Set();

    private readonly REFRESH_BUFFER = 60000; // Refresh 1 minute before expiry
    private readonly MAX_RETRY_ATTEMPTS = 3;
    private readonly RETRY_DELAY = 1000;

    constructor() {
        this.initializeAuthListener();
    }

    /**
     * Initialize Firebase auth state listener
     */
    private initializeAuthListener(): void {
        this.authStateUnsubscribe = onAuthStateChanged(auth, (user) => {
            if (!user) {
                this.clearCache();
            } else if (this.cache && this.cache.userId !== user.uid) {
                // User changed, clear the cache
                this.clearCache();
            }
            // Notify React components of auth state change
            this.notifyListeners();
        });
    }

    /**
     * Subscribe to token status changes (used by React hook)
     */
    subscribe(listener: (status: TokenStatus) => void): () => void {
        this.listeners.add(listener);
        // Immediately notify of current status
        listener(this.getStatus());

        // Return unsubscribe function
        return () => {
            this.listeners.delete(listener);
        };
    }

    /**
     * Notify all listeners of status change
     */
    private notifyListeners(): void {
        const status = this.getStatus();
        this.listeners.forEach(listener => listener(status));
    }

    /**
     * Get current token status
     */
    getStatus(): TokenStatus {
        const user = auth.currentUser;

        if (!this.cache || !user) {
            return {
                hasCachedToken: false,
                isValid: false,
                expiresIn: null,
                userId: null,
            };
        }

        const isValid = this.isTokenValid(user.uid);
        const expiresIn = isValid
            ? Math.max(0, this.cache.expiry - Date.now())
            : null;

        return {
            hasCachedToken: true,
            isValid,
            expiresIn,
            userId: this.cache.userId,
        };
    }

    /**
     * Get a valid token, either from cache or by refreshing
     * This is the main method used by API layer
     */
    async getToken(): Promise<string | null> {
        const user = auth.currentUser;

        if (!user) {
            this.clearCache();
            return null;
        }

        // Check if we have a valid cached token
        if (this.isTokenValid(user.uid)) {
            return this.cache!.token;
        }

        // If a refresh is already in progress, wait for it
        if (this.refreshPromise) {
            try {
                return await this.refreshPromise;
            } catch (error) {
                // If the shared refresh failed, try again
                return this.getToken();
            }
        }

        // Start a new refresh
        return this.refreshToken(user);
    }

    /**
     * Check if the cached token is still valid
     */
    private isTokenValid(userId: string): boolean {
        if (!this.cache) return false;
        if (this.cache.userId !== userId) return false;

        const now = Date.now();
        const expiryWithBuffer = this.cache.expiry - this.REFRESH_BUFFER;

        return now < expiryWithBuffer;
    }

    /**
     * Refresh the token with retry logic
     */
    private async refreshToken(user: User): Promise<string> {
        // Create a shared promise to prevent concurrent refreshes
        this.refreshPromise = this.performRefresh(user);

        try {
            const token = await this.refreshPromise;
            this.notifyListeners(); // Notify React components
            return token;
        } finally {
            this.refreshPromise = null;
        }
    }

    /**
     * Perform the actual token refresh with retry logic
     */
    private async performRefresh(user: User, attempt = 1): Promise<string> {
        try {
            console.debug(`[TokenManager] Refreshing token (attempt ${attempt})`);

            // Force refresh the token
            const token = await user.getIdToken(true);

            // Parse the token to get expiry
            const payload = this.parseJWT(token);

            if (!payload || !payload.exp) {
                throw new Error('Invalid token format');
            }

            // Cache the new token
            this.cache = {
                token,
                expiry: payload.exp * 1000, // Convert to milliseconds
                userId: user.uid,
            };

            console.debug('[TokenManager] Token refreshed successfully');
            return token;

        } catch (error) {
            console.error(`[TokenManager] Token refresh failed (attempt ${attempt}):`, error);

            // Retry logic
            if (attempt < this.MAX_RETRY_ATTEMPTS) {
                await this.delay(this.RETRY_DELAY * attempt);
                return this.performRefresh(user, attempt + 1);
            }

            // Clear cache on final failure
            this.clearCache();
            throw new Error(`Failed to refresh token after ${this.MAX_RETRY_ATTEMPTS} attempts`);
        }
    }

    /**
     * Parse JWT token to extract payload
     */
    private parseJWT(token: string): any {
        try {
            const parts = token.split('.');
            if (parts.length !== 3) {
                throw new Error('Invalid JWT format');
            }

            const payload = parts[1];
            const decoded = atob(payload.replace(/-/g, '+').replace(/_/g, '/'));
            return JSON.parse(decoded);
        } catch (error) {
            console.error('[TokenManager] Failed to parse JWT:', error);
            return null;
        }
    }

    /**
     * Clear the token cache
     */
    clearCache(): void {
        this.cache = null;
        this.refreshPromise = null;
        this.notifyListeners();
        console.debug('[TokenManager] Cache cleared');
    }

    /**
     * Utility delay function for retry logic
     */
    private delay(ms: number): Promise<void> {
        return new Promise(resolve => setTimeout(resolve, ms));
    }

    /**
     * Preemptively refresh token if it's close to expiry
     */
    async ensureFreshToken(): Promise<void> {
        const user = auth.currentUser;
        if (!user) return;

        const status = this.getStatus();

        // Refresh if token expires in less than 5 minutes
        if (!status.isValid || (status.expiresIn && status.expiresIn < 300000)) {
            await this.refreshToken(user);
        }
    }

    /**
     * Force a token refresh (useful for error recovery)
     */
    async forceRefresh(): Promise<string | null> {
        const user = auth.currentUser;
        if (!user) return null;

        this.clearCache();
        return this.refreshToken(user);
    }

    /**
     * Clean up resources
     */
    dispose(): void {
        if (this.authStateUnsubscribe) {
            this.authStateUnsubscribe();
            this.authStateUnsubscribe = null;
        }
        this.clearCache();
        this.listeners.clear();
    }
}

// ============================================================================
// SINGLETON INSTANCE
// This is what gets used everywhere
// ============================================================================

export const tokenManager = new TokenManagerClass();

// ============================================================================
// REACT HOOK WRAPPER
// Convenient interface for React components
// ============================================================================

interface UseTokenManagerReturn {
    // Token operations
    getToken: () => Promise<string | null>;
    clearCache: () => void;
    ensureFreshToken: () => Promise<void>;
    forceRefresh: () => Promise<string | null>;

    // Status information
    status: TokenStatus;
    isAuthenticated: boolean;
    isTokenValid: boolean;
    tokenExpiresIn: number | null;
}

/**
 * React hook for token management
 *
 * Usage in components:
 * ```tsx
 * function MyComponent() {
 *     const { getToken, isAuthenticated, status } = useTokenManager();
 *
 *     const handleApiCall = async () => {
 *         const token = await getToken();
 *         // Use token...
 *     };
 * }
 * ```
 */
export function useTokenManager(): UseTokenManagerReturn {
    const [status, setStatus] = useState<TokenStatus>(tokenManager.getStatus());

    // Subscribe to token status changes
    useEffect(() => {
        const unsubscribe = tokenManager.subscribe((newStatus) => {
            setStatus(newStatus);
        });

        return unsubscribe;
    }, []);

    // Memoize methods to prevent unnecessary re-renders
    const getToken = useCallback(() => tokenManager.getToken(), []);
    const clearCache = useCallback(() => tokenManager.clearCache(), []);
    const ensureFreshToken = useCallback(() => tokenManager.ensureFreshToken(), []);
    const forceRefresh = useCallback(() => tokenManager.forceRefresh(), []);

    // Derived state
    const isAuthenticated = useMemo(() => status.userId !== null, [status.userId]);
    const isTokenValid = useMemo(() => status.isValid, [status.isValid]);
    const tokenExpiresIn = useMemo(() => status.expiresIn, [status.expiresIn]);

    return {
        // Operations
        getToken,
        clearCache,
        ensureFreshToken,
        forceRefresh,

        // Status
        status,
        isAuthenticated,
        isTokenValid,
        tokenExpiresIn,
    };
}

// ============================================================================
// USAGE EXAMPLES
// ============================================================================

/**
 * Example 1: Using in API layer (non-React)
 *
 * ```typescript
 * // src/features/api/apiSlice.ts
 * import { tokenManager } from '@/lib/services/TokenManager';
 *
 * const baseQuery = fetchBaseQuery({
 *     prepareHeaders: async (headers) => {
 *         // Direct class usage - works outside React
 *         const token = await tokenManager.getToken();
 *         if (token) {
 *             headers.set("Authorization", `Bearer ${token}`);
 *         }
 *         return headers;
 *     },
 * });
 * ```
 */

/**
 * Example 2: Using in React component
 *
 * ```tsx
 * function UserProfile() {
 *     const { isAuthenticated, tokenExpiresIn, getToken } = useTokenManager();
 *
 *     if (!isAuthenticated) {
 *         return <LoginPrompt />;
 *     }
 *
 *     return (
 *         <div>
 *             <p>Token expires in: {Math.round(tokenExpiresIn / 1000)}s</p>
 *             <button onClick={() => getToken()}>Refresh Token</button>
 *         </div>
 *     );
 * }
 * ```
 */

/**
 * Example 3: Using in middleware
 *
 * ```typescript
 * // src/middleware/auth.ts
 * import { tokenManager } from '@/lib/services/TokenManager';
 *
 * export async function authMiddleware(req, next) {
 *     const token = await tokenManager.getToken();
 *     if (!token) {
 *         throw new Error('Unauthorized');
 *     }
 *     req.headers.authorization = `Bearer ${token}`;
 *     return next();
 * }
 * ```
 */

// ============================================================================
// TYPE EXPORTS
// ============================================================================

export type { TokenCache, TokenStatus };
