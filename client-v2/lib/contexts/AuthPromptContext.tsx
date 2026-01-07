"use client";

import React, {
	createContext,
	useState,
	useContext,
	type ReactNode,
	useCallback,
} from "react";
import { useRouter } from "next/navigation";
import {
	AlertDialog,
	AlertDialogAction,
	AlertDialogCancel,
	AlertDialogContent,
	AlertDialogDescription,
	AlertDialogFooter,
	AlertDialogHeader,
	AlertDialogTitle,
} from "@/components/ui/alert-dialog";

interface AuthPromptContextType {
	/** Whether the auth prompt dialog is currently open */
	isAuthPromptOpen: boolean;
	/** Show the auth prompt dialog */
	showAuthPrompt: () => void;
	/** Hide the auth prompt dialog */
	hideAuthPrompt: () => void;
}

const AuthPromptContext = createContext<AuthPromptContextType | undefined>(
	undefined
);

interface AuthPromptProviderProps {
	children: ReactNode;
}

/**
 * Provides a context for showing authentication prompts throughout the app.
 * When an unauthenticated user tries to perform a protected action,
 * call `showAuthPrompt()` to display a login/signup prompt.
 *
 * @example
 * ```tsx
 * function VoteButton() {
 *   const { showAuthPrompt } = useAuthPrompt();
 *   const { isAuthenticated } = useIsUserLoggedIn();
 *
 *   const handleVote = () => {
 *     if (!isAuthenticated) {
 *       showAuthPrompt();
 *       return;
 *     }
 *     // Proceed with vote...
 *   };
 * }
 * ```
 */
export function AuthPromptProvider({ children }: AuthPromptProviderProps) {
	const [isAuthPromptOpen, setIsAuthPromptOpen] = useState(false);
	const router = useRouter();

	const showAuthPrompt = useCallback(() => {
		setIsAuthPromptOpen(true);
	}, []);

	const hideAuthPrompt = useCallback(() => {
		setIsAuthPromptOpen(false);
	}, []);

	const handleSignIn = useCallback(() => {
		hideAuthPrompt();
		router.push("/auth/signin");
	}, [hideAuthPrompt, router]);

	const handleSignUp = useCallback(() => {
		hideAuthPrompt();
		router.push("/auth/signup");
	}, [hideAuthPrompt, router]);

	return (
		<AuthPromptContext.Provider
			value={{ isAuthPromptOpen, showAuthPrompt, hideAuthPrompt }}
		>
			{children}

			<AlertDialog open={isAuthPromptOpen} onOpenChange={hideAuthPrompt}>
				<AlertDialogContent>
					<AlertDialogHeader>
						<AlertDialogTitle>Sign in to continue</AlertDialogTitle>
						<AlertDialogDescription>
							You need to be signed in to perform this action.
							Create an account or sign in to get started.
						</AlertDialogDescription>
					</AlertDialogHeader>
					<AlertDialogFooter className="flex-col sm:flex-row gap-2">
						<AlertDialogCancel className="rounded-full">
							Cancel
						</AlertDialogCancel>
						<AlertDialogAction
							onClick={handleSignIn}
							className="rounded-full bg-primary hover:bg-primary/90"
						>
							Sign In
						</AlertDialogAction>
						<AlertDialogAction
							onClick={handleSignUp}
							className="rounded-full bg-secondary text-secondary-foreground hover:bg-secondary/80"
						>
							Sign Up
						</AlertDialogAction>
					</AlertDialogFooter>
				</AlertDialogContent>
			</AlertDialog>
		</AuthPromptContext.Provider>
	);
}

/**
 * Hook to access the auth prompt context.
 * Must be used within an AuthPromptProvider.
 *
 * @throws Error if used outside of AuthPromptProvider
 */
export function useAuthPrompt(): AuthPromptContextType {
	const context = useContext(AuthPromptContext);
	if (context === undefined) {
		throw new Error(
			"useAuthPrompt must be used within an AuthPromptProvider"
		);
	}
	return context;
}
