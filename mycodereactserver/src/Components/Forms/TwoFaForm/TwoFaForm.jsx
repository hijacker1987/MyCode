import React, { useState, useEffect } from "react";

import { InputForm } from "../../Styles/Forms.styled";
import { VerHeading3 } from "../../Styles/InputOutput.styled";
import { StyledButton } from "../../Styles/Buttons/InternalButtons.styled";
import { VerificationColumnContainer } from "../../Styles/Containers/Containers.styled";
import { ButtonItemWrapper, ButtonContentsWrapper, ButtonVerContainerWrapper, ButtonVerColumnContainerWrapper } from "../../Styles/Containers/Wrappers.styled";
import { FacebookButton, FacebookIcon,
         GitHubButton, GitHubIcon,
         GoogleButton, GoogleIcon
        } from "../../Styles/Buttons/ExternalButtons.styled";

function TwoFactorAuthenticationForm({ onEnable, onSubmit, onSubmitAddress, onDisable, onCancel, isEmailConfirmed, isTwoFactorEnabled, isReliable }) {
    const [code, setCode] = useState("");
    const [isCodeSubmitEnabled, setIsCodeSubmitEnabled] = useState(false);
    const [isDisableEnabled, setIsDisableEnabled] = useState(false);
    const [isReliableEnabled, setIsReliableEnabled] = useState(false);

    useEffect(() => {
        if (!isEmailConfirmed && isTwoFactorEnabled) {
            setIsCodeSubmitEnabled(true);
        } else {
            setIsCodeSubmitEnabled(false);
        }

        if (isTwoFactorEnabled) {
            setIsDisableEnabled(true);
        } else {
            setIsDisableEnabled(false);
        }

        if (isReliable) {
            setIsReliableEnabled(true);
        } else {
            setIsReliableEnabled(false);
        }
    }, [isEmailConfirmed, isTwoFactorEnabled, isReliable]);

    const handleEnable = async () => {
        try {
            await onEnable();
        } catch (error) {
            console.error("Unsuccessful engage 2fa", error);
        }
    }

    const handleSubmit = async (e) => {
        e.preventDefault();
        try {
            await onSubmit(code);
        } catch (error) {
            console.error("Unsuccessful 2fa", error);
        }
    };

    const handleDisable = async () => {
        try {
            await onDisable();
        } catch (error) {
            console.error("Unsuccessful engage 2fa", error);
        }
    }

    const handleOnExternalLogin = async (ext) => {
        try {
            await onSubmitAddress(ext);
        } catch (error) {
            console.error(`Error occurred during login: ${error}`);
        }
    };

    return (
        <VerificationColumnContainer onSubmit={handleSubmit}>
            {!isReliableEnabled && (
                <>
                    <VerHeading3>Please provide Your trustworthy e-mail (like Gmail) here, add an external account to prove it:</VerHeading3>
                    <ButtonVerColumnContainerWrapper>
                        <GoogleButton onClick={() => handleOnExternalLogin("Google")}>
                            <ButtonItemWrapper>
                                <GoogleIcon />
                                <ButtonContentsWrapper>oogle</ButtonContentsWrapper>
                            </ButtonItemWrapper>
                        </GoogleButton>

                        <FacebookButton onClick={() => handleOnExternalLogin("Facebook")}>
                            <ButtonItemWrapper>
                                <FacebookIcon />
                                <ButtonContentsWrapper>acebook</ButtonContentsWrapper>
                            </ButtonItemWrapper>
                        </FacebookButton>

                        <GitHubButton onClick={() => handleOnExternalLogin("GitHub")}>
                            <GitHubIcon xmlns="http://www.w3.org/2000/svg" viewBox="0 0 1792 1792">
                                <path d="M896 128q209 0 385.5 103t279.5 279.5 103 385.5q0 251-146.5 451.5t-378.5 277.5q-27 5-40-7t-13-30q0-3 .5-76.5t.5-134.5q0-97-52-142 57-6 102.5-18t94-39 81-66.5 53-105 20.5-150.5q0-119-79-206 37-91-8-204-28-9-81 11t-92 44l-38 24q-93-26-192-26t-192 26q-16-11-42.5-27t-83.5-38.5-85-13.5q-45 113-8 204-79 87-79 206 0 85 20.5 150t52.5 105 80.5 67 94 39 102.5 18q-39 36-49 103-21 10-45 15t-57 5-65.5-21.5-55.5-62.5q-19-32-48.5-52t-49.5-24l-20-3q-21 0-29 4.5t-5 11.5 9 14 13 12l7 5q22 10 43.5 38t31.5 51l10 23q13 38 44 61.5t67 30 69.5 7 55.5-3.5l23-4q0 38 .5 88.5t.5 54.5q0 18-13 30t-40 7q-232-77-378.5-277.5t-146.5-451.5q0-209 103-385.5t279.5-279.5 385.5-103zm-477 1103q3-7-7-12-10-3-13 2-3 7 7 12 9 6 13-2zm31 34q7-5-2-16-10-9-16-3-7 5 2 16 10 10 16 3zm30 45q9-7 0-19-8-13-17-6-9 5 0 18t17 7zm42 42q8-8-4-19-12-12-20-3-9 8 4 19 12 12 20 3zm57 25q3-11-13-16-15-4-19 7t13 15q15 6 19-6zm63 5q0-13-17-11-16 0-16 11 0 13 17 11 16 0 16-11zm58-10q-2-11-18-9-16 3-14 15t18 8 14-14z"></path>
                            </GitHubIcon>
                            GitHub
                        </GitHubButton>
                    </ButtonVerColumnContainerWrapper>
                </>
            )}
            {isReliableEnabled && !isDisableEnabled && (
                <>
                    <VerHeading3>Press the button if You would like to continue the authentication</VerHeading3>
                    <ButtonItemWrapper>
                        <StyledButton type="button" onClick={handleEnable}>Enable Verification</StyledButton>
                    </ButtonItemWrapper>
                </>
            )}
            {isReliableEnabled && isCodeSubmitEnabled && (
                <>
                    <VerHeading3>
                        Please type Your unique authentication code here (You received it to Your thrustworty e-mail account):
                        <InputForm type="text" value={code} onChange={(e) => setCode(e.target.value)} />
                        <ButtonVerContainerWrapper>
                            <StyledButton type="submit" onClick={handleSubmit}>Send</StyledButton>
                        </ButtonVerContainerWrapper>
                    </VerHeading3>
                </>
            )}
            {isReliableEnabled && isDisableEnabled && (
                <>
                    <VerHeading3>Press the button if You would like to revoke Your multi-factor authentication</VerHeading3>
                    <ButtonItemWrapper>
                        <StyledButton type="button" onClick={handleDisable}>Disable Verification</StyledButton>
                    </ButtonItemWrapper>
                </>
            )}
            <ButtonItemWrapper>
                <StyledButton type="button" onClick={onCancel}>Cancel</StyledButton>
            </ButtonItemWrapper>
        </VerificationColumnContainer>
    );
}

export default TwoFactorAuthenticationForm;
