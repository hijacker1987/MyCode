import { useState, useEffect } from "react";
import { useParams } from 'react-router-dom';
import { getApi } from "../../Services/Api";
import { getToken } from "../../Services/AuthService";
import { toast } from "react-toastify";
import UsersTable from "../../Components/Lists/UsersTable/UsersTable";
import CodesTable from "../../Components/Lists/CodesTable/CodesTable";
import Loading from "../../Components/Loading/Loading";
import ErrorPage from "../Services/ErrorPage";
import "react-toastify/dist/ReactToastify.css";

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
                    toast.success(`List of ${kind} created successfully!`, {
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
                    toast.error(`Unable to create the List of ${kind}`, {
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
