import { useState, useEffect } from "react";

import Loading from "../Loading/index";

import { InputForm, Form, FormRow } from "../Styles/Forms.styled";
import { StyledButton } from "../Styles/Buttons/InternalButtons.styled";
import { SmallTextContainer } from "../Styles/Containers/ComplexContainers.styled";
import { RowButtonWithTopMarginContainer } from "../Styles/Containers/Containers.styled";
import { ButtonContainerWrapper, InputWrapper } from "../Styles/Containers/Wrappers.styled";

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
                    <SmallTextContainer>E-mail:</SmallTextContainer>
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

                    <SmallTextContainer>Current Password:</SmallTextContainer>
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

                    <SmallTextContainer>New Password:</SmallTextContainer>
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

                    <SmallTextContainer>Confirm Password:</SmallTextContainer>
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
                    <ButtonContainerWrapper>
                        <StyledButton type="button" onClick={() => setShowPassword(!showPassword)}>Show Password</StyledButton>
                    </ButtonContainerWrapper>
                    </InputWrapper>

                    <RowButtonWithTopMarginContainer>
                        <StyledButton type="submit">
                            Change Password
                        </StyledButton>
                        <StyledButton type="button" onClick={onCancel}>
                            Cancel
                        </StyledButton>
                    </RowButtonWithTopMarginContainer>
                </FormRow>
            </Form>
            {loading && <Loading />}

        </div>
    );
};
