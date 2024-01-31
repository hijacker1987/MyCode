import React, { useState, useEffect } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { getApi, putApi } from "../Services/Api";
import { getToken, getUserRoles } from "../Services/AuthService";
import { getUser, userById, userUpdate, userSuperUpdate } from "../Services/Backend.Endpoints";
import { toast } from "react-toastify";
import UserForm from "../Components/Forms/UserForm/UserForm";
import Loading from "../Components/Loading/Loading";
import ErrorPage from "./Services/ErrorPage";
import "react-toastify/dist/ReactToastify.css";

const UserUpdate = () => {
    const navigate = useNavigate();
    const { userId } = useParams();
    const [user, setUser] = useState(null);
    const [userRole, setUserRole] = useState([]);
    const [loading, setLoading] = useState(false);
    const [userDataId, setUserDataId] = useState(null);
    const [errorMessage, setUpdateError] = useState("");

    useEffect(() => {
        const fetchData = async () => {
            try {
                setLoading(true);
                const token = getToken();
                const role = getUserRoles();
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
                setUpdateError(`Error occurred while fetching user data: ${error}`);
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
                    navigate(-1);
                    toast.success(`Successful Update!`, {
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
                    toast.error("Unable to Update!", {
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
            })
            .catch((error) => {
                setLoading(false);
                setUpdateError(`Error occurred during update: ${error}`);
            });
    };

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
                    <ErrorPage errorMessage={errorMessage} />
                )}
            </div>
};

export default UserUpdate;
