import { jwtDecode } from "jwt-decode";
import Cookies from "js-cookie";
import GenericList from "./GenericList";
import { getCodesByUser, getAllCodes } from "../../Services/Backend.Endpoints";

const CodesList = () => {
    const headers = ["Counter", "Code Title", "The Code itself", "What kind of code", "Back or Front", "Is it visible to others?", "Modify"];
    const token = Cookies.get("jwtToken");
    const decodedToken = jwtDecode(token);
    const role = decodedToken["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] || [];

    const endpoint = role === "Admin" ? getAllCodes : getCodesByUser;

    return <GenericList
        endpoint={endpoint}
        headers={headers}
        type="codes"
    />;
};

export default CodesList;
