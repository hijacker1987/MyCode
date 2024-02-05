import React from "react";
import { getUserRoles } from "../../Services/AuthService";
import { getCodesByUser, getAllCodes, getCodesByVisibility } from "../../Services/Backend.Endpoints";
import GenericList from "./GenericList";

const CodesList = ({ type }) => {
    const role = getUserRoles();
    const auth = role === "Admin" ? "byVis" : "byAuth";
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
    let headers = ["Counter", ...(type === "byVis" ? ["Display name"] : []), "Code Title", "The Code itself", "What kind of code",
        "Back or Front", ...(kind !== "visible Codes" ? ["Is it visible to others?"] : []), ...(type === "byAuth" ? ["Modify"] : [])];

    return <GenericList
        endpoint={endpoint}
        headers={headers}
        role={role}
        type="codes"
        auth={auth}
        kind={kind}
    />;
};

export default CodesList;
