import { useState, useEffect } from "react";
import { useParams, useNavigate } from "react-router-dom";

import { Notify } from "../Services";
import { useUser } from "../../Services/UserContext";
import { getApi, handleResponse } from "../../Services/Api";
import UsersTable from "../../Components/Lists/UsersTable/index";
import CodesTable from "../../Components/Lists/CodesTable/index";
import Loading from "../../Components/Loading/index";

export const GenericList = ({ endpoint, headers, role, type, auth, kind }) => {
    const navigate = useNavigate();
    const { page } = useParams();
    const { setUserData } = useUser();
    const [data, setData] = useState(null);
    const [loading, setLoading] = useState(false);
    const [errorNotify, setErrorNotify] = useState(false);

    useEffect(() => {
        const fetchData = async () => {
            setLoading(true);

            try {
                const responseData = await getApi(endpoint);

                setLoading(false);

                if (responseData === "Unauthorized") {
                    handleResponse(responseData, navigate, setUserData);
                } else if (responseData !== "Unauthorized") {
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
