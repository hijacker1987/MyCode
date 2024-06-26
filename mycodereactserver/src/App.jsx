import React, { useEffect, useState } from "react";
import { createBrowserRouter, RouterProvider, Navigate } from "react-router-dom";
import { ToastContainer } from "react-toastify";

import { ErrorPage, PriPolPage } from "./Pages/Services";
import reportWebVitals from "./reportWebVitals";
import { UserProvider } from "./Services/UserContext";
import { uLogin, uReg, uUpdateOwn, uUpdate, uPwChange, u2fa,
         cReg, cUpdateOwn, cUpdate, cOwn, cOthers, uList, cList,
         errorOccured, homePage, accDel, priPol
        } from "./Services/Frontend.Endpoints";
import { UsersList, CodesList } from "./Pages/Lists/index";
import Layout from "./Pages/Layout/index";
import Homepage from "./Components/Homepage";
import UserLogin from "./Pages/Login";
import UserRegister from "./Pages/UserRegister";
import UserUpdate from "./Pages/UserUpdate";
import TwoFactorAuthentication from "./Pages/TwoFa";
import PwChange from "./Pages/PasswordChange";
import CodeRegister from "./Pages/CodeRegister";
import CodeUpdate from "./Pages/CodeUpdate";
import AccDelPage from "./Pages/Services/AccDel";
import ChuckBot from "./Pages/Services/ChuckBot";

import GlobalStyles from "./Components/Styles/Global.styled";
import "react-toastify/dist/ReactToastify.css";

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
                    path: `${u2fa}:userIdParam`,
                    element: isAuthenticated ? <TwoFactorAuthentication /> : <Navigate to={homePage} />
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
        {                             
            path: priPol,
            element: <PriPolPage />
        },
        {
            path: accDel,
            element: <AccDelPage />
        },
    ]);

    return (
        <UserProvider>
            <GlobalStyles />
            <RouterProvider router={router} />
            <ToastContainer />
            <ChuckBot />
        </UserProvider>
    );
}

reportWebVitals();

export default App;
