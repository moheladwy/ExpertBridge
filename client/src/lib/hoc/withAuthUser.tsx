/**
 * Higher Order Component to inject auth user into components
 *
 * This HOC solves the problem of multiple auth subscriptions by:
 * - Getting the auth user at a higher level
 * - Passing it down as a prop
 * - Preventing each component from creating its own subscription
 *
 * Problem it solves:
 * - Before: 31+ separate auth subscriptions (one per component)
 * - After: Components receive auth user as prop, no subscription needed
 */

import React, { ComponentType } from 'react';
import { User } from 'firebase/auth';
import { useCurrentUser } from '@/lib/services/AuthStateManager';

/**
 * Props that will be injected into the wrapped component
 */
export interface WithAuthUserProps {
  authUser: User | null;
  isAuthenticated: boolean;
}

/**
 * HOC that injects auth user into a component
 *
 * Usage:
 * ```tsx
 * // Instead of this (creates a subscription):
 * const MyComponent = () => {
 *   const authUser = useCurrentAuthUser(); // Creates subscription!
 *   return <div>{authUser?.email}</div>;
 * };
 *
 * // Use this (no subscription):
 * const MyComponent = ({ authUser }: WithAuthUserProps) => {
 *   return <div>{authUser?.email}</div>;
 * };
 *
 * export default withAuthUser(MyComponent);
 * ```
 */
export function withAuthUser<P extends WithAuthUserProps>(
  Component: ComponentType<P>
): ComponentType<Omit<P, keyof WithAuthUserProps>> {
  const WrappedComponent = (props: Omit<P, keyof WithAuthUserProps>) => {
    // Single subscription at HOC level
    const authUser = useCurrentUser();
    const isAuthenticated = authUser !== null;

    // Pass auth user as props to wrapped component
    const componentProps = {
      ...props,
      authUser,
      isAuthenticated,
    } as P;

    return <Component {...componentProps} />;
  };

  // Set display name for debugging
  WrappedComponent.displayName = `withAuthUser(${Component.displayName || Component.name || 'Component'})`;

  return WrappedComponent;
}

/**
 * Alternative: Hook that returns auth user from props
 * For components that are already wrapped with withAuthUser
 *
 * This prevents accidental double subscriptions
 */
export function useAuthUserFromProps(props: any): User | null {
  if ('authUser' in props) {
    return props.authUser as User | null;
  }

  // Warn in development if auth user not in props
  if (process.env.NODE_ENV === 'development') {
    console.warn(
      'useAuthUserFromProps: authUser not found in props. ' +
      'Make sure the component is wrapped with withAuthUser HOC.'
    );
  }

  return null;
}

/**
 * Utility to check if a component has auth user in props
 */
export function hasAuthUserProp(props: any): props is WithAuthUserProps {
  return 'authUser' in props && 'isAuthenticated' in props;
}
