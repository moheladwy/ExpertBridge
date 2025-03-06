import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { createBrowserRouter, RouterProvider } from 'react-router-dom'
import './index.css'
import NotFoundError from './pages/notFoundPage/NotFoundError.tsx'
import LandingPage from './pages/landingPage/LandingPage.tsx'
import { store } from './app/store.ts'

import { Provider as ReduxProvider } from "react-redux";
import SignUpPage from './features/auth/SignUpPage.tsx'
import LoginPage from './features/auth/LoginPage.tsx'
import App from './App.tsx'
import PublicRoute from './components/Routes/PublicRoute.tsx'
import ProtectedRoute from './components/Routes/ProtectedRoute.tsx'

const router = createBrowserRouter([
  {
    path: "/",
    element: <App />, // Ensures NavBar is always present
    children: [
      { index: true, element: <LandingPage /> }, // Redirect '/' to '/home'http://localhost:5173/
      {
        path: "home",
        element: (
          <ProtectedRoute>
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
  { path: "*", element: <NotFoundError /> }, // Catch-all 404
]);

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <ReduxProvider store={store} >
      <RouterProvider router={router} />
    </ReduxProvider>
  </StrictMode>,
)
