import React from "react";
import { getUserRoles } from "../../Services/AuthService";
import { getCodesByUser, getAllCodes, getCodesByVisibility } from "../../Services/Backend.Endpoints";
import GenericList from "./GenericList";

const CodesList = ({ type }) => {
    let headers = ["Counter", "Code Title", "The Code itself", "What kind of code", "Back or Front", "Is it visible to others?"];
    const role = getUserRoles();
    let endpoint = "";

    if (type === "byAuth") {
        endpoint = role === "Admin" ? getAllCodes : getCodesByUser;
        headers.push("Modify");
    }
    if (type === "byVis") {
        endpoint = getCodesByVisibility;
    }

    return <GenericList
        endpoint={endpoint}
        headers={headers}
        role={role}
        type="codes"
        auth={type}
    />;
};

export default CodesList;
