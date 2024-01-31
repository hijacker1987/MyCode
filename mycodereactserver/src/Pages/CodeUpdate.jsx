import React, { useState, useEffect } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { getApi, putApi } from "../Services/Api";
import { getToken, getUserRoles } from "../Services/AuthService";
import { getCodesByUserId, codeUpdate, codeSuperUpdate } from "../Services/Backend.Endpoints";
import CodeForm from "../Components/Forms/CodeForm/CodeForm";
import Loading from "../Components/Loading/Loading";
import Notify from "./Services/ToastNotifications";
import ErrorPage from "./Services/ErrorPage";

const CodeUpdate = () => {
    const navigate = useNavigate();
    const { codeId } = useParams();
    const [loading, setLoading] = useState(false);
    const [errorMessage, setUpdateError] = useState("");
    const [userRole, setUserRole] = useState([]);
    const [codeData, setCodeData] = useState(null);

    useEffect(() => {
        const fetchData = async () => {
            try {
                setLoading(true);
                const token = getToken();
                const role = getUserRoles();
                const apiUrl = `${getCodesByUserId}${codeId}`;
                const data = await getApi(token, apiUrl);

                setLoading(false);
                if (data) {
                    setCodeData(data);
                    setUserRole(role);
                } else {
                    Notify("Error", "Unable to fetch the codes!");
                }
            } catch (error) {
                setLoading(false);
                setUpdateError(`Error occurred while fetching code data: ${error}`);
            }
        };

        fetchData();
    }, [codeId]);

    const handleOnSave = (code) => {
        setLoading(true);
        const token = getToken();
        const endpoint = userRole.includes("Admin") ? codeSuperUpdate : codeUpdate;
        const apiUrl = `${endpoint}${codeId}`;

        putApi(code, token, apiUrl)
            .then((data) => {
                setLoading(false);
                if (data) {
                    setCodeData(data);
                    navigate(-1);
                    Notify("Success", "Successful code update!");
                } else {
                    Notify("Error", "Unable to update the codes!");
                }
            })
            .catch((error) => {
                setLoading(false);
                setUpdateError(`Error occurred while updating code data: ${error}`);
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
                role={userRole}
                onCancel={handleCancel}
            />
        ) : (
            <ErrorPage errorMessage={errorMessage} />
        )}
    </div>
};

export default CodeUpdate;
