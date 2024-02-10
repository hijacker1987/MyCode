import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { postApi } from "../Services/Api";
import { homePage } from "../Services/Frontend.Endpoints";
import { userRegistration } from "../Services/Backend.Endpoints";
import UserForm from "../Components/Forms/UserForm";
import Loading from "../Components/Loading/Loading";
import Notify from "./Services/ToastNotifications";
import ErrorPage from "./Services/ErrorPage";

const UserRegister = () => {
    const navigate = useNavigate();
    const [loading, setLoading] = useState(false);
    const [errorMessage, setError] = useState("");

    const handleCreateUser = (user) => {
        setLoading(true);
        postApi(userRegistration, user)
            .then((res) => {
                setLoading(false);
                navigate(homePage);
                if (res.status != 400) {
                    Notify("Success", "Successful Registration!");
                } else {
                    Notify("Error", "Unable to Register!");
                }
            })
            .catch((error) => {
                setLoading(false);
                setError(`Error occurred during registration: ${error}`);
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
                                      <UserForm onSave={handleCreateUser} onCancel={handleCancel} />
                                  ) : (
                                      <ErrorPage errorMessage={errorMessage} />
                                  )}
           </div>
};

export default UserRegister;
