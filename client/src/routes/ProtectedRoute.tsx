import { useGetCurrentUserProfileQuery } from "@/features/profiles/profilesSlice";
import { useCurrentAuthUser } from "@/hooks/useCurrentAuthUser";
import useIsUserLoggedIn from "@/hooks/useIsUserLoggedIn";
import { auth } from "@/lib/firebase";
import useAuthSubscribtion from "@/lib/firebase/useAuthSubscribtion";
import useSignOut from "@/lib/firebase/useSignOut";
import { useCallback, useEffect, useState } from "react";
import { Navigate, useNavigate } from "react-router-dom";

// ✅ Protected Route Component
const ProtectedRoute = ({ children }: { children: React.ReactNode }) => {
	const [signOut] = useSignOut(auth);
	const navigate = useNavigate();

	useEffect(() => {
		console.log("protected route mounting...");
	}, []);

	const [isLoggedIn, loginLoading, loginError, authUser, appUser] =
		useIsUserLoggedIn();

	// Handle login errors (e.g., token expired)
	useEffect(() => {
		if (loginError) {
			if (loginLoading) return;
			console.log("ProtectedRoute: Error during auth", loginError);
		}
	}, [loginError]);

	const signUserOut = useCallback(async () => {
		if (!appUser) {
			if (loginLoading) return;
			signOut();
			navigate("/login");
		}
	}, [appUser, loginLoading, navigate, signOut]);

	useEffect(() => {
		signUserOut();
	}, [signUserOut]);

	// Handle valid auth but unverified email or missing onboarding
	useEffect(() => {
		if (authUser && appUser) {
			if (!authUser.emailVerified) {
				signOut();
				console.log("ProtectedRoute: email not verified");
				navigate("/login");
			}

			const isOnboarded = true; // <- change if needed

			if (!isOnboarded) {
				console.log("ProtectedRoute: user needs onboarding");
				navigate("/onboarding");
			}
		}
	}, [authUser, appUser, signOut, navigate]);

	// ⏳ Still verifying auth state
	if (loginLoading) {
		return (
			<div className="min-h-screen w-full flex flex-col justify-center items-center bg-gradient-to-b from-gray-50 to-gray-100 dark:from-gray-900 dark:to-gray-800 transition-colors duration-200">
				<div className="animate-spin rounded-full h-16 w-16 border-4 border-gray-200 dark:border-gray-700 border-t-blue-600 dark:border-t-blue-400 shadow-lg"></div>
			</div>
		);
	}

	// ✅ All good, show protected children
	return children;
};

export default ProtectedRoute;
