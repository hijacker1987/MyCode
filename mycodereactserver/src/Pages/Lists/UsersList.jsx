import { jwtDecode } from "jwt-decode";
import { getAllUsers } from "../../Services/Backend.Endpoints";
import GenericList from "./GenericList";
import Cookies from "js-cookie";

const UsersList = () => {
    const headers = ["Counter", "Display Name", "Last Time Logged in", "User Name", "E-mail address", "Phone Number", "Modify"];
    const token = Cookies.get("jwtToken");
    const decodedToken = jwtDecode(token);
    const role = decodedToken["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] || [];

    return <GenericList
        endpoint={getAllUsers}
        headers={headers}
        role={role}
        type="users"
    />;
};

export default UsersList;
