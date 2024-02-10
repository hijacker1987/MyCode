import { useState, useEffect } from "react";
import { useParams } from "react-router-dom";
import { getApi } from "../../Services/Api";
import UsersTable from "../../Components/Lists/UsersTable/UsersTable";
import CodesTable from "../../Components/Lists/CodesTable/CodesTable";
import Loading from "../../Components/Loading/Loading";
import Notify from "../Services/ToastNotifications";

const GenericList = ({ endpoint, headers, role, type, auth, kind }) => {
    const { page } = useParams();
    const [loading, setLoading] = useState(false);
    const [errorNotify, setErrorNotify] = useState(false);
    const [data, setData] = useState(null);

    useEffect(() => {
        const fetchData = async () => {
            setLoading(true);

            try {
                const responseData = await getApi(endpoint);
                setLoading(false);

                if (responseData) {
                    setData(responseData);
                    setErrorNotify(false);
                }
            } catch (error) {
                setLoading(false);
                setErrorNotify(true);
            }
        };

        fetchData();

        setTimeout(() => {
            if (errorNotify) {
                Notify("Error", `Unable to create the List of ${kind}`);
            } else {
                Notify("Success", `List of ${kind} created successfully!`);
            }
        }, 0);
    }, [endpoint, errorNotify]);


    if (loading) {
        return <Loading />;
    }

    return (
        <div>
            {type === "codes" ? <CodesTable codes={data} headers={headers} role={role} page={page} auth={auth} kind={kind} />
                              : <UsersTable users={data} headers={headers} role={role} page={page} />}
        </div>
    );
};

export default GenericList;
