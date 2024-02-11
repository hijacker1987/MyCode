import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { useUser } from "../Services/UserContext";
import { postApi, handleResponse } from "../Services/Api";
import { codeRegistration } from "../Services/Backend.Endpoints";
import CodeForm from "../Components/Forms/CodeForm";
import Loading from "../Components/Loading/Loading";
import Notify from "./Services/ToastNotifications";
import ErrorPage from "./Services/ErrorPage";

const CodeRegister = () => {
    const navigate = useNavigate();
    const { setUserData } = useUser();
    const [loading, setLoading] = useState(false);
    const [errorMessage, setError] = useState("");

    const handleCreateCode = (code) => {
        setLoading(true);
        postApi(codeRegistration, code)
            .then((data) => {
                setLoading(false);
                if (data === "Unauthorized") {
                    handleResponse(data, navigate, setUserData);
                } else if (data) {
                    navigate(-1);
                    Notify("Success", "Successful code registration!");
                } else {
                    Notify("Error", "Unable to register the code!");
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
                               <CodeForm
                                  onSave={handleCreateCode}
                                  onCancel={handleCancel}
                               />
                          ) : (
                               <ErrorPage errorMessage={errorMessage} />
                          )}
    </div>
};

export default CodeRegister;
