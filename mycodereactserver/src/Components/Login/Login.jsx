import { useState } from "react";

import Loading from "../Loading/index";

import { ButtonContainer, ButtonContainerWrapper } from "../Styles/ButtonContainer.styled";
import { InputForm, InputWrapper } from "../Styles/Input.styled";
import { ButtonRowContainer } from "../Styles/ButtonRow.styled";
import { TextContainer } from "../Styles/TextContainer.styled";
import { Form, FormRow } from "../Styles/Form.styled";

const Login = ({ onLogin, user, onCancel }) => {

    const [loading, setLoading] = useState(false);
    const [email, setEmail] = useState(user?.email ?? "");
    const [password, setPassword] = useState(user?.password ?? "");
    const [confirmPassword, setConfirmPassword] = useState(user?.confirmPassword ?? "");
    const [showPassword, setShowPassword] = useState(false);

    const onSubmit = async (e) => {
        e.preventDefault();

        if (email == "Chuck Norris") {
            window.location.href = "https://www.chucknorris.com";
            return;
        }

        try {
            setLoading(true); 
            
            await onLogin({
                email,
                password,
                confirmPassword
            });         
        } catch (error) {
            console.error("Error occurred during login: ", error);
        } finally {
            setLoading(false);
        }
    };

    if (loading) {
        return <Loading />;
    }

    return (
        <div>
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
                            onChange={(e) => {
                                setPassword(e.target.value);
                                setConfirmPassword(e.target.value);
                            }}
                            name="password"
                            id="password"
                            placeholder="Set a Password"
                            autoComplete="off"
                            type={showPassword ? "text" : "password"}
                        />
                        <ButtonContainerWrapper>
                            <ButtonContainer type="button" onClick={() => setShowPassword(!showPassword)}>Show Password</ButtonContainer>
                        </ButtonContainerWrapper>
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
            {loading && <Loading />}
        </div>
    );
};

export default Login;
