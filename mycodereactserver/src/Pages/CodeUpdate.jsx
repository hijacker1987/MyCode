import React, { useState, useEffect } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { getApi, putApi } from "../Services/Api";
import { getCodesByUserId, codeUpdate, codeSuperUpdate } from "../Services/Backend.Endpoints";
import { jwtDecode } from "jwt-decode";
import CodeForm from "../Components/Forms/CodeForm/CodeForm";
import Loading from "../Components/Loading/Loading";
import ErrorPage from "./Service/ErrorPage";
import Cookies from "js-cookie";

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
                const apiUrl = `${getCodesByUserId}${codeId}`;
                const data = await getApi(token, apiUrl);
                setLoading(false);
                setCodeData(data);
                const decodedToken = jwtDecode(token);
                const role = decodedToken["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] || [];
                setUserRole(role);
            } catch (error) {
                setLoading(false);
                console.error('Error occurred while fetching user data:', error);
                setUpdateError('An error occurred while fetching user data. Please try again.');
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
                } else {
                    setUpdateError("Update was unsuccessful.");
                }
            })
            .catch((error) => {
                setLoading(false);
                console.error("Error occurred during update: ", error);
            });
    };

    const getToken = () => {
        return Cookies.get("jwtToken");
    }

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
            <ErrorPage
                errorMessage={errorMessage}
            />
        )}
    </div>
};

export default CodeUpdate;