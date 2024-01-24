import { backendUrl } from "../Services/config";

export const getApi = async (token, endpoint) => {
        const response = await fetch(`${backendUrl}${endpoint}`, {
            method: "GET",
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${token}`,
            },
        });

        const data = await response.json();
        return data;
};

export const postApi = async (user, endpoint) => {
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

export const postApiV2 = async (user, token, endpoint) => {
    const response = await fetch(`${backendUrl}${endpoint}`, {
        method: "POST",
        headers: {
            'Content-Type': "application/json",
            "Authorization": `Bearer ${token}`,
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

    const data = await response.json();
    return data;
};
