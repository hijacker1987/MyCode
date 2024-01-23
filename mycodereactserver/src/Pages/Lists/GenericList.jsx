import { useState, useEffect } from "react";
import { getApi } from "../../Services/Api";
import UsersTable from "../../Components/Lists/UsersTable/UsersTable";
import CodesTable from "../../Components/Lists/CodesTable/CodesTable";
import Loading from "../../Components/Loading/Loading";
import ErrorPage from "../Service/ErrorPage";
import Cookies from "js-cookie";

const GenericList = ({ endpoint, headers, type }) => {
    const [loading, setLoading] = useState(false);
    const [errorMessage, setError] = useState("");
    const [data, setData] = useState(null);

    useEffect(() => {
        const fetchData = async () => {
            setLoading(true);

            const token = getToken();
            try {
                const responseData = await getApi(token, endpoint);
                setLoading(false);

                if (responseData) {
                    setData(responseData);
                } else {
                    setError("Received empty data from the server.");
                }
            } catch (error) {
                setLoading(false);
                setError(`Error occurred during API call on tables: ${error}`);
            }
        };

        fetchData();
    }, [endpoint]);

    const getToken = () => {
        return Cookies.get("jwtToken");
    };

    if (loading) {
        return <Loading />;
    }

    return (
        <div>
            {errorMessage === "" ? (
                type === "codes" ? <CodesTable codes={data} headers={headers} /> :
                                   <UsersTable users={data} headers={headers} />
            ) : (
                <ErrorPage errorMessage={errorMessage} />
            )}
        </div>
    );
};

export default GenericList;
