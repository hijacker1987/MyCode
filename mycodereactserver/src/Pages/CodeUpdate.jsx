import React, { useState, useEffect } from "react";
import { useNavigate, useParams } from "react-router-dom";

import { ErrorPage, Notify } from "./Services";
import { useUser } from "../Services/UserContext";
import { getApi, putApi, handleResponse } from "../Services/Api";
import { getCodesByUserId, codeUpdate, codeSuperUpdate } from "../Services/Backend.Endpoints";
import CodeForm from "../Components/Forms/CodeForm/index";
import Loading from "../Components/Loading/index";

const CodeUpdate = () => {
    const navigate = useNavigate();
    const { codeId } = useParams();
    const { userData, setUserData } = useUser();
    const { role } = userData;
    const [loading, setLoading] = useState(false);
    const [errorMessage, setError] = useState("");
    const [codeData, setCodeData] = useState(null);

    useEffect(() => {
        const fetchData = async () => {
            try {
                setLoading(true);
                const data = await getApi(`${getCodesByUserId}${codeId}`);

                setLoading(false);
                if (data === "Unauthorized") {
                    handleResponse(data, navigate, setUserData);
                } else if (data) {
                    setCodeData(data);
                } else {
                    Notify("Error", "Unable to fetch the codes!");
                }
            } catch (error) {
                setLoading(false);
                setError(`Error occurred while fetching code data: ${error}`);
            }
        };

        fetchData();
    }, [codeId]);

    const handleOnSave = (code) => {
        setLoading(true);

        const endpoint = role.includes("Admin") ? codeSuperUpdate : codeUpdate;
        const apiUrl = `${endpoint}${codeId}`;

        putApi(apiUrl, code)
            .then((data) => {
                setLoading(false);

                if (data === "Unauthorized") {
                    handleResponse(responseData, navigate, setUserData);
                } else if (data) {
                    setCodeData(data);
                    navigate(-1);
                    Notify("Success", "Successful code update!");
                } else {
                    Notify("Error", "Unable to update the codes!");
                }
            })
            .catch((error) => {
                setLoading(false);
                setError(`Error occurred while updating code data: ${error}`);
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
                                  onSave={handleOnSave}
                                  code={codeData}
                                  role={role}
                                  onCancel={handleCancel}
                               />
                          ) : (
                               <ErrorPage errorMessage={errorMessage} />
                          )}
    </div>
};

export default CodeUpdate;
