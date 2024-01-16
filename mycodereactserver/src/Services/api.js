import { backendUrl } from '../Services/config';

export const postApi = async (user, endpoint) => {
    const response = await fetch(`${backendUrl}${endpoint}`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(user),
    });

    return response.json();
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

    return response.json();
};
