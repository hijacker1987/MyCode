import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { postApi } from "../Services/Api";
import { getToken } from "../Services/AuthService";
import { userLogin } from "../Services/Backend.Endpoints";
import { jwtDecode } from "jwt-decode";
import Login from "../Components/Login/Login";
import Loading from "../Components/Loading/Loading";
import Notify from "./Services/ToastNotifications";
import ErrorPage from "./Services/ErrorPage";
import Cookies from "js-cookie";

const UserLogin = () => {
    const navigate = useNavigate();
    const [loading, setLoading] = useState(false);
    const [errorMessage, setLoginError] = useState("");

    const handleOnLogin = (user) => {
        setLoading(true);
        postApi(user, userLogin)
            .then((data) => {
                setLoading(false);
                if (data.token) {
                    const decodedToken = jwtDecode(data.token);
                    const expirationTime = decodedToken.exp * 1000;

                    Cookies.set("jwtToken", data.token, { expires: new Date(expirationTime) });

                    navigate("/");
                    Notify("Success", "Successful Login!");
                } else {
                    Notify("Error", "Unable to Login!");
                }
            })
            .catch((error) => {
                setLoading(false);
                setLoginError(`Error occurred during login: ${error}`);
            });
    };

    const checkTokenExpiration = () => {
        const token = getToken();
        if (token) {
            const decodedToken = jwtDecode(token);
            const expirationTime = decodedToken.exp * 1000;

            if (expirationTime < Date.now()) {
                handleLogout();
            }
        }
    };

    const handleLogout = () => {
        Cookies.remove("jwtToken");
        navigate("/");
    };

    const handleCancel = () => {
        navigate("/");
    };

    useEffect(() => {
        const tokenCheckInterval = setInterval(() => {
            checkTokenExpiration();
        }, 60000);

        return () => {
            clearInterval(tokenCheckInterval);
        };
    }, []);

    if (loading) {
        return <Loading />;
    }

    return <div>
                {errorMessage == "" ? ( <Login onLogin={handleOnLogin} onCancel={handleCancel} />
                                  ) : (
                                        <ErrorPage errorMessage={errorMessage} />
                                  )}
           </div>
};

export default UserLogin;
