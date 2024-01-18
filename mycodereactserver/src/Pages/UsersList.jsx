import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { getApi } from "../Services/Api";
import { getAllUsers } from "../Services/Backend.Endpoints";
import Cookies from "js-cookie";
import Loading from "../Components/Loading/Loading";
import UsersTable from "../Components/Lists/UsersTable/UsersTable";

const UsersList = () => {
    const navigate = useNavigate();
    const [loading, setLoading] = useState(false);
    const [data, setData] = useState(null); 
    const headers = ["Display Name", "Last Time Logged in", "User Name", "E-mail address", "Phone Number"];

    useEffect(() => {
        const handleGet = async () => {
            setLoading(true);

            const token = getToken();
            try {
                const responseData = await getApi(token, getAllUsers);
                setLoading(false);
                if (responseData) {
                    setData(responseData);
                } else {
                    console.error("Received empty data from the server.");
                }
            } catch (error) {
                setLoading(false);
                console.error("Error occurred during getUsers:", error);
            }
        };

        handleGet();
    }, []);

    const getToken = () => {
        return Cookies.get("jwtToken");
    }

    const handleCancel = () => {
        navigate("/");
    };

    if (loading) {
        return <Loading />;
    }

    return (
        <UsersTable
            headers={headers}
            users={data}
            onCancel={handleCancel}
        />
    );
};

export default UsersList;
