import { backendUrl } from '../Services/config'

export const postApi = async (user, endpoint) => {
    try {
        const response = await fetch(`${backendUrl}${endpoint}`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(user),
        });

        const data = await response.json();

        return data;
    } catch (error) {
        console.error(`Error occurred during ${endpoint} request:`, error);
        throw error;
    }
};
