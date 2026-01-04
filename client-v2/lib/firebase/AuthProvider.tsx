"use client";

import {
	createContext,
	useContext,
	useState,
	useEffect,
	type ReactNode,
} from "react";
import { onAuthStateChanged, type User } from "firebase/auth";
import { auth } from "./index";

interface AuthContextValue {
	/** Firebase user object, null if not authenticated */
	user: User | null;
	/** True until initial auth state is determined */
	loading: boolean;
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined);

// Module-level flag to track if auth has been initialized
// This persists across component remounts during navigation
let authInitialized = false;
let cachedUser: User | null = null;

interface AuthProviderProps {
	children: ReactNode;
}

/**
 * Provides Firebase authentication state to the component tree.
 *
 * This is a simplified replacement for the 539-line singleton AuthStateManager.
 * Uses React's context pattern instead of global singletons for better
 * SSR compatibility and testability.
 *
 * @example
 * ```tsx
 * // In app/providers.tsx
 * <AuthProvider>
 *   {children}
 * </AuthProvider>
 *
 * // In any component
 * const { user, loading } = useAuth();
 * ```
 */
export function AuthProvider({ children }: AuthProviderProps) {
	// Initialize with cached values if already initialized
	const [user, setUser] = useState<User | null>(cachedUser);
	const [loading, setLoading] = useState(!authInitialized);

	useEffect(() => {
		// Single listener for the entire app
		const unsubscribe = onAuthStateChanged(auth, (firebaseUser) => {
			setUser(firebaseUser);
			setLoading(false);
			// Cache for future remounts
			authInitialized = true;
			cachedUser = firebaseUser;
		});

		return () => unsubscribe();
	}, []);

	return (
		<AuthContext.Provider value={{ user, loading }}>
			{children}
		</AuthContext.Provider>
	);
}

/**
 * Hook to access Firebase authentication state.
 *
 * @returns Object containing user and loading state
 * @throws Error if used outside of AuthProvider
 *
 * @example
 * ```tsx
 * function MyComponent() {
 *   const { user, loading } = useAuth();
 *
 *   if (loading) return <Spinner />;
 *   if (!user) return <LoginPrompt />;
 *
 *   return <Dashboard user={user} />;
 * }
 * ```
 */
export function useAuth(): AuthContextValue {
	const context = useContext(AuthContext);

	if (context === undefined) {
		throw new Error("useAuth must be used within an AuthProvider");
	}

	return context;
}

/**
 * Get the current Firebase user (convenience wrapper).
 * Returns null if not authenticated.
 */
export function useCurrentUser(): User | null {
	return useAuth().user;
}

/**
 * Check if auth state has been initialized.
 * Useful for showing loading states on initial render.
 */
export function useAuthReady(): boolean {
	return !useAuth().loading;
}
