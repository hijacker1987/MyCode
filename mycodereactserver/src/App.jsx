import React, { useEffect, useState } from "react";
import { createBrowserRouter, RouterProvider, Navigate } from "react-router-dom";
import reportWebVitals from "./reportWebVitals";
import "./App.css";

import { uReg, uLogin, uPwChange, uList, cList, uUpdate, uUpdateOwn, cUpdate, errorOccured } from "../src/Services/Frontend.Endpoints";

import Layout from "./Pages/Layout/Layout";
import UserRegister from "./Pages/Register";
import UserLogin from "./Pages/Login";
import PwChange from "./Pages/PasswordChange";
import UsersList from "./Pages/Lists/UsersList";
import CodesList from "./Pages/Lists/CodesList";
import UserUpdate from "./Pages/UserUpdate";
import CodeUpdate from "./Pages/CodeUpdate";
import ErrorPage from "./Pages/Service/ErrorPage";

function App() {
    const [isAuthenticated, setIsAuthenticated] = useState(false);

    useEffect(() => {
        setIsAuthenticated(prevState => !prevState);
    }, []);

    const router = createBrowserRouter([
        {
            path: "/",
            element: <Layout />,
            children: [
                {
                    path: "/",
                    element: <div className="welcome-text">Welcome Code Fanatic!</div>,
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
                {
                    path: cList,
                    element: isAuthenticated ? <CodesList /> : <Navigate to={uLogin} />
                },
                {
                    path: `${uUpdateOwn}:userId`,
                    element: isAuthenticated ? <UserUpdate /> : <Navigate to={uLogin} />
                },
                {
                    path: `${uUpdate}:userId`,
                    element: isAuthenticated ? <UserUpdate /> : <Navigate to={uLogin} />
                },
                {
                    path: `${cUpdate}:codeId`,
                    element: isAuthenticated ? <CodeUpdate /> : <Navigate to={uLogin} />
                },
                {
                    path: errorOccured,
                    element: <ErrorPage />
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
