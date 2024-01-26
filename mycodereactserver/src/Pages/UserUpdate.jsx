import React, { useState, useEffect } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { getApi, putApi } from "../Services/Api";
import { getUser, userById, userUpdate, userSuperUpdate } from "../Services/Backend.Endpoints";
import { jwtDecode } from "jwt-decode";
import UserForm from "../Components/Forms/UserForm/UserForm";
import Loading from "../Components/Loading/Loading";
import ErrorPage from "./Service/ErrorPage";
import Cookies from "js-cookie";

const UserUpdate = () => {
    const navigate = useNavigate();
    const { userId } = useParams();
    const [loading, setLoading] = useState(false);
    const [errorMessage, setUpdateError] = useState("");
    const [userRole, setUserRole] = useState([]);
    const [user, setUser] = useState(null);
    const [userDataId, setUserDataId] = useState(null);

    useEffect(() => {
        const fetchData = async () => {
            try {
                setLoading(true);
                const token = getToken();
                const decodedToken = jwtDecode(token);
                const role = decodedToken["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] || [];
                setUserRole(role);

                const userEndpoint = role === "Admin" ? `${userById}${userId}` : getUser;
                const userData = await getApi(token, userEndpoint);
                if (role === "User") {
                    setUserDataId(userData.id);
                }

                setLoading(false);
                setUser(userData);
            } catch (error) {
                setLoading(false);
                console.error('Error occurred while fetching user data:', error);
                setUpdateError(
                    'An error occurred while fetching user data. Please try again.'
                );
            }
        };

        fetchData();
    }, [userId]);

    const handleOnSave = (user) => {
        setLoading(true);
        const token = getToken();
        const endpoint = userRole.includes("Admin") ? userSuperUpdate : userUpdate;
        const apiUrl = userRole.includes("Admin") ? `${endpoint}${user.id}` : `${endpoint}${userDataId}`;

        putApi(user, token, apiUrl)
            .then((data) => {
                setLoading(false);
                if (data) {
                    setUser(data);
                    navigate("/");
                } else {
                    setUpdateError("Update was unsuccessful.");
                }
            })
            .catch((error) => {
                setLoading(false);
                console.error("Error occurred during update: ", error);
                if (error.response) {
                    console.error("Error Response Data:", error.response.data);
                }
            });
    };

    const getToken = () => {
        return Cookies.get("jwtToken");
    }

    const handleCancel = () => {
        navigate(-1);
    };

    if (loading) {
        return <Loading />;
    }

    return <div>
                {errorMessage == "" ? (
                    <UserForm
                        onSave={handleOnSave}
                        user={user}
                        role={userRole}
                        onCancel={handleCancel}
                    />
                ) : (
                    <ErrorPage
                        errorMessage={errorMessage}
                    />
                )}
            </div>
};

export default UserUpdate;
