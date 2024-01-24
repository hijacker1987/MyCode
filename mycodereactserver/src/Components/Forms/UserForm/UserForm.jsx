import { useState } from "react";
import { Link } from "react-router-dom";
import { uPwChange } from "../../../Services/Frontend.Endpoints";
import { ButtonContainer } from "../../Styles/ButtonContainer.styled";
import { ButtonRowContainer } from "../../Styles/ButtonRow.styled";
import { InputForm, InputWrapper } from "../../Styles/Input.styled";
import { TextContainer } from "../../Styles/TextContainer.styled";
import { Form, FormRow } from "../../Styles/Form.styled";
import Loading from "../../Loading/Loading";

const UserForm = ({ onSave, user, role, onCancel }) => {
    const [loading, setLoading] = useState(false);
    const [email, setEmail] = useState(user?.email ?? "");
    const [username, setUsername] = useState(user?.userName ?? "");
    const [password, setPassword] = useState(user?.password ?? "");
    const [displayname, setDisplayname] = useState(user?.displayName ?? "");
    const [phoneNumber, setPhone] = useState(user?.phoneNumber ?? "");
    const [isRegistration, setIsRegistration] = useState(!user);

    const onSubmit = async (e) => {
        e.preventDefault();

        try {
            setLoading(true);

            if (user) {
                await onSave({
                    ...user,
                    email,
                    username,
                    password,
                    displayname,
                    phoneNumber
                });
            } else {
                await onSave({
                    email,
                    username,
                    password,
                    displayname,
                    phoneNumber
                });
            }
        } catch (error) {
            console.error("Error occurred during form submission: ", error);
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
                <FormRow className="control">
                    <TextContainer>E-mail:</TextContainer>
                    <InputWrapper>
                        <InputForm
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                            name="email"
                            id="email"
                            placeholder="Your E-mail address"
                            autoComplete="off"
                        />
                    </InputWrapper>

                    <TextContainer>Username:</TextContainer>
                    <InputWrapper>
                        <InputForm
                            value={username}
                            onChange={(e) => setUsername(e.target.value)}
                            name="username"
                            id="username"
                            placeholder="Desired Username"
                            autoComplete="off"
                        />
                    </InputWrapper>

                    <TextContainer>Displayname:</TextContainer>
                    <InputWrapper>
                        <InputForm
                            value={displayname}
                            onChange={(e) => setDisplayname(e.target.value)}
                            name="displayname"
                            id="displayname"
                            placeholder="Desired Displayname"
                            autoComplete="off"
                        />
                    </InputWrapper>

                    <TextContainer>Phone number:</TextContainer>
                    <InputWrapper>
                        <InputForm
                            value={phoneNumber}
                            onChange={(e) => setPhone(e.target.value)}
                            name="phoneNumber"
                            id="phoneNumber"
                            placeholder="Add Your Phone Number"
                            autoComplete="off"
                        />
                    </InputWrapper>

                    {isRegistration && (
                        <>
                            <TextContainer>Password:</TextContainer>
                            <InputWrapper>
                                <InputForm
                                    value={password}
                                    onChange={(e) => setPassword(e.target.value)}
                                    name="password"
                                    id="password"
                                    placeholder="Set a Password"
                                    autoComplete="off"
                                    type="password"
                                />
                            </InputWrapper>
                        </>
                    )}
                </FormRow>

                {user && role === "User" && (
                    <ButtonRowContainer style={{ marginLeft: '250px' }}>
                        <Link to={uPwChange} className="link">
                            <ButtonContainer type="button">Password Change</ButtonContainer>
                        </Link>
                    </ButtonRowContainer>
                )}

                <ButtonRowContainer>
                    <ButtonContainer type="submit">
                        {user ? "Update User" : "Register"}
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

export default UserForm;
