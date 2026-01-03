"use client";

import {
	createContext,
	useContext,
	useState,
	useCallback,
	type ReactNode,
} from "react";
import { LoadingScreen } from "./LoadingScreen";

interface LoadingContextValue {
	/** Whether the loading screen is currently visible */
	isLoading: boolean;
	/** Show the loading screen */
	showLoading: () => void;
	/** Hide the loading screen */
	hideLoading: () => void;
}

const LoadingContext = createContext<LoadingContextValue | undefined>(
	undefined
);

interface AppLoadingProviderProps {
	children: ReactNode;
}

/**
 * Provides programmatic control over the app loading screen.
 *
 * Use this for manual loading control during auth flows:
 * - Show loading before sign in/up
 * - Hide loading after profile is fetched or on error
 *
 * @example
 * ```tsx
 * // In providers.tsx
 * <AppLoadingProvider>
 *   {children}
 * </AppLoadingProvider>
 *
 * // In a component
 * const { showLoading, hideLoading } = useAppLoading();
 * showLoading();
 * await doSomething();
 * hideLoading();
 * ```
 */
export function AppLoadingProvider({ children }: AppLoadingProviderProps) {
	const [isLoading, setIsLoading] = useState(false);

	const showLoading = useCallback(() => setIsLoading(true), []);
	const hideLoading = useCallback(() => setIsLoading(false), []);

	return (
		<LoadingContext.Provider
			value={{ isLoading, showLoading, hideLoading }}
		>
			{isLoading && <LoadingScreen />}
			{children}
		</LoadingContext.Provider>
	);
}

/**
 * Hook to programmatically control the loading screen.
 *
 * @returns Object with isLoading state and show/hide functions
 * @throws Error if used outside of AppLoadingProvider
 *
 * @example
 * ```tsx
 * const { showLoading, hideLoading } = useAppLoading();
 *
 * const handleSubmit = async () => {
 *   showLoading();
 *   try {
 *     await signIn();
 *   } finally {
 *     hideLoading();
 *   }
 * };
 * ```
 */
export function useAppLoading(): LoadingContextValue {
	const context = useContext(LoadingContext);

	if (context === undefined) {
		throw new Error(
			"useAppLoading must be used within an AppLoadingProvider"
		);
	}

	return context;
}
