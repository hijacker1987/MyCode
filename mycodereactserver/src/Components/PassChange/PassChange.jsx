import { useState, useEffect } from "react";

import Loading from "../Loading/index";

import { ButtonContainer, ButtonContainerWrapper } from "../../Components/Styles/ButtonContainer.styled";
import { ButtonRowContainer } from "../../Components/Styles/ButtonRow.styled";
import { InputForm, InputWrapper } from "../Styles/Input.styled";
import { TextContainer } from "../Styles/TextContainer.styled";
import { Form, FormRow } from "../Styles/Form.styled";

export const PassChange = ({ onPassChange, user, onCancel }) => {
    const [loading, setLoading] = useState(false);
    const [email, setEmail] = useState(user?.email || "");
    const [currentPassword, setCurrentPassword] = useState(user?.currentPassword || "");
    const [newPassword, setNewPassword] = useState(user?.newPassword || "");
    const [confirmPassword, setConfirmPassword] = useState(user?.confirmPassword || "");
    const [showPassword, setShowPassword] = useState(false);

    useEffect(() => {
        setEmail(user?.email || "");
    }, [user]);

    const handlePasswordChange = async (e) => {
        e.preventDefault();

        try {
            setLoading(true);
            await onPassChange({
                ...user,
                email,
                currentPassword,
                newPassword,
                confirmPassword
            });
        } catch (error) {
            console.error("Error occurred during password change: ", error);
        } finally {
            setLoading(false);
        }
    };

    if (loading) {
        return <Loading />;
    }

    return (
        <div>
            <Form onSubmit={handlePasswordChange}>
                <FormRow className="control">
                    <TextContainer>E-mail:</TextContainer>
                    <InputWrapper>
                        <InputForm
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            name="email"
                            id="email"
                            placeholder="E-mail address"
                            autoComplete="off"
                            readOnly
                        />
                    </InputWrapper>

                    <TextContainer>Current Password:</TextContainer>
                    <InputWrapper>
                        <InputForm
                            value={currentPassword}
                            onChange={(e) => setCurrentPassword(e.target.value)}
                            name="currentPassword"
                            id="currentPassword"
                            placeholder="Old Password"
                            autoComplete="off"
                            type={showPassword ? "text" : "password"}
                        />
                    </InputWrapper>

                    <TextContainer>New Password:</TextContainer>
                    <InputWrapper>
                        <InputForm
                            value={newPassword}
                            onChange={(e) => setNewPassword(e.target.value)}
                            name="newPassword"
                            id="newPassword"
                            placeholder="New Password"
                            autoComplete="off"
                            type={showPassword ? "text" : "password"}
                        />
                    </InputWrapper>
                    <TextContainer>Confirm Password:</TextContainer>
                    <InputWrapper>
                        <InputForm
                            value={confirmPassword}
                            onChange={(e) => setConfirmPassword(e.target.value)}
                            name="confirmPassword"
                            id="confirmPassword"
                            placeholder="Confirm Password"
                            autoComplete="off"
                            type={showPassword ? "text" : "password"}
                        />
                    </InputWrapper>
                    <ButtonContainerWrapper>
                        <ButtonContainer type="button" onClick={() => setShowPassword(!showPassword)}>Show Password</ButtonContainer>
                    </ButtonContainerWrapper>
                </FormRow>

                <ButtonRowContainer>
                    <ButtonContainer type="submit">
                        Change Password
                    </ButtonContainer>
                    <ButtonContainer type="button" onClick={onCancel}>
                        Cancel
                    </ButtonContainer>
                </ButtonRowContainer>
            </Form>
            {loading && <Loading />}

        </div>
    );
};
