import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { getApi, patchApi } from "../Services/Api";
import { getToken } from "../Services/AuthService";
import { getUser, changePassword } from "../Services/Backend.Endpoints";
import PassChange from "../Components/PassChange/PassChange";
import Loading from "../Components/Loading/Loading";
import Notify from "./Services/ToastNotifications";
import ErrorPage from "./Services/ErrorPage";

const PasswordChange = () => {
    const navigate = useNavigate();
    const [loading, setLoading] = useState(false);
    const [errorMessage, setPWCError] = useState("");
    const [userData, setUserData] = useState(null);

    useEffect(() => {
        setLoading(true);
        try {
            const token = getToken();

            if (typeof token === "string" && token.length > 0) {
                getApi(token, getUser)
                    .then((getUserData) => {
                        setLoading(false);
                        setUserData(getUserData);
                    })
                    .catch((error) => {
                        setLoading(false);
                        setUpdateError(`Error occurred while fetching user data: ${error}`);
                    });
            }
        } catch (error) {
            setUpdateError(`Error occurred while fetching user data: ${error}`);
        }
    }, []);

    const handleUserPasswordUpdate = async (user) => {
        setLoading(true);
        const token = getToken();

        try {
            const response = await patchApi(user, token, changePassword);

            setLoading(false);
            if (response && response.message) {
                navigate("/");
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
                        user={userData}
                        onCancel={handleCancel}
                    />
                ) : (
                    <ErrorPage errorMessage={errorMessage} />
                )}
            </div>
};

export default PasswordChange;
