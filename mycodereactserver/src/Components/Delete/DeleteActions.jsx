import { deleteApi, handleResponse } from "../../Services/Api";

const DeleteActions = {
    deleteRecord: async (endpoint, onSuccess, onError, navigate, setUserData) => {
        try {
            const data = await deleteApi(endpoint);

            if (data === "Unauthorized") {
                handleResponse(data, navigate, setUserData);
            } else {
                onSuccess();
            }
        } catch (error) {
            console.error("Error occurred during delete: ", error);
            onError();
        }
    },
};

export default DeleteActions;
