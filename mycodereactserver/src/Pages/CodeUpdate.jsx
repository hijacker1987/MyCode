import React, { useState, useEffect } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { getApi, putApi } from "../Services/Api";
import { getToken, getUserRoles } from "../Services/AuthService";
import { getCodesByUserId, codeUpdate, codeSuperUpdate } from "../Services/Backend.Endpoints";
import { toast } from "react-toastify";
import CodeForm from "../Components/Forms/CodeForm/CodeForm";
import Loading from "../Components/Loading/Loading";
import ErrorPage from "./Services/ErrorPage";
import "react-toastify/dist/ReactToastify.css";

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
                    toast.error("Unable to fetch the codes!", {
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
                    toast.success("Successful code update!", {
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
                    toast.error("Unable to update the codes!", {
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
