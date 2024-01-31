import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { getApi, patchApi } from "../Services/Api";
import { getToken } from "../Services/AuthService";
import { getUser, changePassword } from "../Services/Backend.Endpoints";
import { toast } from "react-toastify";
import PassChange from "../Components/PassChange/PassChange";
import Loading from "../Components/Loading/Loading";
import ErrorPage from "./Services/ErrorPage";
import "react-toastify/dist/ReactToastify.css";

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
                toast.success("Password changed!", {
                    position: "top-right",
                    autoClose: 5000,
                    hideProgressBar: false,
                    closeOnClick: true,
                    pauseOnHover: true,
                    draggable: true,
                    progress: undefined,
                    theme: "dark",
                });
            } else {
                toast.error("Unable to change!", {
                    position: "top-right",
                    autoClose: 5000,
                    hideProgressBar: false,
                    closeOnClick: true,
                    pauseOnHover: true,
                    draggable: true,
                    progress: undefined,
                    theme: "dark"
                });
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
