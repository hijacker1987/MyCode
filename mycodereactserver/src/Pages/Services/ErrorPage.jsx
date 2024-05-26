import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";

import { homePage } from "../../Services/Frontend.Endpoints";

import { Form, FormRow } from "../../Components/Styles/Forms.styled";
import { ColumnTextWrapper } from "../../Components/Styles/Containers/Wrappers.styled";
import { ErrorTextContainerRed, ErrorTextContainerWhite } from "../../Components/Styles/Containers/ComplexContainers.styled";

const ErrorPage = ({ errorMessage }) => {
    const navigate = useNavigate();
    const [countdown, setCountdown] = useState(5);

    useEffect(() => {
        const countdownInterval = setInterval(() => {
            setCountdown(prevCountdown => prevCountdown - 1);
        }, 1000);

        return () => {
            clearInterval(countdownInterval);
        };
    }, []);

    useEffect(() => {
        if (countdown === 0) {
            navigate(homePage);
        }
    }, [countdown, navigate]);

    return (
        <Form style={{ display: "flex", alignItems: "center", justifyContent: "flex-end" }}>
            <FormRow>
                <ColumnTextWrapper>
                    <ErrorTextContainerRed>
                        {errorMessage}
                    </ErrorTextContainerRed>
                    <ErrorTextContainerWhite>
                        Redirecting to the Home page in {countdown} seconds
                    </ErrorTextContainerWhite>
                </ColumnTextWrapper>
            </FormRow>
        </Form>
    );
};

export default ErrorPage;
