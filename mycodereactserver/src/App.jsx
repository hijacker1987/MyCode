import React, { useEffect, useState } from "react";
import { createBrowserRouter, RouterProvider, Navigate } from "react-router-dom";
import { ToastContainer } from "react-toastify";
import { UserProvider } from "./Services/UserContext";
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
import ErrorPage from "./Pages/Services/ErrorPage";
import Homepage from "./Components/Homepage";

function App() {
    const [isAuthenticated, setIsAuthenticated] = useState(false);

    useEffect(() => {
        setIsAuthenticated(true);

        return () => {
            setIsAuthenticated(false);
        };
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
                    element: isAuthenticated ? <PwChange /> : <Navigate to={homePage} />
                },
                {
                    path: uUpdateOwn,
                    element: isAuthenticated ? <UserUpdate /> : <Navigate to={homePage} />
                },
                {
                    path: cReg,
                    element: isAuthenticated ? <CodeRegister /> : <Navigate to={homePage} />
                },
                {
                    path: `${cOwn}:page`,
                    element: isAuthenticated ? <CodesList type="byAuth" /> : <Navigate to={homePage} />
                },
                {
                    path: `${cOthers}:page`,
                    element: isAuthenticated ? <CodesList type="byVis" /> : <Navigate to={homePage} />
                },
                {
                    path: cUpdateOwn,
                    element: isAuthenticated ? <UserUpdate /> : <Navigate to={homePage} />
                },
                {
                    path: `${uUpdate}:userIdParam`,
                    element: isAuthenticated ? <UserUpdate /> : <Navigate to={homePage} />
                },
                {
                    path: `${cUpdate}:codeId`,
                    element: isAuthenticated ? <CodeUpdate /> : <Navigate to={homePage} />
                },
                {
                    path: `${uList}:page`,
                    element: isAuthenticated ? <UsersList /> : <Navigate to={homePage} />
                },
                {
                    path: `${cList}:page`,
                    element: isAuthenticated ? <CodesList type="byAuth" /> : <Navigate to={homePage} />
                },
                {
                    path: errorOccured,
                    element: <ErrorPage />
                },
            ],
        },
    ]);

    return (
        <UserProvider>
            <RouterProvider router={router} />
            <ToastContainer />
        </UserProvider>
    );
}

reportWebVitals();

export default App;
