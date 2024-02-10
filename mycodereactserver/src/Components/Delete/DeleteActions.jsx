import { deleteApi } from "../../Services/Api";

const DeleteActions = {
    deleteRecord: async (endpoint, onSuccess, onError) => {
        try {
            await deleteApi(endpoint);
            onSuccess();
        } catch (error) {
            console.error("Error occurred during delete: ", error);
            onError();
        }
    },
};

export default DeleteActions;
