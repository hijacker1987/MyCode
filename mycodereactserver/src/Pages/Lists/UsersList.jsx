import { useUser } from "../../Services/UserContext";
import { getAllUsers } from "../../Services/Backend.Endpoints";
import { GenericList } from "./index";

export const UsersList = () => {
    const headers = ["Counter", "Display Name", "Last Time Logged in", "User Name", "E-mail address", "Phone Number", "Role", "Modify"];
    const { userData } = useUser();
    const { role } = userData;

    return <GenericList
        endpoint={getAllUsers}
        headers={headers}
        role={role}
        type="users"
        kind="Users"
    />;
};
