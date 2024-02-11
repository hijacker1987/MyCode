import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { postApi } from "../Services/Api";
import { homePage } from "../Services/Frontend.Endpoints";
import { userLogin } from "../Services/Backend.Endpoints";
import { useUser } from "../Services/UserContext";
import Login from "../Components/Login/Login";
import Loading from "../Components/Loading/Loading";
import Notify from "../Pages/Services/ToastNotifications";
import ErrorPage from "./Services/ErrorPage";

const UserLogin = () => {
    const navigate = useNavigate();
    const { setUserData } = useUser();
    const [loading, setLoading] = useState(false);
    const [errorMessage, setError] = useState("");

    const handleOnLogin = async (user) => {
        setLoading(true);
        try {
            const data = await postApi(userLogin, user);

            if (data) {
                setUserData(data.role, data.userid);
                Notify("Success", "Successful Login!");
                navigate(homePage);
            } else {
                Notify("Error", "Unable to Login!");
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
                {errorMessage == "" ? ( <Login onLogin={handleOnLogin} onCancel={handleCancel} />
                                  ) : (
                                        <ErrorPage errorMessage={errorMessage} />
                                  )}
           </div>
};

export default UserLogin;
