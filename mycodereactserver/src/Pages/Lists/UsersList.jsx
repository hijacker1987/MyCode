import { getUserRoles } from "../../Services/AuthService";
import { getAllUsers } from "../../Services/Backend.Endpoints";
import GenericList from "./GenericList";

const UsersList = () => {
    const headers = ["Counter", "Display Name", "Last Time Logged in", "User Name", "E-mail address", "Phone Number", "Modify"];
    const role = getUserRoles();

    return <GenericList
        endpoint={getAllUsers}
        headers={headers}
        role={role}
        type="users"
        kind="Users"
    />;
};

export default UsersList;
