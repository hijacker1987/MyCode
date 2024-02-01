import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { postApiV2 } from "../Services/Api";
import { homePage } from "../Services/Frontend.Endpoints";
import { userLogin } from "../Services/Backend.Endpoints";
import { jwtDecode } from "jwt-decode";
import Login from "../Components/Login/Login";
import Loading from "../Components/Loading/Loading";
import Notify from "../Pages/Services/ToastNotifications";
import ErrorPage from "./Services/ErrorPage";
import Cookies from "js-cookie";

const UserLogin = () => {
    const navigate = useNavigate();
    const [loading, setLoading] = useState(false);
    const [errorMessage, setLoginError] = useState("");

    const handleOnLogin = async (user) => {
        setLoading(true);
        try {
            const data = await postApiV2(user, userLogin);

            if (data.token) {
                const decodedToken = jwtDecode(data.token);
                const expirationTime = decodedToken.exp * 1000;

                Cookies.set("jwtToken", data.token, { expires: new Date(expirationTime) });

                Notify("Success", "Successful Login!");
                navigate(homePage);
            } else {
                Notify("Error", "Unable to Login!");
            }
        } catch (error) {
            setLoginError(`Error occurred during login: ${error}`);
        } finally {
            setLoading(false);
        }
    };

    const handleCancel = () => {
        navigate(-1);
    };

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
