import { useState } from "react";
import { useNavigate } from "react-router-dom";
import UserForm from "../Components/UserForm";
import Loading from "../Components/Loading/Loading";

const createUser = async (user) => {
    console.log('User Data:', JSON.stringify(user));

    try {
        const response = await fetch('https://localhost:7001/registerUser', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(user),
        });

        const data = await response.json();

        console.log('Response:', response);
        console.log('Data:', data);

        if (data.errors) {
            console.log('Validation Errors:', data.errors);
        }

        return data;
    } catch (error) {
        console.error('Error occurred during login:', error);
        throw error;
    }
};

const UserRegister = () => {
    const navigate = useNavigate();
    const [loading, setLoading] = useState(false);

    const handleCreateUser = (user) => {
        setLoading(true);

        createUser(user)
            .then(() => {
                setLoading(false);
                navigate("/");
            })
    };

    const handleCancel = () => {
        navigate("/");
    };

    if (loading) {
        return <Loading />;
    }

    return (
        <UserForm
            onSave={handleCreateUser}
            onCancel={handleCancel}
        />
    );
};

export default UserRegister;
