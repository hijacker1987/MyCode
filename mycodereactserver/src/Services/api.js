import { backendUrl } from "../Services/Config";
import { checkTokenExpiration } from "../Services/AuthService";

export const getApi = async (token, endpoint) => {
    checkTokenExpiration(); 
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

export const postApi = async (user, token, endpoint) => {
    checkTokenExpiration(); 
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
    checkTokenExpiration(); 
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
    checkTokenExpiration(); 
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

export const deleteApi = async (token, endpoint) => {
    checkTokenExpiration(); 
    const response = await fetch(`${backendUrl}${endpoint}`, {
        method: "DELETE",
        headers: {
            "Content-Type": "application/json",
            "Authorization": `Bearer ${token}`,
        },
    });

    return response.status;
};
