import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { postApiV2 } from "../Services/Api";
import { getToken } from "../Services/AuthService";
import { codeRegistration } from "../Services/Backend.Endpoints";
import { toast } from "react-toastify";
import CodeForm from "../Components/Forms/CodeForm";
import Loading from "../Components/Loading/Loading";
import ErrorPage from "./Service/ErrorPage";
import "react-toastify/dist/ReactToastify.css";

const CodeRegister = () => {
    const navigate = useNavigate();
    const [loading, setLoading] = useState(false);
    const [errorMessage, setRegError] = useState("");

    const handleCreateCode = (code) => {
        setLoading(true);
        const token = getToken();

        postApiV2(code, token, codeRegistration)
            .then((data) => {
                setLoading(false);
                if (data) {
                    navigate(-1);
                    toast.success("Successful code registration!", {
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
                    toast.error("Unable to register the code!", {
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
