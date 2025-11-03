/**
 * FeedAuthContext - Optimized auth state management for feed components
 *
 * CRITICAL: This context solves the infinite scroll memory leak where each
 * post/comment/vote button was creating its own auth subscription.
 *
 * Problem it solves:
 * - Before: Loading 50 posts = 50+ auth subscriptions (memory leak!)
 * - After: Loading any number of posts = 1 auth subscription (shared)
 *
 * Usage:
 * 1. Wrap your feed/list components with FeedAuthProvider
 * 2. Use useFeedAuth() in child components instead of useCurrentAuthUser()
 * 3. Components will receive auth state without creating new subscriptions
 */

import React, { createContext, useContext, ReactNode, useMemo } from 'react';
import { User } from 'firebase/auth';
import { useCurrentUser } from '@/lib/services/AuthStateManager';
import { useAuthPrompt } from '@/contexts/AuthPromptContext';

// ============================================================================
// TYPES
// ============================================================================

interface FeedAuthContextType {
  /**
   * Current authenticated user or null if not logged in
   */
  authUser: User | null;

  /**
   * Quick boolean check for authentication status
   */
  isAuthenticated: boolean;

  /**
   * User ID for quick comparisons (e.g., checking if current user owns a post)
   */
  currentUserId: string | null;

  /**
   * Function to show auth prompt modal (from AuthPromptContext)
   * Call this when user tries to perform an action requiring auth
   */
  showAuthPrompt: () => void;
}

// ============================================================================
// CONTEXT
// ============================================================================

const FeedAuthContext = createContext<FeedAuthContextType | undefined>(undefined);

// ============================================================================
// PROVIDER
// ============================================================================

interface FeedAuthProviderProps {
  children: ReactNode;
  /**
   * Optional: Override auth user for testing
   */
  overrideUser?: User | null;
}

/**
 * FeedAuthProvider - Provides auth state to all child components
 *
 * This provider should wrap any component that renders lists of items
 * that need auth state (feeds, comment sections, notification lists, etc.)
 *
 * @example
 * ```tsx
 * // In Feed.tsx
 * return (
 *   <FeedAuthProvider>
 *     <div className="feed">
 *       {posts.map(post => (
 *         <PostCard key={post.id} post={post} />
 *       ))}
 *     </div>
 *   </FeedAuthProvider>
 * );
 * ```
 */
export const FeedAuthProvider: React.FC<FeedAuthProviderProps> = ({
  children,
  overrideUser
}) => {
  // SINGLE subscription for the entire feed - this is the key!
  const authUser = overrideUser !== undefined ? overrideUser : useCurrentUser();

  // Get auth prompt function from existing context
  const { showAuthPrompt } = useAuthPrompt();

  // Memoize the context value to prevent unnecessary re-renders
  const contextValue = useMemo<FeedAuthContextType>(() => ({
    authUser,
    isAuthenticated: authUser !== null,
    currentUserId: authUser?.uid || null,
    showAuthPrompt,
  }), [authUser, showAuthPrompt]);

  return (
    <FeedAuthContext.Provider value={contextValue}>
      {children}
    </FeedAuthContext.Provider>
  );
};

// ============================================================================
// HOOKS
// ============================================================================

/**
 * useFeedAuth - Primary hook for accessing auth state in feed components
 *
 * This hook MUST be used instead of useCurrentAuthUser() in any component
 * that's part of a list (posts, comments, notifications, etc.)
 *
 * @throws Error if used outside of FeedAuthProvider
 *
 * @example
 * ```tsx
 * // In PostVoteButtons.tsx
 * const PostVoteButtons = ({ post }) => {
 *   const { authUser, showAuthPrompt } = useFeedAuth();
 *
 *   const handleUpvote = () => {
 *     if (!authUser) {
 *       showAuthPrompt();
 *       return;
 *     }
 *     // Perform upvote...
 *   };
 * };
 * ```
 */
export const useFeedAuth = (): FeedAuthContextType => {
  const context = useContext(FeedAuthContext);

  if (context === undefined) {
    // Helpful error message for developers
    throw new Error(
      'useFeedAuth must be used within FeedAuthProvider. ' +
      'Make sure your component is wrapped with <FeedAuthProvider> ' +
      'at the feed/list level, not at individual item level.'
    );
  }

  return context;
};

/**
 * useFeedAuthUser - Convenience hook that returns just the auth user
 *
 * Drop-in replacement for useCurrentAuthUser() in feed components
 *
 * @example
 * ```tsx
 * // Before (creates subscription):
 * const authUser = useCurrentAuthUser();
 *
 * // After (no subscription):
 * const authUser = useFeedAuthUser();
 * ```
 */
export const useFeedAuthUser = (): User | null => {
  const { authUser } = useFeedAuth();
  return authUser;
};

/**
 * useIsCurrentUser - Check if a user ID matches the current user
 *
 * Useful for showing edit/delete buttons only for user's own content
 *
 * @example
 * ```tsx
 * const PostCard = ({ post }) => {
 *   const isOwner = useIsCurrentUser(post.userId);
 *
 *   return (
 *     <div>
 *       {isOwner && <button>Edit</button>}
 *     </div>
 *   );
 * };
 * ```
 */
export const useIsCurrentUser = (userId: string | null | undefined): boolean => {
  const { currentUserId } = useFeedAuth();
  return Boolean(userId && currentUserId && userId === currentUserId);
};

// ============================================================================
// UTILITIES
// ============================================================================

/**
 * withFeedAuth - HOC to inject feed auth props into a component
 *
 * Use this for class components or when you prefer props over hooks
 *
 * @example
 * ```tsx
 * const PostCard = ({ post, authUser, isAuthenticated }) => {
 *   // Use auth props directly
 * };
 *
 * export default withFeedAuth(PostCard);
 * ```
 */
export function withFeedAuth<P extends Partial<FeedAuthContextType>>(
  Component: React.ComponentType<P>
): React.ComponentType<Omit<P, keyof FeedAuthContextType>> {
  const WrappedComponent = (props: Omit<P, keyof FeedAuthContextType>) => {
    const feedAuth = useFeedAuth();

    const componentProps = {
      ...props,
      ...feedAuth,
    } as P;

    return <Component {...componentProps} />;
  };

  WrappedComponent.displayName = `withFeedAuth(${Component.displayName || Component.name || 'Component'})`;

  return WrappedComponent;
}

// ============================================================================
// DEVELOPMENT HELPERS
// ============================================================================

/**
 * FeedAuthDebugger - Development component to monitor auth state
 *
 * Shows current auth state in feed context for debugging
 */
export const FeedAuthDebugger: React.FC = () => {
  if (process.env.NODE_ENV !== 'development') {
    return null;
  }

  const { authUser, isAuthenticated, currentUserId } = useFeedAuth();

  return (
    <div className="fixed bottom-20 right-4 bg-purple-100 dark:bg-purple-900 p-2 rounded text-xs z-50">
      <div className="font-bold text-purple-900 dark:text-purple-100">Feed Auth</div>
      <div>Authenticated: {isAuthenticated ? '✅' : '❌'}</div>
      <div>User ID: {currentUserId || 'None'}</div>
      <div>Email: {authUser?.email || 'None'}</div>
    </div>
  );
};
