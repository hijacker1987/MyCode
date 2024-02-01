import { useNavigate } from "react-router-dom";
import { jwtDecode } from "jwt-decode";
import { uLogin } from "../Services/Frontend.Endpoints";
import Notify from "../Pages/Services/ToastNotifications";
import Cookies from "js-cookie";

export const getToken = () => {
    const token = Cookies.get("jwtToken");
    return token;
};

export const getUserRoles = () => {
    const token = getToken();
    if (typeof token !== 'string') {
        handleLogout();
        Notify("Error", "Session has expired. Please log in again.");
    }
    const decodedToken = jwtDecode(token);
    return decodedToken["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] || [];
};

export const getUserIdFromToken = () => {
    const token = getToken();
    const decodedToken = jwtDecode(token);
    return decodedToken["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"] || null;
};

export const checkTokenExpiration = () => {
    const token = getToken();
    if (token) {
        try {
            const decodedToken = jwtDecode(token);
            const expirationTime = decodedToken.exp * 1000;

            if (isNaN(expirationTime) || expirationTime < Date.now()) {
                handleLogout();
                Notify("Error", "Session has expired. Please log in again.");
            }
        } catch (error) {
            handleLogout();
            Notify("Error", "Invalid token. Please log in again.");
        }
    } else {
        handleLogout();
        Notify("Error", "Missing token. Please log in.");
    }
};

export const handleLogout = () => {
    const navigate = useNavigate();
    Cookies.remove("jwtToken");
    navigate(uLogin);
};
