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
                if (data) {
                    navigate("/");
                } else {
                    setRegError("An error occurred during registration. Please try again.");
                }
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
