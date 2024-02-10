import React, { useState, useEffect } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { getApi, putApi } from "../Services/Api";
import { useUser } from "../Services/UserContext";
import { getUser, userById, userUpdate, userSuperUpdate } from "../Services/Backend.Endpoints";
import UserForm from "../Components/Forms/UserForm/UserForm";
import Loading from "../Components/Loading/Loading";
import Notify from "./Services/ToastNotifications";
import ErrorPage from "./Services/ErrorPage";

const UserUpdate = () => {
    const navigate = useNavigate();
    const { userIdParam } = useParams();
    const { userData } = useUser();
    const { role, userid } = userData;
    const [user, setUser] = useState(null);
    const [loading, setLoading] = useState(false);
    const [errorMessage, setError] = useState("");

    useEffect(() => {
        const fetchData = async () => {
            try {
                setLoading(true);
                const userEndpoint = role === "Admin" ? `${userById}${userIdParam}` : getUser;
                const data = await getApi(userEndpoint);
                setLoading(false);
                setUser(data);
            } catch (error) {
                setLoading(false);
                setError(`Error occurred while fetching user data: ${error}`);
            }
        };

        fetchData();
    }, [userIdParam]);

    const handleOnSave = (user) => {
        setLoading(true);
        const endpoint = role === "Admin" ? userSuperUpdate : userUpdate;
        const apiUrl = `${endpoint}${user.id}`;

        putApi(apiUrl, user)
            .then((data) => {
                setLoading(false);
                if (data) {
                    setUser(data);
                    navigate(-1);
                    Notify("Success", "Successful Update!");
                } else {
                    Notify("Error", "Unable to Update!");
                }
            })
            .catch((error) => {
                setLoading(false);
                setError(`Error occurred during update: ${error}`);
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
                        role={role}
                        onCancel={handleCancel}
                    />
                ) : (
                    <ErrorPage errorMessage={errorMessage} />
                )}
            </div>
};

export default UserUpdate;
