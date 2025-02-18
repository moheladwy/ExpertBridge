import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { createBrowserRouter, RouterProvider } from 'react-router-dom'
import './index.css'
import App from './App.tsx'
import NotFoundError from './pages/notFoundPage/NotFoundError.tsx'
import { store } from './app/store.ts'

import { Provider } from "react-redux";

const router = createBrowserRouter([
  {
    path: '/', element: <App />, children: [

    ]
  },

  { path: '*', element: <NotFoundError /> }
]);

createRoot(document.getElementById('root')!).render(
  <StrictMode>
    <Provider store={store} >
      <RouterProvider router={router} />
    </Provider>
  </StrictMode>,
)
