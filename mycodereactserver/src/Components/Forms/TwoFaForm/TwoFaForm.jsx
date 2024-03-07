import React, { useState, useEffect } from "react";

import { ButtonContainer } from "../../Styles/ButtonContainer.styled";
import { InputForm, InputWrapper } from "../../Styles/Input.styled";
import { Form, FormColumn } from "../../Styles/Form.styled";

function TwoFactorAuthenticationForm({ onEnable, onSubmit, onDisable, onCancel, isEmailConfirmed, isTwoFactorEnabled }) {
    const [code, setCode] = useState("");
    const [isCodeSubmitEnabled, setIsCodeSubmitEnabled] = useState(false);
    const [isDisableEnabled, setIsDisableEnabled] = useState(false);

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
    }, [isEmailConfirmed, isTwoFactorEnabled]);

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

    return (
        <FormColumn onSubmit={handleSubmit}>
            {!isDisableEnabled && (
                <>
                    <h3>Press the button if You would like to setup the two factor authentication</h3>
                    <ButtonContainer type="button" onClick={handleEnable}>Enable Verification</ButtonContainer>
                </>
            )}
            {isCodeSubmitEnabled && (
                <>
                    <h3>
                        Please write Your unique two factor authentication code here:
                        <InputForm type="text" value={code} onChange={(e) => setCode(e.target.value)} />
                    </h3>
                    <ButtonContainer type="submit" onClick={handleSubmit}>Send</ButtonContainer>
                </>
            )}
            {isDisableEnabled && (
                <>
                    <h3>Press the button if You would like to revoke the two factor authentication</h3>
                    <ButtonContainer type="button" onClick={handleDisable}>Disable Verification</ButtonContainer>
                </>
            )}
            <ButtonContainer type="button" onClick={onCancel}>Cancel</ButtonContainer>
        </FormColumn>
    );
}

export default TwoFactorAuthenticationForm;
