import React, { useEffect, useState } from 'react';
import { createBrowserRouter, RouterProvider } from "react-router-dom";
import reportWebVitals from "./reportWebVitals";
import './App.css';

import { uReg, uLogin, uPwChange } from "../src/Services/Frontend.Endpoints";

import Layout from './Pages/Layout/Layout';
import UserRegister from './Pages/Register';
import Login from './Pages/Login';
import PwChange from "./Pages/PasswordChange";

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
                    element: isAuthenticated ? <Login /> : <div className="welcome-text">You are already logged in!</div>
                },
                {
                    path: uPwChange,
                    element: <PwChange />
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
