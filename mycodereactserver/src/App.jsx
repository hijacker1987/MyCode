import React, { useEffect, useState } from 'react';
import { createBrowserRouter, RouterProvider, Navigate } from "react-router-dom";
import reportWebVitals from './reportWebVitals';
import './App.css';

import { uReg, uLogin, uPwChange, uList } from '../src/Services/Frontend.Endpoints';

import Layout from './Pages/Layout/Layout';
import UserRegister from './Pages/Register';
import UserLogin from './Pages/Login';
import PwChange from './Pages/PasswordChange';
import UsersList from './Pages/UsersList';

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
                    path: uReg,
                    element: <UserRegister />,
                },
                {
                    path: uLogin,
                    element: isAuthenticated ? <UserLogin /> : <div className="welcome-text">You are already logged in!</div>
                },
                {
                    path: uPwChange,
                    element: isAuthenticated ? <PwChange /> : <Navigate to={uLogin} />
                },
                {
                    path: uList,
                    element: isAuthenticated ? <UsersList /> : <Navigate to={uLogin} />
                },
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
