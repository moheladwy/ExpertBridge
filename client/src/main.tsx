import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { createBrowserRouter, RouterProvider } from 'react-router-dom'
import './index.css'
import App from './App.tsx'
import NotFoundError from './pages/notFoundPage/NotFoundError.tsx'
import LandingPage from './pages/landingPage/LandingPage.tsx'


const router = createBrowserRouter([
  {path: '/', element: <App/>, children: [
    {index: true, element: <LandingPage/>}
  ]},

  {path: '*', element: <NotFoundError/>}
]);

createRoot(document.getElementById('root')!).render(
  <StrictMode>
        <RouterProvider router={router}/>
  </StrictMode>,
)
