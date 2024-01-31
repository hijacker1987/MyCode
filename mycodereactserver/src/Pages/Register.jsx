import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { postApi } from "../Services/Api";
import { userRegistration } from "../Services/Backend.Endpoints";
import { toast } from "react-toastify";
import UserForm from "../Components/Forms/UserForm";
import Loading from "../Components/Loading/Loading";
import ErrorPage from "./Services/ErrorPage";
import "react-toastify/dist/ReactToastify.css";

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
                    toast.success("Successful Registration!", {
                        position: "top-right",
                        autoClose: 5000,
                        hideProgressBar: false,
                        closeOnClick: true,
                        pauseOnHover: true,
                        draggable: true,
                        progress: undefined,
                        theme: "dark",
                    });
                } else {
                    toast.error("Unable to Register!", {
                        position: "top-right",
                        autoClose: 5000,
                        hideProgressBar: false,
                        closeOnClick: true,
                        pauseOnHover: true,
                        draggable: true,
                        progress: undefined,
                        theme: "dark"
                    });
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
