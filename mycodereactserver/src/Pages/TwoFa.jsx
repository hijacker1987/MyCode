import { useState, useEffect } from "react";
import { useNavigate, useParams } from "react-router-dom";

import { getStatApi, patchExtApi, postStatApi, postStatExtApi } from "../Services/Api";
import { ErrorPage, Notify } from "./Services";
import { primary2fa, enable2fa, verify2fa, disable2fa, reliableAdd } from "../Services/Backend.Endpoints";
import TwoFactorAuthenticationForm from "../Components/Forms/TwoFaForm/index";
import Loading from "../Components/Loading/index";

const TwoFactorAuthentication = () => {
    const navigate = useNavigate();
    const { userIdParam } = useParams();
    const [isEmailConfirmed, setEmailConfirmed] = useState(false);
    const [isTwoFactorEnabled, setTwoFactorEnabled] = useState(false);
    const [reliableAddress, setReliableAddress] = useState(false);
    const [loading, setLoading] = useState(false);
    const [errorMessage, setError] = useState("");

    useEffect(() => {
        handleUserBasic();
    }, []);

    const handleUserBasic = async () => {
        setLoading(true);
        getStatApi(primary2fa, userIdParam)
            .then((data) => {
                setLoading(false);
                setEmailConfirmed(data.emailConfirmed);
                setTwoFactorEnabled(data.twoFactorEnabled);
                setReliableAddress(data.isReliable);
            })
            .catch((error) => {
                setLoading(false);
                setError(`Error occurred getting basic data for verify: ${error}`);
            });
    }

    const handleSendEmailWithCode = async () => {
        setLoading(true);
        postStatApi(enable2fa, userIdParam)
            .then(() => {
                setLoading(false);
                Notify("Success", "Please check Your e-mail!");
                handleUserBasic();
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
                    handleUserBasic();
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
                handleUserBasic();
            })
            .catch((error) => {
                setLoading(false);
                setError(`Error occurred during disable: ${error}`);
            });
    }

    const handleAddressUpdate = (address) => {
        setLoading(true);
        patchExtApi(reliableAdd, userIdParam, address)
            .then((res) => {
                setLoading(false);
                navigate(-1);
                if (res.status != 400) {
                    Notify("Success", "Successful Verification!");
                    handleUserBasic();
                } else {
                    Notify("Error", "Unable to Verify!");
                }
            })
            .catch((error) => {
                setLoading(false);
                setError(`Error occurred during verifycation: ${error}`);
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
            <TwoFactorAuthenticationForm
                onEnable={handleSendEmailWithCode}
                onSubmit={handleVerify2fa}
                onSubmitAddress={handleAddressUpdate}
                onDisable={handleDisable2fa}
                onCancel={handleCancel}
                isEmailConfirmed={isEmailConfirmed}
                isTwoFactorEnabled={isTwoFactorEnabled}
                isReliable={reliableAddress}
            />
        ) : (
            <ErrorPage errorMessage={errorMessage} />
        )}
    </div>
};

export default TwoFactorAuthentication;
