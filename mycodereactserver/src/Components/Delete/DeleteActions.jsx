import { useNavigate } from "react-router-dom";
import { useUser } from "../../Services/UserContext";
import { deleteApi, handleResponse } from "../../Services/Api";

const DeleteActions = {
    deleteRecord: async (endpoint, onSuccess, onError) => {
        try {
            const navigate = useNavigate();
            const { setUserData } = useUser();
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
