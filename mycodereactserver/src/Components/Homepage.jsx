import React, { useState, useEffect } from "react";
import { getCodesByVisibility } from "../Services/Backend.Endpoints";
import { getToken } from "../Services/AuthService";
import { getApi } from "../Services/Api";
import { MidContainer } from "./Styles/TextContainer.styled";
import ErrorPage from "../Pages/Services/ErrorPage";

const Homepage = () => {
    const [visibleCodes, setVisibleCodes] = useState([]);
    const [randomCodeIndex, setRandomCodeIndex] = useState(null);
    const [jwtToken, setJwtToken] = useState(getToken());
    const [errorMessage, setError] = useState("");

    useEffect(() => {
        const fetchVisibleCodes = async () => {
            try {
                if (jwtToken) {
                    const responseData = await getApi(jwtToken, getCodesByVisibility);
                    setVisibleCodes(responseData);
                }
            } catch (error) {
                setError(`Error fetching visible codes: ${error}`);
            }
        };

        fetchVisibleCodes();
    }, [jwtToken]);

    useEffect(() => {
        const intervalId = setInterval(() => {
            if (visibleCodes.length > 0) {
                const newIndex = Math.floor(Math.random() * visibleCodes.length);
                setRandomCodeIndex(newIndex);
            }
        }, 30000);

        return () => clearInterval(intervalId);
    }, [jwtToken, visibleCodes]);

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
                {!jwtToken ? (
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
