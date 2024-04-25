import React, { useState, useEffect } from "react";

import { ButtonContainer } from "../../Styles/ButtonContainer.styled";
import { InputForm } from "../../Styles/Input.styled";
import { FormColumn } from "../../Styles/Form.styled";

import "../../Styles/CustomCss/google.css";
import "../../Styles/CustomCss/fb.css";

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
        <FormColumn onSubmit={handleSubmit}>
            {!isReliableEnabled && (
                <>
                    <h3>Please provide Your trustworthy e-mail (like Gmail) here, add an external account to prove it:</h3>
                    <button class="gsi-material-button" onClick={() => handleOnExternalLogin("Google")} style={{ width: "120px", marginLeft: "45%" }}>
                        <div class="gsi-material-button-content-wrapper">
                            <span class="gsi-material-button-icon" style={{ marginLeft: "-13%" }}></span>
                            <span class="gsi-material-button-contents">Google</span>
                        </div>
                    </button>
                    <button class="loginBtn--facebook" onClick={() => handleOnExternalLogin("Facebook")} style={{ width: "120px", height: "40px", marginLeft: "45%" }}>
                        <div class="gsi-material-button-content-wrapper">
                            <span class="loginBtn--facebook-icon" style={{ marginLeft: "-13%" }}></span>
                            <span class="gsi-material-button-contents" style={{ marginLeft: "2%", color: "aliceblue" }}>acebook</span>
                        </div>
                    </button>
                    <button type="button" onClick={() => handleOnExternalLogin("GitHub")} style={{ width: "120px", height: "40px", marginLeft: "45%", cursor: "pointer" }} class="py-2 px-4 max-w-md flex justify-center items-center bg-gray-600 hover:bg-gray-700 focus:ring-gray-500 focus:ring-offset-gray-200 text-white w-full transition ease-in duration-200 text-center text-base font-semibold shadow-md focus:outline-none focus:ring-2 focus:ring-offset-2 rounded-lg">
                        <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" fill="currentColor" class="mr-2" viewBox="0 0 1792 1792">
                            <path d="M896 128q209 0 385.5 103t279.5 279.5 103 385.5q0 251-146.5 451.5t-378.5 277.5q-27 5-40-7t-13-30q0-3 .5-76.5t.5-134.5q0-97-52-142 57-6 102.5-18t94-39 81-66.5 53-105 20.5-150.5q0-119-79-206 37-91-8-204-28-9-81 11t-92 44l-38 24q-93-26-192-26t-192 26q-16-11-42.5-27t-83.5-38.5-85-13.5q-45 113-8 204-79 87-79 206 0 85 20.5 150t52.5 105 80.5 67 94 39 102.5 18q-39 36-49 103-21 10-45 15t-57 5-65.5-21.5-55.5-62.5q-19-32-48.5-52t-49.5-24l-20-3q-21 0-29 4.5t-5 11.5 9 14 13 12l7 5q22 10 43.5 38t31.5 51l10 23q13 38 44 61.5t67 30 69.5 7 55.5-3.5l23-4q0 38 .5 88.5t.5 54.5q0 18-13 30t-40 7q-232-77-378.5-277.5t-146.5-451.5q0-209 103-385.5t279.5-279.5 385.5-103zm-477 1103q3-7-7-12-10-3-13 2-3 7 7 12 9 6 13-2zm31 34q7-5-2-16-10-9-16-3-7 5 2 16 10 10 16 3zm30 45q9-7 0-19-8-13-17-6-9 5 0 18t17 7zm42 42q8-8-4-19-12-12-20-3-9 8 4 19 12 12 20 3zm57 25q3-11-13-16-15-4-19 7t13 15q15 6 19-6zm63 5q0-13-17-11-16 0-16 11 0 13 17 11 16 0 16-11zm58-10q-2-11-18-9-16 3-14 15t18 8 14-14z"></path>
                        </svg>
                        GitHub
                    </button>
                </>
            )}
            {isReliableEnabled && !isDisableEnabled && (
                <>
                    <h3>Press the button if You would like to continue the authentication</h3>
                    <ButtonContainer type="button" onClick={handleEnable}>Enable Verification</ButtonContainer>
                </>
            )}
            {isReliableEnabled && isCodeSubmitEnabled && (
                <>
                    <h3>
                        Please type Your unique authentication code here (You received it to Your thrustworty e-mail account):
                        <InputForm type="text" value={code} onChange={(e) => setCode(e.target.value)} />
                    </h3>
                    <ButtonContainer type="submit" onClick={handleSubmit}>Send</ButtonContainer>
                </>
            )}
            {isReliableEnabled && isDisableEnabled && (
                <>
                    <h3>Press the button if You would like to revoke Your multi-factor authentication</h3>
                    <ButtonContainer type="button" onClick={handleDisable}>Disable Verification</ButtonContainer>
                </>
            )}
            <ButtonContainer type="button" onClick={onCancel}>Cancel</ButtonContainer>
        </FormColumn>
    );
}

export default TwoFactorAuthenticationForm;
