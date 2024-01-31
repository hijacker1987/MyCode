import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { postApi } from "../Services/Api";
import { userRegistration } from "../Services/Backend.Endpoints";
import UserForm from "../Components/Forms/UserForm";
import Loading from "../Components/Loading/Loading";
import Notify from "./Services/ToastNotifications";
import ErrorPage from "./Services/ErrorPage";

const UserRegister = () => {
    const navigate = useNavigate();
    const [loading, setLoading] = useState(false);
    const [errorMessage, setRegError] = useState("");

    const handleCreateUser = (user) => {
        setLoading(true);
        postApi(user, userRegistration)
            .then((res) => {
                setLoading(false);
                navigate("/");
                if (res.status != 400) {
                    Notify("Success", "Successful Registration!");
                } else {
                    Notify("Error", "Unable to Register!");
                }
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
                                      <UserForm onSave={handleCreateUser} onCancel={handleCancel} />
                                  ) : (
                                      <ErrorPage errorMessage={errorMessage} />
                                  )}
           </div>
};

export default UserRegister;
