import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { Form, FormRow } from "../../Components/Styles/Form.styled";
import { ErrorTextContainerRed, ErrorTextContainerWhite, ColumnTextWrapper } from "../../Components/Styles/TextContainer.styled";

const ErrorPage = ({ errorMessage }) => {
    const [countdown, setCountdown] = useState(5);
    const navigate = useNavigate();

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
            navigate("/");
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
