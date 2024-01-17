import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { postApi } from "../Services/Api";
import { userLogin } from "../Services/Backend.Endpoints";
import Login from "../Components/Login/Login";
import Loading from "../Components/Loading/Loading";
import Cookies from "js-cookie";

const UserLogin = () => {
    const navigate = useNavigate();
    const [loading, setLoading] = useState(false);
    const [userRoles, setUserRoles] = useState([]);

    const handleOnLogin = (user) => {
        setLoading(true);

        postApi(user, userLogin)
            .then((data) => {
                setLoading(false);
                if (data.token) {
                    const decodedToken = jwt_decode(data.token);
                    const expirationTime = decodedToken.exp * 1000;
                    const userRoles = decodedToken.roles;

                    Cookies.set("jwtToken", data.token, { expires: new Date(expirationTime) });
                    setUserRoles(roles);

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

    const checkTokenExpiration = () => {
        const token = Cookies.get("jwtToken");
        if (token) {
            const decodedToken = jwt_decode(token);
            const expirationTime = decodedToken.exp * 1000;

            if (expirationTime < Date.now()) {
                handleLogout();
            }
        }
    };

    const handleLogout = () => {
        Cookies.remove("jwtToken");
        navigate("/");
        setJwtToken(null);
        setUserRoles([]);
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

    return (
        <Login
            onLogin={handleOnLogin}
            onCancel={handleCancel}
        />
    );
};

export default UserLogin;
