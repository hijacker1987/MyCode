import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { postApi } from "../Services/Api";
import { userRegistration } from "../Services/Backend.Endpoints";
import UserForm from "../Components/Forms/UserForm";
import Loading from "../Components/Loading/Loading";
import ErrorPage from "./Service/ErrorPage";

const UserRegister = () => {
    const navigate = useNavigate();
    const [loading, setLoading] = useState(false);
    const [errorMessage, setRegError] = useState("");

    const handleCreateUser = (user) => {
        setLoading(true);
        postApi(user, userRegistration)
            .then((data) => {
                setLoading(false);
                navigate("/");
            })
            .catch((error) => {
                setLoading(false);
                setRegError(`Error occurred during registration: ${error}`);
            });
    };

    const handleCancel = () => {
        navigate("/");
    };

    if (loading) {
        return <Loading />;
    }

    return <div>
                {errorMessage == "" ? (
                    <UserForm
                        onSave={handleCreateUser}
                        onCancel={handleCancel}
                    />
                ) : (
                    <ErrorPage
                        errorMessage={errorMessage}
                    />
                )}
            </div>
};

export default UserRegister;
