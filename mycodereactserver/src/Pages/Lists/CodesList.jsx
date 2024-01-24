import GenericList from "./GenericList";
import { getAllCodes } from "../../Services/Backend.Endpoints";

const CodesList = () => {
    const headers = ["Counter", "Code Title", "The Code itself", "What kind of code", "Back or Front", "Is it visible to others?", "Modify"];
    return <GenericList
        endpoint={getAllCodes}
        headers={headers}
        type="codes"
    />;
};

export default CodesList;
