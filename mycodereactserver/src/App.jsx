import React, { useEffect, useState } from "react";
import { createBrowserRouter, RouterProvider, Navigate } from "react-router-dom";
import reportWebVitals from "./reportWebVitals";
import "./App.css";

import { uLogin, errorOccured,
         uReg, uUpdateOwn, uUpdate, uPwChange,
         cReg, cUpdateOwn, cUpdate, cOwn,
         uList, cList
       } from "../src/Services/Frontend.Endpoints";

import Layout from "./Pages/Layout/Layout";
import UserLogin from "./Pages/Login";
import UserRegister from "./Pages/Register";
import UserUpdate from "./Pages/UserUpdate";
import PwChange from "./Pages/PasswordChange";
import CodeRegister from "./Pages/CodeRegister";
import CodeUpdate from "./Pages/CodeUpdate";
import UsersList from "./Pages/Lists/UsersList";
import CodesList from "./Pages/Lists/CodesList";
import ErrorPage from "./Pages/Service/ErrorPage";

function App() {
    const [isAuthenticated, setIsAuthenticated] = useState(false);
    const [userRole, setUserRole] = useState([]);

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
                    path: uUpdateOwn,
                    element: isAuthenticated ? <UserUpdate /> : <Navigate to={uLogin} />
                },
                {
                    path: cReg,
                    element: isAuthenticated ? <CodeRegister /> : <Navigate to={uLogin} />
                },
                {
                    path: cOwn,
                    element: isAuthenticated ? <CodesList /> : <Navigate to={uLogin} />
                },
                {
                    path: cUpdateOwn,
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
                    path: uList,
                    element: isAuthenticated ? <UsersList /> : <Navigate to={uLogin} />
                },
                {
                    path: cList,
                    element: isAuthenticated ? <CodesList /> : <Navigate to={uLogin} />
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
