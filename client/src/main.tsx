import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { createBrowserRouter, RouterProvider } from 'react-router-dom'
import './index.css'
import NotFoundError from './views/components/common/ui/NotFoundError.tsx'
import LandingPage from './views/pages/landing/LandingPage.tsx'
import { store } from './app/store.ts'

import { Provider as ReduxProvider } from "react-redux";
import SignUpPage from './views/pages/auth/SignUpPage.tsx'
import LoginPage from './views/pages/auth/LoginPage.tsx'
import App from './App.tsx'
import ProtectedRoute from './routes/ProtectedRoute.tsx'
import PublicRoute from './routes/PublicRoute.tsx'
import HomePage from './views/pages/feed/HomePage.tsx'
import Interests from './views/pages/auth/Interests.tsx'

const router = createBrowserRouter([
  {
    path: "/",
    element: <App />, // Ensures NavBar is always present
    children: [
      {
        index: true,
        element: <LandingPage />
      },
      {
        path: "home",
        element: (
          <ProtectedRoute>
            <HomePage />
          </ProtectedRoute>
        ),
      },
    ],
  },
  {
    path: "login",
    element: (
      <LoginPage />
    ),
  },
  {
    path: "signup",
    element: (
      <SignUpPage />
    ),
  },
  {
    path: "interests",
    element: (
      // <ProtectedRoute>
        <Interests/>
      // </ProtectedRoute>
    ),
  },
  { path: "*", element: <NotFoundError /> }, // Catch-all 404
]);

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <ReduxProvider store={store} >
      <RouterProvider router={router} />
    </ReduxProvider>
  </StrictMode>,
)
