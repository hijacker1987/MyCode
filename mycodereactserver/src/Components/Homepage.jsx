import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom"
import { getCodesByVisibility } from "../Services/Backend.Endpoints";
import { getApi, handleResponse } from "../Services/Api";
import { useUser } from "../Services/UserContext";
import { MidContainer } from "./Styles/TextContainer.styled";
import ErrorPage from "../Pages/Services/ErrorPage";

const Homepage = () => {
    const navigate = useNavigate();
    const { userData, setUserData } = useUser();
    const { role, userid } = userData;
    const [visibleCodes, setVisibleCodes] = useState([]);
    const [randomCodeIndex, setRandomCodeIndex] = useState(null);
    const [errorMessage, setError] = useState("");

    useEffect(() => {
        const fetchVisibleCodes = async () => {
            try {
                const responseData = await getApi(getCodesByVisibility);

                if (responseData === "Unauthorized") {
                    handleResponse(responseData, navigate, setUserData);
                } else {
                    setVisibleCodes(responseData);
                }
            } catch (error) {

                setError(`Error fetching visible codes: ${error}`);
            }
        };

        fetchVisibleCodes();
    }, []);

    useEffect(() => {
        const intervalId = setInterval(() => {
            if (visibleCodes.length > 0) {
                const newIndex = Math.floor(Math.random() * visibleCodes.length);
                setRandomCodeIndex(newIndex);
            }
        }, 30000);

        return () => clearInterval(intervalId);
    }, [visibleCodes]);

    useEffect(() => {
        const initialRandomCode = setTimeout(() => {
            if (visibleCodes.length > 0) {
                const newIndex = Math.floor(Math.random() * visibleCodes.length);
                setRandomCodeIndex(newIndex);
            }
        }, 0);

        return () => clearTimeout(initialRandomCode);
    }, [visibleCodes]);

    return (
        <div>
            {errorMessage === "" ? (
                <div>
                {!role && !userid ? (
                    <MidContainer className="welcome-text">
                        Welcome Code Fanatic!
                        <p>Please login or register</p>
                    </MidContainer>  
                ) : (
                    randomCodeIndex !== null && visibleCodes.length > 0 && (
                        <MidContainer className="random-code">
                            Random Code of <p>{visibleCodes[randomCodeIndex].displayName}</p>
                            <div>Title: {visibleCodes[randomCodeIndex].codeTitle}</div>
                            <div>Code: {visibleCodes[randomCodeIndex].myCode}</div>
                        </MidContainer>
                    )
                    )}
                </div>
            ) : (
            <ErrorPage errorMessage={errorMessage} />
            )}
        </div>
    );
};

export default Homepage;
