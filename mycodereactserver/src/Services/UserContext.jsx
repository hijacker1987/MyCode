import React, { createContext, useContext, useState } from "react";

import { revoke } from "./Backend.Endpoints";
import { deleteApi } from "./Api";

const UserContext = createContext();

export const UserProvider = ({ children }) => {
    const [userData, setUserData] = useState({ role: "", userid: "", username: "" });

    const updateUserData = (role, userid, username) => {
        setUserData({ role, userid, username });
    };

    return (
        <UserContext.Provider value={{ userData, setUserData: updateUserData }}>
            {children}
        </UserContext.Provider>
    );
};

export const useUser = () => useContext(UserContext);

export const logoutUser = async () => {
    try {
        await deleteApi(revoke);
    } catch (error) {
        console.error("Error occurred during delete: ", error);
    }
}
