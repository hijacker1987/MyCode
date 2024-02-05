import { backendUrl } from "../Services/Config";
import { handleLogout } from "../Services/AuthService";

export const getApi = async (token, endpoint) => {
    const response = await fetch(`${backendUrl}${endpoint}`, {
        method: "GET",
        headers: {
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}`,
        },
    });

    if (response.status === 401) {
        handleUnauthorized();
        return null;
    }

    const data = await response.json();
    return data;
};

export const postApi = async (user, token, endpoint) => {
    const response = await fetch(`${backendUrl}${endpoint}`, {
        method: "POST",
        headers: {
            'Content-Type': "application/json",
            "Authorization": `Bearer ${token}`,
        },
        body: JSON.stringify(user),
    });

    if (response.status === 401) {
        handleUnauthorized();
        return null;
    }

    const data = await response.json();
    return data;
};

export const postApiV2 = async (user, endpoint) => {
    const response = await fetch(`${backendUrl}${endpoint}`, {
        method: "POST",
        headers: {
            'Content-Type': "application/json",
        },
        body: JSON.stringify(user),
    });

    const data = await response.json();
    return data;
};


export const patchApi = async (user, token, endpoint) => {
    const response = await fetch(`${backendUrl}${endpoint}`, {
        method: "PATCH",
        headers: {
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}`
        },
        body: JSON.stringify(user)
    });

    if (response.status === 401) {
        handleUnauthorized();
        return null;
    }

    const data = await response.json();
    return data;
};

export const putApi = async (user, token, endpoint) => {
    const response = await fetch(`${backendUrl}${endpoint}`, {
        method: "PUT",
        headers: {
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}`
        },
        body: JSON.stringify(user)
    });

    if (response.status === 401) {
        handleUnauthorized();
        return null;
    }

    const data = await response.json();
    return data;
};

export const deleteApi = async (token, endpoint) => {
    const response = await fetch(`${backendUrl}${endpoint}`, {
        method: "DELETE",
        headers: {
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}`,
        },
    });

    if (response.status === 401) {
        handleUnauthorized();
        return null;
    }

    return response.status;
};

const handleUnauthorized = () => {
    handleLogout("Error", "Session has expired. Please log in again.");
};
