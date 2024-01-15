import { useState } from "react";
import { useNavigate } from "react-router-dom";
import Login from "../Components/Login/Login";
import Cookies from "js-cookie";
import Loading from "../Components/Loading/Loading";

const postLogin = async (credentials) => {
    try {
        const response = await fetch('https://localhost:7001/login', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(credentials),
        });

        const data = await response.json();

        if ('token' in data) {
            console.log('Response from server:', data);
            return data;
        } else {
            console.error('Invalid response format:', data);
            throw new Error('Invalid response format');
        }
    } catch (error) {
        console.error('Error occurred during login:', error);
        throw error;
    }
};


const UserLogin = () => {
    const navigate = useNavigate();
    const [loading, setLoading] = useState(false);

    const handleOnLogin = (user) => {
        setLoading(true);
        console.log(user);
        postLogin(user)
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
