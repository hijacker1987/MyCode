import { getToken } from "../../Services/AuthService";
import { deleteApi } from "../../Services/Api";

const DeleteActions = {
    deleteRecord: async (endpoint, onSuccess, onError) => {
        try {
            const token = getToken();
            await deleteApi(token, endpoint);
            onSuccess();
        } catch (error) {
            console.error("Error occurred during delete: ", error);
            onError();
        }
    },
};

export default DeleteActions;
