import GenericList from "./GenericList";
import { getAllUsers } from "../../Services/Backend.Endpoints";

const UsersList = () => {
    const headers = ["Counter", "Display Name", "Last Time Logged in", "User Name", "E-mail address", "Phone Number", "Modify"];
    return <GenericList
        endpoint={getAllUsers}
        headers={headers}
        type="users"
    />;
};

export default UsersList;
