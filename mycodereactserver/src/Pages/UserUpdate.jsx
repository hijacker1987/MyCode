import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { getApi, putApi } from '../Services/Api';
import { userById, userSuperUpdate } from '../Services/Backend.Endpoints';
import Cookies from "js-cookie";
import UserForm from '../Components/UserForm/UserForm';
import Loading from '../Components/Loading/Loading';
import ErrorPage from './Service/ErrorPage';

const UserUpdate = () => {
    const navigate = useNavigate();
    const { userId } = useParams();
    const [loading, setLoading] = useState(true);
    const [errorMessage, setUpdateError] = useState('');
    const [user, setUser] = useState(null);

    useEffect(() => {
        const token = getToken();

        getApi(token, userById + userId)
            .then((userData) => {
                setLoading(false);
                setUser(userData);
            })
            .catch((error) => {
                setLoading(false);
                console.error('Error occurred while fetching user data:', error);
                setUpdateError('An error occurred while fetching user data. Please try again.');
            });
    }, [userId]);

    const handleOnSave = (user) => {
        const token = getToken();

        putApi(user, token, userSuperUpdate + userId)
            .then((data) => {
                setLoading(false);
                if (data) {
                    setUser(data);
                    navigate('/');
                } else {
                    setUpdateError('Update unsuccessful.');
                }
            })
            .catch((error) => {
                setLoading(false);
                console.error('Error occurred during update:', error);
                setUpdateError('An error occurred during update. Please try again.');
            });
    };

    const getToken = () => {
        return Cookies.get("jwtToken");
    }

    const handleCancel = () => {
        navigate('/');
    };

    if (loading) {
        return <Loading />;
    }

    return <div>
        {errorMessage == '' ? (
            <UserForm
                onSave={handleOnSave}
                user={user}
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
