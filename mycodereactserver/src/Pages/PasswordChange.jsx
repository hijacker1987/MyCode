import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { useUser } from "../Services/UserContext";
import { getApi, patchApi, handleResponse } from "../Services/Api";
import { homePage } from "../Services/Frontend.Endpoints";
import { getUser, changePassword } from "../Services/Backend.Endpoints";
import PassChange from "../Components/PassChange/PassChange";
import Loading from "../Components/Loading/Loading";
import Notify from "./Services/ToastNotifications";
import ErrorPage from "./Services/ErrorPage";

const PasswordChange = () => {
    const navigate = useNavigate();
    const { setUserData } = useUser();
    const [loading, setLoading] = useState(false);
    const [data, setData] = useState(null);
    const [errorMessage, setError] = useState("");

    useEffect(() => {
        setLoading(true);

        try {
            getApi(getUser)
                .then((getUserData) => {
                    setLoading(false);

                    if (getUserData === "Unauthorized") {
                        handleResponse(getUserData, navigate, setUserData);
                    } else {
                        setData(getUserData);
                    }
                })
        } catch (e) {
            setLoading(false);
            setError(`Error occurred while fetching user data: ${error}`);
        }
    }, []);

    const handleUserPasswordUpdate = async (user) => {
        setLoading(true);

        try {
            const response = await patchApi(changePassword, user);

            setLoading(false);
            if (response === "Unauthorized") {
                handleResponse(response, navigate, setUserData);
            } else if (response && response.message) {
                navigate(homePage);
                Notify("Success", "Password changed!");
            } else {
                Notify("Error", "Unable to change!");
            }
        } catch (error) {
            setLoading(false);
            setPWCError(`Error occurred during password change: ${error.message}`);
        }
    };

    const handleCancel = () => {
        navigate(-1);
    };

    if (loading) {
        return <Loading />;
    }

    return <div>
                {errorMessage == "" ? (
                                       <PassChange
                                          onPassChange={handleUserPasswordUpdate}
                                          user={data}
                                          onCancel={handleCancel}
                                       />
                                  ) : (
                                       <ErrorPage errorMessage={errorMessage} />
                                  )}
            </div>
};

export default PasswordChange;
