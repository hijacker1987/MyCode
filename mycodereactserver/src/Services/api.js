import { backendUrl } from '../Services/config';

export const postApi = async (user, endpoint) => {
    try {
        const response = await fetch(`${backendUrl}${endpoint}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(user),
        });

        if (!response.ok) {
            throw new Error(`HTTP error! Status: ${response.status}`);
        }

        const data = await response.json();

        return data;
    } catch (error) {
        console.error(`Error occurred during ${endpoint} request:`, error);
        throw error;
    }
};

export const patchApi = async (user, token, endpoint) => {
    try {
        const response = await fetch(`${backendUrl}${endpoint}`, {
            method: "PATCH",
            headers: {
                "Content-Type": "application/json",
                "Authorization": `Bearer ${token}`
            },
            body: JSON.stringify(user)
        });

        if (!response.ok) {
            throw new Error(`HTTP error! Status: ${response.status}`);
        }

        const data = await response.json();

        return data;
    } catch (error) {
        console.error(`Error occurred during ${endpoint} request:`, error);
        throw error;
    }
};
