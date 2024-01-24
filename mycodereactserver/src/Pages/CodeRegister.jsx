import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { postApiV2 } from "../Services/Api";
import { codeRegistration } from "../Services/Backend.Endpoints";
import CodeForm from "../Components/Forms/CodeForm";
import Loading from "../Components/Loading/Loading";
import ErrorPage from "./Service/ErrorPage";
import Cookies from "js-cookie";

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
                } else {
                    setRegError("An error occurred during registration. Please try again.");
                }
            })
            .catch((error) => {
                setLoading(false);
                console.error("Error occurred during registration:", error);
            });
    };

    const getToken = () => {
        return Cookies.get("jwtToken");
    }

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
            <ErrorPage
                errorMessage={errorMessage}
            />
        )}
    </div>
};

export default CodeRegister;
