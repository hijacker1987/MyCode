import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { postApi } from "../Services/Api";
import UserForm from "../Components/UserForm";
import Loading from "../Components/Loading/Loading";

const UserRegister = () => {
    const navigate = useNavigate();
    const [loading, setLoading] = useState(false);

    const handleCreateUser = (user) => {
        setLoading(true);

        postApi(user, "registerUser")
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
