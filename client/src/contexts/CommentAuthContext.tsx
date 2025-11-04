/**
 * CommentAuthContext
 *
 * This context provides auth user information to comment components,
 * preventing each comment card from creating its own auth subscription.
 *
 * Problem it solves:
 * - Before: 31 separate auth subscriptions (one per comment/vote button)
 * - After: 1 subscription shared across all comment components
 */

import React, { createContext, useContext, ReactNode } from 'react';
import { User } from 'firebase/auth';
import { useCurrentUser } from '@/lib/services/AuthStateManager';

interface CommentAuthContextType {
  authUser: User | null;
  isAuthenticated: boolean;
}

const CommentAuthContext = createContext<CommentAuthContextType | undefined>(undefined);

/**
 * Provider component that should wrap comment sections
 * Gets auth user ONCE and shares it with all child components
 */
export const CommentAuthProvider: React.FC<{ children: ReactNode }> = ({ children }) => {
  // Single subscription to auth state for all comments
  const authUser = useCurrentUser();
  const isAuthenticated = authUser !== null;

  return (
    <CommentAuthContext.Provider value={{ authUser, isAuthenticated }}>
      {children}
    </CommentAuthContext.Provider>
  );
};

/**
 * Hook to use auth user in comment components
 * This does NOT create a new subscription - it just reads from context
 */
export const useCommentAuth = (): CommentAuthContextType => {
  const context = useContext(CommentAuthContext);

  if (context === undefined) {
    throw new Error('useCommentAuth must be used within CommentAuthProvider');
  }

  return context;
};

/**
 * Optional: Hook that returns just the user
 * For drop-in replacement of useCurrentAuthUser in comment components
 */
export const useCommentAuthUser = (): User | null => {
  const { authUser } = useCommentAuth();
  return authUser;
};
