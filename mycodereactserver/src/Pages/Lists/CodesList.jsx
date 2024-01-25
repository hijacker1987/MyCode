import React from "react";
import { jwtDecode } from "jwt-decode";
import { getCodesByUser, getAllCodes, getCodesByVisibility } from "../../Services/Backend.Endpoints";
import GenericList from "./GenericList";
import Cookies from "js-cookie";

const CodesList = ({ type }) => {
    let headers = ["Counter", "Code Title", "The Code itself", "What kind of code", "Back or Front", "Is it visible to others?"];
    const token = Cookies.get("jwtToken");
    const decodedToken = jwtDecode(token);
    const role = decodedToken["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] || [];
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
