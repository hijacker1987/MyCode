import { useState, useEffect } from "react";
import { useParams } from 'react-router-dom';
import { getApi } from "../../Services/Api";
import { getToken } from "../../Services/AuthService";
import UsersTable from "../../Components/Lists/UsersTable/UsersTable";
import CodesTable from "../../Components/Lists/CodesTable/CodesTable";
import Loading from "../../Components/Loading/Loading";
import Notify from "../Services/ToastNotifications";
import ErrorPage from "../Services/ErrorPage";

const GenericList = ({ endpoint, headers, role, type, auth, kind }) => {
    const [loading, setLoading] = useState(false);
    const [errorMessage, setError] = useState("");
    const [data, setData] = useState(null);
    const { page } = useParams();

    useEffect(() => {
        const fetchData = async () => {
            setLoading(true);

            const token = getToken();
            try {
                const responseData = await getApi(token, endpoint);
                setLoading(false);

                if (responseData) {
                    setData(responseData);
                    Notify("Success", `List of ${kind} created successfully!`);
                } else {
                    Notify("Error", `Unable to create the List of ${kind}`);                  
                }
            } catch (error) {
                setLoading(false);
                setError(`Error occurred during API call on tables: ${error}`);
            }
        };

        fetchData();
    }, [endpoint]);

    if (loading) {
        return <Loading />;
    }

    return (
        <div>
            {errorMessage === "" ? (
                type === "codes" ? <CodesTable codes={data} headers={headers} role={role} page={page} type={auth} /> :
                                   <UsersTable users={data} headers={headers} role={role} page={page} />
            ) : (
                <ErrorPage errorMessage={errorMessage} />
            )}
        </div>
    );
};

export default GenericList;
