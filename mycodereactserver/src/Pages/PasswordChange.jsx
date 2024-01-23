import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { getApi, patchApi } from "../Services/Api";
import { userById, changePassword } from "../Services/Backend.Endpoints";
import { jwtDecode } from "jwt-decode";
import PassChange from "../Components/PassChange/PassChange";
import Loading from "../Components/Loading/Loading";
import ErrorPage from "./Service/ErrorPage";
import Cookies from "js-cookie";

const PasswordChange = () => {
    const navigate = useNavigate();
    const [loading, setLoading] = useState(false);
    const [errorMessage, setPWCError] = useState("");
    const [userData, setUserData] = useState(null);

    useEffect(() => {
        setLoading(true);
        try {
            const token = Cookies.get("jwtToken");

            if (typeof token === "string" && token.length > 0) {
                const decodedToken = jwtDecode(token);
                const userIdFromToken = decodedToken["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier" || []];

                getApi(token, userById + userIdFromToken)
                    .then((getUserData) => {
                        setLoading(false);
                        setUserData(getUserData);
                    })
                    .catch((error) => {
                        setLoading(false);
                        console.error('Error occurred while fetching user data:', error);
                        setUpdateError('An error occurred while fetching user data. Please try again.');
                    });
            }
        } catch (error) {
            console.error("Error decoding JWT token:", error);
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
            } else {
                setPWCError("Error occurred during password change.");
            }
        } catch (error) {
            setLoading(false);
            console.error("Error occurred during password change: ", error.message);
        }
    };

    const handleCancel = () => {
        navigate("/");
    };

    const getToken = () => {
        return Cookies.get("jwtToken");
    }

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
                    <ErrorPage
                        errorMessage={errorMessage}
                    />
                )}
            </div>
};

export default PasswordChange;
