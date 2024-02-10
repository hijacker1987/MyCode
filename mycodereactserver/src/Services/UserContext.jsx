import React, { createContext, useContext, useState } from "react";

const UserContext = createContext();

export const UserProvider = ({ children }) => {
    const [userData, setUserData] = useState({ role: "", userid: "" });

    const updateUserData = (role, userid) => {
        setUserData({ role, userid });
    };

    return (
        <UserContext.Provider value={{ userData, setUserData: updateUserData }}>
            {children}
        </UserContext.Provider>
    );
};

export const useUser = () => useContext(UserContext);
