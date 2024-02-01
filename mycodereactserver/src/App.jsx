import React, { useEffect, useState } from "react";
import { createBrowserRouter, RouterProvider, Navigate } from "react-router-dom";
import { ToastContainer } from "react-toastify";
import reportWebVitals from "./reportWebVitals";
import "react-toastify/dist/ReactToastify.css";
import "./App.css";

import { uLogin, errorOccured,
         uReg, uUpdateOwn, uUpdate, uPwChange,
         cReg, cUpdateOwn, cUpdate, cOwn, cOthers,
         uList, cList, homePage
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
import ErrorBoundary from "./Services/ErrorBoundary";
import ErrorPage from "./Pages/Services/ErrorPage";
import Homepage from "./Components/Homepage";

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
                    path: homePage,
                    element: <Homepage />,
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
                    path: `${cOwn}:page`,
                    element: isAuthenticated ? <CodesList type="byAuth" /> : <Navigate to={uLogin} />
                },
                {
                    path: `${cOthers}:page`,
                    element: isAuthenticated ? <CodesList type="byVis" /> : <Navigate to={uLogin} />
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
                    path: `${uList}:page`,
                    element: isAuthenticated ? <UsersList /> : <Navigate to={uLogin} />
                },
                {
                    path: `${cList}:page`,
                    element: isAuthenticated ? <CodesList type="byAuth" /> : <Navigate to={uLogin} />
                },
                {
                    path: errorOccured,
                    element: <ErrorPage />
                },
            ],
        },
    ]);

    return (
        <ErrorBoundary>
            <RouterProvider router={router} />
            <ToastContainer />
        </ErrorBoundary>
    );
}

reportWebVitals();

export default App;
