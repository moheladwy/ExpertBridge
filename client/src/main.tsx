import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { createBrowserRouter, RouterProvider } from 'react-router-dom'
import './index.css'
import NotFoundError from './pages/notFoundPage/NotFoundError.tsx'
import LandingPage from './pages/landingPage/LandingPage.tsx'
import { store } from './app/store.ts'

import { Provider as ReduxProvider } from "react-redux";
import SignUpPage from './pages/SignUpPage.tsx'
import LoginPage from './pages/LoginPage.tsx'
import App from './App.tsx'
import ProtectedRoute from './components/Routes/ProtectedRoute.tsx'
import PublicRoute from './components/Routes/PublicRoute.tsx'
import HomePage from './pages/HomePage.tsx'
import Interests from './pages/Interests.tsx'

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
      <PublicRoute>
        <LoginPage />
      </PublicRoute>
    ),
  },
  {
    path: "signup",
    element: (
      <PublicRoute>
        <SignUpPage />
      </PublicRoute>
    ),
  },
  {
    path: "interests",
    element: (
      <ProtectedRoute>
        <Interests/>
      </ProtectedRoute>
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
