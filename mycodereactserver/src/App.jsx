import React, { useEffect, useState } from 'react';
import { createBrowserRouter, RouterProvider } from "react-router-dom";
import reportWebVitals from "./reportWebVitals";
import './App.css';

import Layout from './Pages/Layout/Layout';
import UserRegister from './Pages/Register';
import Login from './Pages/Login';

function App() {
    const [isAuthenticated, setIsAuthenticated] = useState(false);

    useEffect(() => {
        setIsAuthenticated(prevState => !prevState);
    }, []);

    const router = createBrowserRouter([
        {
            path: '/',
            element: <Layout />,
            children: [
                {
                    path: '/',
                    element: <div className="welcome-text">Welcome to the page!</div>,
                },
                {
                    path: '/account/register',
                    element: <UserRegister />,
                },
                {
                    path: '/account/login',
                    element: isAuthenticated ? <Login /> : <div className="welcome-text">You are already logged in!</div>
                }
            ],
        },
    ]);

    return (
        <React.StrictMode>
            <RouterProvider router={router} />
        </React.StrictMode>
    );
}

reportWebVitals();

export default App;
