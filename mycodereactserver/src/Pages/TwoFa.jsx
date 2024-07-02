import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";

import { getStatApi, postStatApi, postStatExtApi } from "../Services/Api";
import { useUser } from "../Services/UserContext";
import { ErrorPage, Notify } from "./Services";
import { primary2fa, enable2fa, verify2fa, disable2fa, facebookAddon, gitHubAddon, googleAddon } from "../Services/Backend.Endpoints";
import { backendUrl } from "../Services/Config";
import TwoFactorAuthenticationForm from "../Components/Forms/TwoFaForm/index";
import Loading from "../Components/Loading/index";

const TwoFactorAuthentication = () => {
    const navigate = useNavigate();
    const { userData } = useUser();
    const { userid } = userData;
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
        getStatApi(primary2fa)
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
        postStatApi(enable2fa)
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
        postStatExtApi(verify2fa, code, false)
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
        postStatApi(disable2fa)
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

    const handleAddressUpdate = async (ext) => {
        setLoading(true);
        try {
            const whichExtAddon = ext == "GitHub" ? `${gitHubAddon}?attachment=${encodeURIComponent(userid)}`
                                : ext == "Google" ? `${googleAddon}?attachment=${encodeURIComponent(userid)}`
                                                  : `${facebookAddon}?attachment=${encodeURIComponent(userid)}`;

            window.location.href = await `${backendUrl}${whichExtAddon}`;
            
        } catch (error) {
            setError(`Error occurred during login: ${error}`);
        } finally {
            setLoading(false);
        }
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
