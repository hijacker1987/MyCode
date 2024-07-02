import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import Cookies from "js-cookie";

import { postStatApi } from "../Services/Api";
import { useUser } from "../Services/UserContext";
import { ErrorPage, Notify } from "../Pages/Services";
import { homePage } from "../Services/Frontend.Endpoints";
import { userLogin } from "../Services/Backend.Endpoints";
import Login from "../Components/Login/index";
import Loading from "../Components/Loading/index";

const UserLogin = () => {
    const navigate = useNavigate();
    const { setUserData } = useUser();
    const [loading, setLoading] = useState(false);
    const [errorMessage, setError] = useState("");

    const handleOnLogin = async (userData) => {
        setLoading(true);
        try {
            const response = await postStatApi(userLogin, userData);

            const userId = Cookies.get("UI");
            const userName = Cookies.get("UD");
            const userRole = Cookies.get("UR");

            if ((userId != "" || userId != undefined) && response === 200) {
                setUserData(userRole, userId, userName);

                Cookies.remove("UI");
                Cookies.remove("UD");
                Cookies.remove("UR");

                Notify("Success", "Successful Login!");
                navigate(homePage);
            } else {
                Notify("Error", "Invalid username or password. Please try again.");
            }
        } catch (error) {
            setError(`Error occurred during login: ${error}`);
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
        {errorMessage == "" ? (<Login onLogin={handleOnLogin} onCancel={handleCancel} />
        ) : (
            <ErrorPage errorMessage={errorMessage} />
        )}
    </div>
};

export default UserLogin;
