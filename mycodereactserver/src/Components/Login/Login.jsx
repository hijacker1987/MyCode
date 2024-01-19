import Loading from "../Loading/Loading";
import { useState } from "react";
import { ButtonContainer } from "../Styles/ButtonContainer.styled";
import { ButtonRowContainer } from "../Styles/ButtonRow.styled";
import { TextContainer } from "../Styles/TextContainer.styled";
import { Form, FormRow } from "../Styles/Form.styled";
import { InputForm, InputWrapper } from "../Styles/Input.styled";

const Login = ({ onLogin, user, onCancel }) => {

    const [loading, setLoading] = useState(false);
    const [email, setEmail] = useState(user?.email ?? "");
    const [password, setPassword] = useState(user?.password ?? "");

    const onSubmit = (e) => {
        e.preventDefault();

        return onLogin({
            email,
            password
        });
    };

    if (loading) {
        return <Loading />;
    }

    return (
        <Form onSubmit={onSubmit}>

            <FormRow>
                <TextContainer>E-mail:</TextContainer>
                <InputWrapper>
                    <InputForm
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        name="email"
                        id="email"
                        placeholder="E-mail address"
                    />
                </InputWrapper>

                <TextContainer>Password:</TextContainer>
                <InputWrapper>
                    <InputForm
                        value={password}
                        onChange={(e) => setPassword(e.target.value)}
                        name="password"
                        id="password"
                        placeholder="Password"
                        autoComplete="off"
                        type="password"
                    />
                </InputWrapper>
            </FormRow>

            <ButtonRowContainer>
                <ButtonContainer type="submit">
                    Login
                </ButtonContainer>

                <ButtonContainer type="button" onClick={onCancel}>
                    Cancel
                </ButtonContainer>
            </ButtonRowContainer>
        </Form>
    );
};

export default Login;
