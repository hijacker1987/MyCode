import React, { useState, useEffect } from "react";
import { useNavigate, useParams } from "react-router-dom";

import { ErrorPage, Notify } from "./Services";
import { useUser } from "../Services/UserContext";
import { getApi, postApi, putApi, handleResponse } from "../Services/Api";
import { getUser, userById, userUpdate, userSuperUpdate, changeRole } from "../Services/Backend.Endpoints";
import UserForm from "../Components/Forms/UserForm/index";
import Loading from "../Components/Loading/index";

const UserUpdate = () => {
    const navigate = useNavigate();
    const { userIdParam } = useParams();
    const { userData, setUserData } = useUser();
    const { role } = userData;
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

                if (data === "Unauthorized") {
                    handleResponse(data, navigate, setUserData);
                } else {
                    setUser(data);
                }              
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

                if (data === "Unauthorized") {
                    handleResponse(data, navigate, setUserData);
                } else if (data) {
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

    const handleOnRole = (userToChange) => {
        setLoading(true);
        const requestData = { status: userToChange };

        postApi(changeRole, requestData)
            .then((data) => {
                setLoading(false);
                if (data === "Unauthorized") {
                    handleResponse(data, navigate, setUserData);
                } else if (data.status == 404) {
                    Notify("Error", "Unable to Change OWN Role!");
                } else if (data) {
                    navigate(-1);
                    Notify("Success", "Successful Role Change!");
                } else {
                    Notify("Error", "Unable to Change Role!");
                }
            })
            .catch((error) => {
                setLoading(false);
                setError(`Error occurred during role change: ${error}`);
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
                        onRole={handleOnRole}
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
