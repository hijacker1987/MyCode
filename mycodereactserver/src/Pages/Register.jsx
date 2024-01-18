import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { postApi } from "../Services/Api";
import { userRegistration } from "../Services/Backend.Endpoints";
import UserForm from "../Components/UserForm";
import Loading from "../Components/Loading/Loading";

const UserRegister = () => {
    const navigate = useNavigate();
    const [loading, setLoading] = useState(false);

    const handleCreateUser = (user) => {
        setLoading(true);

        postApi(user, userRegistration)
            .then(() => {
                setLoading(false);
                navigate("/");
            })
            .catch((error) => {
                setLoading(false);
                console.error("Error occurred during registration:", error);
            });
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
