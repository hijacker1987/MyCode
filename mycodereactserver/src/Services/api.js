import { backendUrl } from "../Services/Config";

export const getApi = async (endpoint) => {
    const response = await fetch(`${backendUrl}${endpoint}`, {
        method: "GET",
        headers: {
            "Content-Type": "application/json",
        },
        credentials: "include",
    });

    const data = await response.json();
    return data;
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

    const data = await response.json();
    return data;
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

    const data = await response.json();
    return data;
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

    const data = await response.json();
    return data;
};

export const deleteApi = async (endpoint) => {
    const response = await fetch(`${backendUrl}${endpoint}`, {
        method: "DELETE",
        headers: {
            "Content-Type": "application/json",
        },
        credentials: "include",
    });

    return response.status;
};
