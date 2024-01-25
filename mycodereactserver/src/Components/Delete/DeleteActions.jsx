import Cookies from "js-cookie";
import { deleteApi } from "../../Services/Api";

const DeleteActions = {
    deleteRecord: async (endpoint, onSuccess, onError) => {
        try {
            const token = Cookies.get("jwtToken");
            await deleteApi(token, endpoint);
            onSuccess(); // Trigger success callback
        } catch (error) {
            console.error("Error occurred during delete: ", error);
            onError(); // Trigger error callback
        }
    },
};

export default DeleteActions;
