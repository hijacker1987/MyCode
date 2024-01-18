import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { getApi } from "../../Services/Api";
import { getAllCodes } from "../../Services/Backend.Endpoints";

import Cookies from "js-cookie";
import Loading from "../../Components/Loading/Loading";
import CodesTable from "../../Components/Lists/CodesTable/CodesTable";

const CodesList = () => {
    const navigate = useNavigate();
    const [loading, setLoading] = useState(false);
    const [codes, setCodes] = useState(null);
    const headers = ["Code Title", "The Code itself", "What kind of code", "Back or Front", "Is it visible to others?"];

    useEffect(() => {
        const handleGet = async () => {
            setLoading(true);

            const token = getToken();
            try {
                const responseData = await getApi(token, getAllCodes);
                setLoading(false);
                if (responseData) {
                    setCodes(responseData);
                } else {
                    console.error("Received empty data from the server.");
                }
            } catch (error) {
                setLoading(false);
                console.error("Error occurred during getCodes:", error);
            }
        };

        handleGet();
    }, []);

    const getToken = () => {
        return Cookies.get("jwtToken");
    };

    const handleCancel = () => {
        navigate("/");
    };

    if (loading) {
        return <Loading />;
    }

    return (
        <CodesTable
            headers={headers}
            codes={codes}
            onCancel={handleCancel}
        />
    );
};

export default CodesList;
