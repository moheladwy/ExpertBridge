import { Outlet } from "react-router";
import NavBar from "@/views/components/common/ui/NavBar";
import { Toaster } from "react-hot-toast";
import { UpdateUserRequest } from "./features/users/types";
import { useUpdateUserMutation } from "./features/users/usersSlice";
import { useEffect, useState } from "react";
import { tokenManager } from "@/lib/services/TokenManager";
import { useLocation } from "react-router-dom";
import AuthPromptModal from "@/views/components/common/ui/AuthPromptModal";
import { useCurrentUser } from "@/lib/services/AuthStateManager";
import {
	AuthPromptProvider,
	useAuthPrompt,
} from "@/contexts/AuthPromptContext";
import { ThemeProvider } from "@/components/theme-provider";
import TokenMonitor from "@/views/components/common/ui/TokenMonitor";
import AuthStateMonitor from "@/views/components/common/ui/AuthStateMonitor";
import ErrorBoundary from "@/components/errors/ErrorBoundary";

function AppContent() {
	const [updateUser] = useUpdateUserMutation();
	const authUser = useCurrentUser(); // Now using centralized auth - no duplicate listener!

	const [showInitialAuthPrompt, setShowInitialAuthPrompt] = useState(false);
	const location = useLocation();
	const isLandingPage = location.pathname === "/";

	const { isAuthPromptOpen, hideAuthPrompt } = useAuthPrompt();

	useEffect(() => {
		let timer: NodeJS.Timeout | undefined;
		if (!authUser && !isLandingPage) {
			console.log("setting timeout for auth prompt");
			timer = setTimeout(() => {
				console.log(
					"timeout finished setting showInitialAuthPrompt true"
				);
				if (!isAuthPromptOpen) {
					setShowInitialAuthPrompt(true);
				}
			}, 10000);
		} else {
			setShowInitialAuthPrompt(false);
		}

		return () => {
			if (timer) {
				console.log("clearing timeout for auth prompt");
				clearTimeout(timer);
			}
		};
	}, [authUser, isLandingPage, isAuthPromptOpen]);

	useEffect(() => {
		// Token manager handles auth state internally
		// This ensures we have a fresh token ready for API calls
		tokenManager.ensureFreshToken().catch(console.error);

		// Update user profile when auth state changes
		const updateUserProfile = async () => {
			if (!authUser) return;

			console.log("Updating user profile...");
			const token = await tokenManager.getToken();
			const name = authUser.displayName?.split(" ") || [];
			const request: UpdateUserRequest = {
				firstName: name[0],
				lastName: name[1],
				email: authUser.email!,
				phoneNumber: authUser.phoneNumber,
				providerId: authUser.uid,
				profilePictureUrl: authUser.photoURL,
				isEmailVerified: authUser.emailVerified,
				token: token || undefined,
			};

			await updateUser(request);
		};

		// Update profile when auth user changes
		if (authUser) {
			updateUserProfile().catch(console.error);
		}
	}, [authUser, updateUser]);

	return (
		<ThemeProvider defaultTheme="dark" storageKey="vite-ui-theme">
			<div className="fixed top-0 left-0 right-0 z-50">
				<NavBar />
			</div>

			<div className="pt-16 bg-background min-h-screen">
				<Toaster />
				<Outlet /> {/* Renders the current route's element */}
			</div>

			<AuthPromptModal
				open={showInitialAuthPrompt}
				onOpenChange={setShowInitialAuthPrompt}
				title="Tailor Your Experience"
				description="Create an account to unlock all features and get a personalized experience on ExpertBridge."
			/>

			<AuthPromptModal
				open={isAuthPromptOpen}
				onOpenChange={(open) => !open && hideAuthPrompt()}
				title="Authentication Required"
				description="Please log in or sign up to continue with this action."
			/>

			{/* Token Monitor - Development Only */}
			{process.env.NODE_ENV === "development" && <TokenMonitor />}

			{/* Auth State Monitor - Development Only */}
			{process.env.NODE_ENV === "development" && <AuthStateMonitor />}
		</ThemeProvider>
	);
}

function App() {
	return (
		<AuthPromptProvider>
			<AppContent />
		</AuthPromptProvider>
	);
}
export default App;
