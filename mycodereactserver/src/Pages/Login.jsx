import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { postApi } from "../Services/Api";
import Login from "../Components/Login/Login";
import Loading from "../Components/Loading/Loading";
import Cookies from "js-cookie";

const UserLogin = () => {
    const navigate = useNavigate();
    const [loading, setLoading] = useState(false);

    const handleOnLogin = (user) => {
        setLoading(true);

        postApi(user, "login")
            .then((data) => {
                setLoading(false);
                if (data.token) {
                    Cookies.set("jwtToken", data.token, { expires: 1 });
                    navigate("/");
                } else {
                    console.log("Login unsuccessful. Please check your credentials.");
                }
            })
            .catch((error) => {
                setLoading(false);
                console.error("Error occurred during login:", error);
            });
    };

    const handleCancel = () => {
        navigate("/");
    };

    if (loading) {
        return <Loading />;
    }

    return (
        <Login
            onLogin={handleOnLogin}
            onCancel={handleCancel}
        />
    );
};

export default UserLogin;
