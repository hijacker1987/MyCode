import { jwtDecode } from "jwt-decode";
import Cookies from "js-cookie";

export const getToken = () => {
    return Cookies.get("jwtToken");
};

export const getUserRoles = () => {
    const token = getToken();
    const decodedToken = jwtDecode(token);
    return decodedToken["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] || [];
};

export const getUserIdFromToken = () => {
    const token = getToken();
    const decodedToken = jwtDecode(token);
    return decodedToken["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"] || null;
};
