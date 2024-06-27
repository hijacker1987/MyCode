import { useState } from "react";

import Loading from "../Loading/index";
import GoodbyeImage from "../../assets/84652613.jpg";

import { InputForm, Form, FormRow } from "../Styles/Forms.styled";
import { StyledButton } from "../Styles/Buttons/InternalButtons.styled";
import { SmallTextContainer, EditorContainer } from "../Styles/Containers/ComplexContainers.styled";
import { RowButtonWithTopMarginContainer } from "../Styles/Containers/Containers.styled";
import { ButtonContainerWrapper, InputWrapper } from "../Styles/Containers/Wrappers.styled";

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
        <>
            <Form onSubmit={onSubmit}>

                <FormRow>
                    <SmallTextContainer>E-mail:</SmallTextContainer>
                    <InputWrapper>
                        <InputForm
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            name="email"
                            id="email"
                            placeholder="E-mail address"
                        />
                    </InputWrapper>

                    <SmallTextContainer>Password:</SmallTextContainer>
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
                            <StyledButton type="button" onClick={() => setShowPassword(!showPassword)}>Show Password</StyledButton>
                        </ButtonContainerWrapper>
                    </InputWrapper>

                    <RowButtonWithTopMarginContainer>
                        <StyledButton type="submit">
                            Login
                        </StyledButton>
                        <StyledButton type="button" onClick={onCancel}>
                            Cancel
                        </StyledButton>
                    </RowButtonWithTopMarginContainer>
                </FormRow>
            </Form>
            {email === "Goodbye" && (
                <EditorContainer>
                    <img src={GoodbyeImage} alt="Goodbye" />
                </EditorContainer>
            )}
            {loading && <Loading />}
        </>
    );
};

export default Login;
