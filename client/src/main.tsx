import { StrictMode } from "react";
import { createRoot } from "react-dom/client";
import { createBrowserRouter, RouterProvider } from "react-router-dom";
import "./index.css";
import NotFoundError from "./views/components/common/ui/NotFoundError.tsx";
import LandingPage from "./views/pages/landing/LandingPage.tsx";
import { store } from "./app/store.ts";

import { Provider as ReduxProvider } from "react-redux";
import SignUpPage from "./views/pages/auth/SignUpPage.tsx";
import LoginPage from "./views/pages/auth/LoginPage.tsx";
import App from "./App.tsx";
import ProtectedRoute from "./routes/ProtectedRoute.tsx";
import HomePage from "./views/pages/feed/HomePage.tsx";
import Interests from "./views/pages/onboarding/Interests.tsx";
import PostFromFeedPage from "./views/pages/posts/PostFromFeedPage.tsx";
import EmailVerificationPage from "./views/pages/auth/EmailVerificationPage.tsx";
import MyProfilePage from "./views/pages/profile/MyProfilePage.tsx";

import "@fontsource/roboto/300.css";
import "@fontsource/roboto/400.css";
import "@fontsource/roboto/500.css";
import "@fontsource/roboto/700.css";

import {
	createTheme,
	StyledEngineProvider,
	ThemeProvider,
} from "@mui/material/styles";
import PostFromUrlPage from "./views/pages/posts/PostFromUrlPage.tsx";
import UserProfilePage from "./views/pages/profile/UserProfilePage.tsx";
import Notifications from "./views/pages/notifications/Notifications.tsx";
import SearchPosts from "./views/pages/search/SearchPosts.tsx";
import SearchUsers from "./views/pages/search/SearchUsers.tsx";

const router = createBrowserRouter([
	{
		path: "/",
		element: <App />, // Ensures NavBar is always present
		children: [
			{
				index: true,
				element: <LandingPage />,
			},
			{
				path: "home",
				element: (
					// <ProtectedRoute>
					<HomePage />
					// </ProtectedRoute>
				),
			},
			{
				path: "posts/:postId",
				element: (
					// <ProtectedRoute>
					<PostFromUrlPage />
					// </ProtectedRoute>
				),
			},
			{
				path: "feed/:postId",
				element: (
					// <ProtectedRoute>
					<PostFromFeedPage />
					// </ProtectedRoute>
				),
			},
			{
				path: "profile",
				element: (
					<ProtectedRoute>
						<MyProfilePage />
					</ProtectedRoute>
				),
			},
			{
				path: "profile/:userId",
				element: <UserProfilePage />,
			},
			{
				path: "search/p",
				element: <SearchPosts />,
			},
			{
				path: "search/u",
				element: <SearchUsers />,
			},
			{
				path: "notifications",
				element: (
					<ProtectedRoute>
						<Notifications />
					</ProtectedRoute>
				),
			},
		],
	},
	{
		path: "login",
		element: <LoginPage />,
	},
	{
		path: "signup",
		element: <SignUpPage />,
	},
	{
		path: "verify-email",
		element: <EmailVerificationPage />,
	},
	{
		path: "onboarding",
		element: (
			<ProtectedRoute>
				<Interests />
			</ProtectedRoute>
		),
	},
	{ path: "*", element: <NotFoundError /> }, // Catch-all 404
]);

const rootElement = document.getElementById("root");
const root = createRoot(rootElement!);

const theme = createTheme({
	cssVariables: true,
	components: {
		MuiPopover: {
			defaultProps: {
				container: rootElement,
			},
		},
		MuiPopper: {
			defaultProps: {
				container: rootElement,
			},
		},
	},
});

root.render(
	<StrictMode>
		<StyledEngineProvider injectFirst>
			<ThemeProvider theme={theme}>
				<ReduxProvider store={store}>
					<RouterProvider router={router} />
				</ReduxProvider>
			</ThemeProvider>
		</StyledEngineProvider>
	</StrictMode>
);
