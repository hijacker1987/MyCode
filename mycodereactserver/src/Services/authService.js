import { jwtDecode } from "jwt-decode";
import Notify from "../Pages/Services/ToastNotifications";
import Cookies from "js-cookie";

export const getToken = () => {
    const token = Cookies.get("jwtToken");
    return token;
};

export const getUserRoles = () => {
    const token = getToken();

    if (!token) {
        handleLogout("Error", "Session has expired. Please log in again.");
        return [];
    }

    try {
        const decodedToken = jwtDecode(token);
        return decodedToken["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] || [];
    } catch (error) {
        handleLogout("Error", "Invalid token. Please log in again.");
        return [];
    }
};

export const getUserIdFromToken = () => {
    const token = getToken();
    const decodedToken = jwtDecode(token);
    return decodedToken["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"] || null;
};

export const handleLogout = (msg) => {
    window.location.reload();
    Cookies.remove("jwtToken");
    Cookies.remove("refreshToken");
    Notify(msg);
};
