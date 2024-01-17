import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { patchApi } from "../Services/Api";
import { changePassword } from "../Services/Backend.Endpoints";
import Cookies from "js-cookie";
import Loading from "../Components/Loading/Loading";
import PassChange from "../Components/PassChange/PassChange";

const PasswordChange = () => {
    const navigate = useNavigate();
    const [loading, setLoading] = useState(false);

    const handleUserPasswordUpdate = (user) => {
        setLoading(true);

        const token = getToken();

        patchApi(user, token, changePassword)
            .then((response) => {
                if (response && response.message) {
                    setLoading(false);
                    navigate("/");
                } else {
                    const errorMessage = response && response.error ? response.error : "Unknown error";
                    setLoading(false);
                    console.error("Error occurred during password change:", errorMessage);
                }
            })
            .catch((error) => {
                setLoading(false);
                console.error("Error occurred during password change:", error.message || "Unknown error");
            });
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

    return (
        <PassChange
            onPassChange={handleUserPasswordUpdate}
            onCancel={handleCancel}
        />
    );
};

export default PasswordChange;
