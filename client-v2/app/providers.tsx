"use client";

import type { ReactNode } from "react";
import { useEffect, useMemo } from "react";
import { Provider } from "react-redux";
import { setupListeners } from "@reduxjs/toolkit/query";
import { ThemeProvider } from "next-themes";
import { Toaster } from "react-hot-toast";

import type { AppStore } from "@/lib/redux/store";
import { makeStore } from "@/lib/redux/store";
import { AuthProvider } from "@/lib/firebase/AuthProvider";
import { AuthPromptProvider } from "@/lib/contexts/AuthPromptContext";
import { AppLoadingProvider } from "@/components/shared/AppLoadingProvider";
import { AppInitializer } from "@/components/shared/AppInitializer";

interface ProvidersProps {
	readonly children: ReactNode;
}

/**
 * Unified providers wrapper for the application.
 *
 * Combines:
 * - Redux Provider (with RTK Query)
 * - Firebase AuthProvider
 * - Theme Provider (dark/light mode)
 * - Toast notifications
 *
 * Order matters:
 * 1. Redux - Outermost for global state
 * 2. Auth - After Redux so hooks can access store
 * 3. Theme - UI concern, can be anywhere
 * 4. Toaster - Renders toast container
 */
export function Providers({ children }: ProvidersProps) {
	// Create store once per component instance using useMemo
	// This is safe for SSR as each request gets a new store
	const store = useMemo<AppStore>(() => makeStore(), []);

	useEffect(() => {
		// Configure listeners for refetchOnFocus/refetchOnReconnect behaviors
		const unsubscribe = setupListeners(store.dispatch);
		return unsubscribe;
	}, [store]);

	return (
		<Provider store={store}>
			<AuthProvider>
				<AuthPromptProvider>
					<ThemeProvider
						attribute="class"
						defaultTheme="system"
						enableSystem
						disableTransitionOnChange
					>
						<AppLoadingProvider>
							<AppInitializer>{children}</AppInitializer>
						</AppLoadingProvider>
						<Toaster
							position="top-right"
							toastOptions={{
								duration: 4000,
								style: {
									background: "hsl(var(--background))",
									color: "hsl(var(--foreground))",
									border: "1px solid hsl(var(--border))",
								},
							}}
						/>
					</ThemeProvider>
				</AuthPromptProvider>
			</AuthProvider>
		</Provider>
	);
}
