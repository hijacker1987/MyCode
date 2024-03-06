import React, { useState } from "react";

function TwoFactorAuthenticationForm({ onEnable, onSubmit, onDisable, onCancel }) {
    const [code, setCode] = useState("");

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
        <form onSubmit={handleSubmit}>
            <button type="button" onClick={handleEnable}>
                Enable Verification
            </button>
            <label>
                Two factor authentication code:
                <input type="text" value={code} onChange={(e) => setCode(e.target.value)} />
            </label>
            <button type="submit">Send</button>
            <button type="button" onClick={handleDisable}>
                Disable Verification
            </button>
            <button type="button" onClick={onCancel}>
                Cancel
            </button>
        </form>
    );
}

export default TwoFactorAuthenticationForm;
