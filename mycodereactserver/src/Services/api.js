import { backendUrl } from "../Services/Config";
import { homePage } from "../Services/Frontend.Endpoints";
import Notify from "../Pages/Services/ToastNotifications";

export const getApi = async (endpoint) => {
    const response = await fetch(`${backendUrl}${endpoint}`, {
        method: "GET",
        headers: {
            "Content-Type": "application/json",
        },
        credentials: "include",
    });
    if (response.status === 400 || response.status === 401) {
        return "Unauthorized";
    } else {
        const data = await response.json();
        return data;
    }
};

export const postApi = async (endpoint, user) => {
    const response = await fetch(`${backendUrl}${endpoint}`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        credentials: "include",
        body: JSON.stringify(user),
    });
    if (response.status === 400 || response.status === 401) {
        return "Unauthorized";
    } else {
        const data = await response.json();
        return data;
    }
};

export const patchApi = async (endpoint, user) => {
    const response = await fetch(`${backendUrl}${endpoint}`, {
        method: "PATCH",
        headers: {
            "Content-Type": "application/json",
        },
        credentials: "include",
        body: JSON.stringify(user)
    });
    if (response.status === 400 || response.status === 401) {
        return "Unauthorized";
    } else {
        const data = await response.json();
        return data;
    }
};

export const putApi = async (endpoint, user) => {
    const response = await fetch(`${backendUrl}${endpoint}`, {
        method: "PUT",
        headers: {
            "Content-Type": "application/json",
        },
        credentials: "include",
        body: JSON.stringify(user)
    });
    if (response.status === 400 || response.status === 401) {
        return "Unauthorized";
    } else {
        const data = await response.json();
        return data;
    }
};

export const deleteApi = async (endpoint) => {
    const response = await fetch(`${backendUrl}${endpoint}`, {
        method: "DELETE",
        headers: {
            "Content-Type": "application/json",
        },
        credentials: "include",
    });
    if (response.status === 400 || response.status === 401) {
        return "Unauthorized";
    } else {
        return response.status;
    }
};

export const handleResponse = async (response, navigate, setUserData) => {
    if (response === "Unauthorized") {
        setUserData(null);
        Notify("Error", "Session has expired. Please log in again.");
        navigate(homePage);
    } else {
        console.log(response);
    }
};
