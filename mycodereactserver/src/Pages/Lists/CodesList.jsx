import React from "react";

import { useUser } from "../../Services/UserContext";
import { getCodesByUser, getAllCodes, getCodesByVisibility } from "../../Services/Backend.Endpoints";
import { GenericList } from "./index";

export const CodesList = ({ type }) => {
    const { userData } = useUser();
    const { role } = userData;
    const auth = (role === "Admin" || role === "Support") ? "byAuth" : "byVis";

    let endpoint = "";
    let kind = "";

    if (type === "byAuth") {
        endpoint = (role === "Admin" || role === "Support") ? getAllCodes : getCodesByUser;
        kind = (role === "Admin" || role === "Support") ? "Codes" : "your Codes";
    }
    if (type === "byVis") {
        endpoint = getCodesByVisibility;
        kind = "visible Codes";
    }
    let headers = ["Counter", ...(type === "byVis" ? ["Display name"] : []), "Code Title", "The Code itself", "What kind of code",
        "Back or Front", ...(kind !== "visible Codes" ? ["Is it visible to others?"] : []), ...(type === "byAuth" && role === "Admin" ? ["Modify"] : [])];

    return <GenericList
        endpoint={endpoint}
        headers={headers}
        role={role}
        type="codes"
        auth={auth}
        kind={kind}
    />;
};
