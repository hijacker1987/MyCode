import React from "react";
import { getUserRoles } from "../../Services/AuthService";
import { getCodesByUser, getAllCodes, getCodesByVisibility } from "../../Services/Backend.Endpoints";
import GenericList from "./GenericList";

const CodesList = ({ type }) => {
    let headers = ["Counter", ...(type === "byVis" ? ["Username"] : []), "Code Title", "The Code itself", "What kind of code",
                    "Back or Front", "Is it visible to others?", ...(type === "byAuth" ? ["Modify"] : [])];
    const role = getUserRoles();
    let endpoint = "";
    let kind = "";

    if (type === "byAuth") {
        endpoint = role === "Admin" ? getAllCodes : getCodesByUser;
        kind = role === "Admin" ? "Codes" : "your Codes";
    }
    if (type === "byVis") {
        endpoint = getCodesByVisibility;
        kind = "visible Codes";
    }

    return <GenericList
        endpoint={endpoint}
        headers={headers}
        role={role}
        type="codes"
        auth={type}
        kind={kind}
    />;
};

export default CodesList;
