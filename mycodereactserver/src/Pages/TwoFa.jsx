import { useState } from "react";
import { useNavigate, useParams } from "react-router-dom";

import { postStatApi, postStatExtApi } from "../Services/Api";
import { ErrorPage, Notify } from "./Services";
import { enable2fa, verify2fa, disable2fa } from "../Services/Backend.Endpoints";
import TwoFactorAuthenticationForm from "../Components/Forms/TwoFaForm/index";
import Loading from "../Components/Loading/index";

const TwoFactorAuthentication = () => {
    const navigate = useNavigate();
    const { userIdParam } = useParams();
    const [loading, setLoading] = useState(false);
    const [errorMessage, setError] = useState("");

    const handleSendEmailWithCode = async () => {
        setLoading(true);
        postStatApi(enable2fa, userIdParam)
            .then(() => {
                setLoading(false);
                Notify("Success", "Please check Your e-mail!");
            })
            .catch((error) => {
                setLoading(false);
                setError(`Error occurred during verifycation: ${error}`);
            });
    }

    const handleVerify2fa = (code) => {
        setLoading(true);
        postStatExtApi(verify2fa, userIdParam, code)
            .then((res) => {
                setLoading(false);
                navigate(-1);
                if (res.status != 400) {
                    Notify("Success", "Successful Verification!");
                } else {
                    Notify("Error", "Unable to Verify!");
                }
            })
            .catch((error) => {
                setLoading(false);
                setError(`Error occurred during verifycation: ${error}`);
            });
    };

    const handleDisable2fa = async () => {
        setLoading(true);
        postStatApi(disable2fa, userIdParam)
            .then(() => {
                setLoading(false);
                Notify("Success", "2fa disabled!");
            })
            .catch((error) => {
                setLoading(false);
                setError(`Error occurred during disable: ${error}`);
            });
    }

    const handleCancel = () => {
        navigate(-1);
    };

    if (loading) {
        return <Loading />;
    }

    return <div>
        {errorMessage == "" ? (
            <TwoFactorAuthenticationForm onEnable={handleSendEmailWithCode} onSubmit={handleVerify2fa} onDisable={handleDisable2fa} onCancel={handleCancel} />
        ) : (
            <ErrorPage errorMessage={errorMessage} />
        )}
    </div>
};

export default TwoFactorAuthentication;
