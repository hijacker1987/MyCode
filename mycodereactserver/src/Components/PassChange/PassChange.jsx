import Loading from "../Loading/Loading";
import { useState } from "react";
import { ButtonContainer } from "../../Components/Styles/ButtonContainer.styled";
import { ButtonRowContainer } from "../../Components/Styles/ButtonRow.styled";
import { TextContainer } from "../Styles/TextContainer.styled";
import { Form, FormRow } from "../Styles/Form.styled";
import { InputForm, InputWrapper } from "../Styles/Input.styled";

const PassChange = ({ onPassChange, user, onCancel }) => {
    const [loading, setLoading] = useState(false);
    const [email, setEmail] = useState(user?.email ?? "");
    const [currentPassword, setCurrentPassword] = useState(user?.currentPassword ?? "");
    const [newPassword, setNewPassword] = useState(user?.newPassword ?? "");

    const onSubmit = (e) => {
        e.preventDefault();

        if (user) {
            return onPassChange({
                ...user,
                email,
                currentPassword,
                newPassword
            });
        }

        return onPassChange({
            email,
            currentPassword,
            newPassword
        });
    };

    if (loading) {
        return <Loading />;
    }

    return (
        <div>
            <Form onSubmit={onSubmit}>

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
                        />
                    </InputWrapper>
                </FormRow>

                <FormRow className="control">
                    <TextContainer>Current Password:</TextContainer>
                    <InputWrapper>
                        <InputForm
                            value={currentPassword}
                            onChange={(e) => setCurrentPassword(e.target.value)}
                            name="currentPassword"
                            id="currentPassword"
                            placeholder="Old Password"
                            autoComplete="off"
                            type="password"
                        />
                    </InputWrapper>
                </FormRow>

                <FormRow className="control">
                    <TextContainer>New Password:</TextContainer>
                    <InputWrapper>
                        <InputForm
                            value={newPassword}
                            onChange={(e) => setNewPassword(e.target.value)}
                            name="newPassword"
                            id="newPassword"
                            placeholder="New Password"
                            autoComplete="off"
                            type="password"
                        />
                    </InputWrapper>
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
        </div>
    );
};

export default PassChange;
