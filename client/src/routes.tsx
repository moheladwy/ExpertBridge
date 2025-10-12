import { createBrowserRouter } from "react-router-dom";
import SignUpPage from "./views/pages/auth/SignUpPage.tsx";
import LoginPage from "./views/pages/auth/LoginPage.tsx";
import ProtectedRoute from "./routes/ProtectedRoute.tsx";
import HomePage from "./views/pages/feed/HomePage.tsx";
import Interests from "./views/pages/onboarding/Interests.tsx";
import PostFromFeedPage from "./views/pages/posts/PostFromFeedPage.tsx";
import EmailVerificationPage from "./views/pages/auth/EmailVerificationPage.tsx";
import MyProfilePage from "./views/pages/profile/MyProfilePage.tsx";
import PostFromUrlPage from "./views/pages/posts/PostFromUrlPage.tsx";
import UserProfilePage from "./views/pages/profile/UserProfilePage.tsx";
import Notifications from "./views/pages/notifications/Notifications.tsx";
import SearchPosts from "./views/pages/search/SearchPosts.tsx";
import SearchUsers from "./views/pages/search/SearchUsers.tsx";
import JobPostingsFeed from "./views/components/common/jobPostings/JobPostingsFeed.tsx";
import JobPostingFromFeedPage from "./views/pages/jobPostings/JobPostingFromFeedPage.tsx";
import JobPostingFromUrlPage from "./views/pages/jobPostings/JobPostingFromUrlPage.tsx";
import PrivacyPolicy from "./views/pages/landing/PrivacyPolicy.tsx";
import JobApplicationsPage from "./views/pages/jobPostings/JobApplicationsPage.tsx";
import { JobOffersDashboardPage } from "./views/pages/jobs/JobOffersDashboardPage.tsx";
import { MyJobsPage } from "./views/pages/jobs/MyJobsPage.tsx";
import { JobDetailsPage } from "./views/pages/jobs/JobDetailsPage.tsx";
import SearchJobPosts from "./views/pages/search/SearchJobPosts.tsx";
import AboutUsPage from "./views/pages/landing/AboutUsePage.tsx";
import NotFoundError from "./views/components/common/ui/NotFoundError.tsx";
import ApiHealthErrorPage from "./views/pages/error/ApiHealthErrorPage.tsx";
import App from "./App.tsx";
import LandingPage from "./views/pages/landing/LandingPage.tsx";

export const router = createBrowserRouter([
  {
    path: "/",
    element: <App />,
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
        path: "jobs",
        element: <JobPostingsFeed />,
      },
      {
        path: "jobs/:jobPostingId",
        element: <JobPostingFromFeedPage />,
      },
      {
        path: "job/:jobPostingId",
        element: <JobPostingFromUrlPage />,
      },
      {
        path: "job/:jobPostingId/applications",
        element: <JobApplicationsPage />,
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
        path: "search/jobs",
        element: <SearchJobPosts />,
      },
      {
        path: "notifications",
        element: (
          <ProtectedRoute>
            <Notifications />
          </ProtectedRoute>
        ),
      },
      {
        path: "offers",
        element: (
          <ProtectedRoute>
            <JobOffersDashboardPage />
          </ProtectedRoute>
        ),
      },
      {
        path: "my-jobs",
        element: (
          <ProtectedRoute>
            <MyJobsPage />
          </ProtectedRoute>
        ),
      },
      {
        path: "my-jobs/:jobId",
        element: (
          <ProtectedRoute>
            <JobDetailsPage />
          </ProtectedRoute>
        ),
      },
      {
        path: "privacy-policy",
        element: <PrivacyPolicy />,
      },
      {
        path: "AboutUs",
        element: <AboutUsPage />,
      },
      {
        path: "service-unavailable",
        element: <ApiHealthErrorPage healthData={null} />,
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
