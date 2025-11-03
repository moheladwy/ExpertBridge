import React, { lazy, Suspense } from "react";
import { createBrowserRouter } from "react-router-dom";
import App from "./App.tsx";
import ProtectedRoute from "./routes/ProtectedRoute.tsx";
import PageLoader from "./components/loaders/PageLoader.tsx";
import ErrorBoundary from "./components/errors/ErrorBoundary.tsx";
import { lazyWithRetry } from "./utils/lazyWithRetry.ts";

// Auth pages - loaded together as "auth" chunk
const LoginPage = lazyWithRetry(
  () => import("./views/pages/auth/LoginPage.tsx"),
  "LoginPage",
);
const SignUpPage = lazyWithRetry(
  () => import("./views/pages/auth/SignUpPage.tsx"),
  "SignUpPage",
);
const EmailVerificationPage = lazyWithRetry(
  () => import("./views/pages/auth/EmailVerificationPage.tsx"),
  "EmailVerificationPage",
);

// Landing pages - loaded on demand
const LandingPage = lazyWithRetry(
  () => import("./views/pages/landing/LandingPage.tsx"),
  "LandingPage",
);
const PrivacyPolicy = lazyWithRetry(
  () => import("./views/pages/landing/PrivacyPolicy.tsx"),
  "PrivacyPolicy",
);
const AboutUsPage = lazyWithRetry(
  () => import("./views/pages/landing/AboutUsePage.tsx"),
  "AboutUsPage",
);

// Core feed pages - high priority
const HomePage = lazyWithRetry(
  () => import("./views/pages/feed/HomePage.tsx"),
  "HomePage",
);

// Post pages
const PostFromFeedPage = lazyWithRetry(
  () => import("./views/pages/posts/PostFromFeedPage.tsx"),
  "PostFromFeedPage",
);
const PostFromUrlPage = lazyWithRetry(
  () => import("./views/pages/posts/PostFromUrlPage.tsx"),
  "PostFromUrlPage",
);

// Job posting pages
const JobPostingsFeed = lazyWithRetry(
  () => import("./views/components/common/jobPostings/JobPostingsFeed.tsx"),
  "JobPostingsFeed",
);
const JobPostingFromFeedPage = lazyWithRetry(
  () => import("./views/pages/jobPostings/JobPostingFromFeedPage.tsx"),
  "JobPostingFromFeedPage",
);
const JobPostingFromUrlPage = lazyWithRetry(
  () => import("./views/pages/jobPostings/JobPostingFromUrlPage.tsx"),
  "JobPostingFromUrlPage",
);
const JobApplicationsPage = lazyWithRetry(
  () => import("./views/pages/jobPostings/JobApplicationsPage.tsx"),
  "JobApplicationsPage",
);

// Job management pages
const JobOffersDashboardPage = lazyWithRetry(
  () => import("./views/pages/jobs/JobOffersDashboardPage.tsx"),
  "JobOffersDashboardPage",
);
const MyJobsPage = lazyWithRetry(
  () => import("./views/pages/jobs/MyJobsPage.tsx"),
  "MyJobsPage",
);
const JobDetailsPage = lazyWithRetry(
  () => import("./views/pages/jobs/JobDetailsPage.tsx"),
  "JobDetailsPage",
);

// Profile pages
const MyProfilePage = lazyWithRetry(
  () => import("./views/pages/profile/MyProfilePage.tsx"),
  "MyProfilePage",
);
const UserProfilePage = lazyWithRetry(
  () => import("./views/pages/profile/UserProfilePage.tsx"),
  "UserProfilePage",
);

// Search pages
const SearchPosts = lazyWithRetry(
  () => import("./views/pages/search/SearchPosts.tsx"),
  "SearchPosts",
);
const SearchUsers = lazyWithRetry(
  () => import("./views/pages/search/SearchUsers.tsx"),
  "SearchUsers",
);
const SearchJobPosts = lazyWithRetry(
  () => import("./views/pages/search/SearchJobPosts.tsx"),
  "SearchJobPosts",
);

// Other pages
const Notifications = lazyWithRetry(
  () => import("./views/pages/notifications/Notifications.tsx"),
  "Notifications",
);
const Interests = lazyWithRetry(
  () => import("./views/pages/onboarding/Interests.tsx"),
  "Interests",
);
const NotFoundError = lazyWithRetry(
  () => import("./views/components/common/ui/NotFoundError.tsx"),
  "NotFoundError",
);

// Wrapper component for lazy loaded routes
const LazyRoute: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  return (
    <ErrorBoundary>
      <Suspense fallback={<PageLoader />}>{children}</Suspense>
    </ErrorBoundary>
  );
};

export const router = createBrowserRouter([
  {
    path: "/",
    element: <App />,
    children: [
      {
        index: true,
        element: (
          <LazyRoute>
            <LandingPage />
          </LazyRoute>
        ),
      },
      {
        path: "home",
        element: (
          <LazyRoute>
            <HomePage />
          </LazyRoute>
        ),
      },
      {
        path: "posts/:postId",
        element: (
          <LazyRoute>
            <PostFromUrlPage />
          </LazyRoute>
        ),
      },
      {
        path: "feed/:postId",
        element: (
          <LazyRoute>
            <PostFromFeedPage />
          </LazyRoute>
        ),
      },
      {
        path: "jobs",
        element: (
          <LazyRoute>
            <JobPostingsFeed />
          </LazyRoute>
        ),
      },
      {
        path: "jobs/:jobPostingId",
        element: (
          <LazyRoute>
            <JobPostingFromFeedPage />
          </LazyRoute>
        ),
      },
      {
        path: "job/:jobPostingId",
        element: (
          <LazyRoute>
            <JobPostingFromUrlPage />
          </LazyRoute>
        ),
      },
      {
        path: "job/:jobPostingId/applications",
        element: (
          <LazyRoute>
            <JobApplicationsPage />
          </LazyRoute>
        ),
      },
      {
        path: "profile",
        element: (
          <ProtectedRoute>
            <LazyRoute>
              <MyProfilePage />
            </LazyRoute>
          </ProtectedRoute>
        ),
      },
      {
        path: "profile/:userId",
        element: (
          <LazyRoute>
            <UserProfilePage />
          </LazyRoute>
        ),
      },
      {
        path: "search/p",
        element: (
          <LazyRoute>
            <SearchPosts />
          </LazyRoute>
        ),
      },
      {
        path: "search/u",
        element: (
          <LazyRoute>
            <SearchUsers />
          </LazyRoute>
        ),
      },
      {
        path: "search/jobs",
        element: (
          <LazyRoute>
            <SearchJobPosts />
          </LazyRoute>
        ),
      },
      {
        path: "notifications",
        element: (
          <ProtectedRoute>
            <LazyRoute>
              <Notifications />
            </LazyRoute>
          </ProtectedRoute>
        ),
      },
      {
        path: "offers",
        element: (
          <ProtectedRoute>
            <LazyRoute>
              <JobOffersDashboardPage />
            </LazyRoute>
          </ProtectedRoute>
        ),
      },
      {
        path: "my-jobs",
        element: (
          <ProtectedRoute>
            <LazyRoute>
              <MyJobsPage />
            </LazyRoute>
          </ProtectedRoute>
        ),
      },
      {
        path: "my-jobs/:jobId",
        element: (
          <ProtectedRoute>
            <LazyRoute>
              <JobDetailsPage />
            </LazyRoute>
          </ProtectedRoute>
        ),
      },
      {
        path: "privacy-policy",
        element: (
          <LazyRoute>
            <PrivacyPolicy />
          </LazyRoute>
        ),
      },
      {
        path: "AboutUs",
        element: (
          <LazyRoute>
            <AboutUsPage />
          </LazyRoute>
        ),
      },
    ],
  },
  {
    path: "login",
    element: (
      <LazyRoute>
        <LoginPage />
      </LazyRoute>
    ),
  },
  {
    path: "signup",
    element: (
      <LazyRoute>
        <SignUpPage />
      </LazyRoute>
    ),
  },
  {
    path: "verify-email",
    element: (
      <LazyRoute>
        <EmailVerificationPage />
      </LazyRoute>
    ),
  },
  {
    path: "onboarding",
    element: (
      <ProtectedRoute>
        <LazyRoute>
          <Interests />
        </LazyRoute>
      </ProtectedRoute>
    ),
  },
  {
    path: "*",
    element: (
      <LazyRoute>
        <NotFoundError />
      </LazyRoute>
    ),
  },
]);
