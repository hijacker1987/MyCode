import { useState, useEffect } from "react";
import { useNavigate, useParams } from "react-router-dom";

import { ErrorPage, Notify } from "./Services";
import { getApi, getStatApi, postApi, handleResponse } from "../Services/Api";
import { getUserId, codeRegistration, primary2fa } from "../Services/Backend.Endpoints";
import CodeForm from "../Components/Forms/CodeForm/index";
import Loading from "../Components/Loading/index";

const CodeRegister = () => {
    const navigate = useNavigate();
    const [isEmailConfirmed, setEmailConfirmed] = useState(false);
    const [isTwoFactorEnabled, setTwoFactorEnabled] = useState(false);
    const [loading, setLoading] = useState(false);
    const [errorMessage, setError] = useState("");

    useEffect(() => {
        handleUserBasic();
    }, []);

    const handleUserBasic = async () => {
        setLoading(true);
        try {
            const data = await getApi(getUserId)
            if (data === "Unauthorized") {
                handleResponse(data, navigate, setUserData);
            } else {
                getStatApi(primary2fa, data)
                    .then((data) => {
                        setLoading(false);
                        setEmailConfirmed(data.emailConfirmed);
                        setTwoFactorEnabled(data.twoFactorEnabled);
                    })
                    .catch((error) => {
                        setLoading(false);
                        setError(`Error occurred getting basic data for verify: ${error}`);
                    });
            }
        } catch (error) {
            setLoading(false);
            setError(`Error occurred while fetching user data: ${error}`);
        }
    }

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

    return (
        <div>
            {errorMessage === "" ? (
                !isEmailConfirmed && !isTwoFactorEnabled ? (
                    <>
                        <h3>Please verify Your account to be able to add/share Your knowledge</h3>
                    </>
                ) : (
                    <CodeForm onSave={handleCreateCode} onCancel={handleCancel} />
                )
            ) : (
                <ErrorPage errorMessage={errorMessage} />
            )}
        </div>
    );
};

export default CodeRegister;
