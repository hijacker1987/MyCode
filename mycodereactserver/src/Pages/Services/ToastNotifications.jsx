import { toast } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";

const Notify = ( notifyKind, notifyMessage ) => {
    if (notifyKind === "Success") {
        return toast.success(`${notifyMessage}`, {
                position: "top-right",
                autoClose: 5000,
                hideProgressBar: false,
                closeOnClick: true,
                pauseOnHover: true,
                draggable: true,
                progress: undefined,
                theme: "dark",
        });
    } else {
        return toast.error(`${notifyMessage}`, {
                position: "top-right",
                autoClose: 5000,
                hideProgressBar: false,
                closeOnClick: true,
                pauseOnHover: true,
                draggable: true,
                progress: undefined,
                theme: "dark"
        });
    }
};

export default Notify;
