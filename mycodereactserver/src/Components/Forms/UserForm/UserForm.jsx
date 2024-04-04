import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import Modal from "react-bootstrap/Modal";

import { useUser } from "../../../Services/UserContext";
import { ErrorPage, Notify } from "./../../../Pages/Services";
import { homePage, uPwChange, u2fa } from "../../../Services/Frontend.Endpoints";
import { deleteAccount } from "../../../Services/Backend.Endpoints";
import DeleteActions from "../../../Components/Delete/index";
import Loading from "../../Loading/index";

import { BlurredOverlay, ModalContainer, StyledModal } from "../../Styles/Background.styled";
import { ButtonContainer, ButtonContainerWrapper } from "../../Styles/ButtonContainer.styled";
import { ButtonRowContainer } from "../../Styles/ButtonRow.styled";
import { InputForm, InputWrapper } from "../../Styles/Input.styled";
import { TextContainer } from "../../Styles/TextContainer.styled";
import { Form, FormRow } from "../../Styles/Form.styled";

const UserForm = ({ onSave, onRole, user, onCancel }) => {
    const navigate = useNavigate();
    const { userData, setUserData } = useUser();
    const { role } = userData;
    const [loading, setLoading] = useState(false);
    const [email, setEmail] = useState(user?.email ?? "");
    const [username, setUsername] = useState(user?.userName ?? "");
    const [password, setPassword] = useState(user?.password ?? "");
    const [userRole, setUserRole] = useState(user?.role ?? "");
    const [showPassword, setShowPassword] = useState(false);
    const [displayname, setDisplayname] = useState(user?.displayName ?? "");
    const [phoneNumber, setPhone] = useState(user?.phoneNumber ?? "");
    const [isRegistration, setIsRegistration] = useState(!user);
    const [showDeleteModal, setShowDeleteModal] = useState(false);
    const [userToDeleteId, setUserToDeleteId] = useState(null);
    const [errorMessage, setError] = useState("");

    const isValidEmail = (email) => {
        const emailPattern = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return emailPattern.test(email);
    };

    const onSubmit = async (e) => {
        e.preventDefault();

        if (!email) {
            Notify("Error", "Please provide an e-mail address!");
            return;
        }
        if (!isValidEmail(email)) {
            Notify("Error", "Please provide a valid e-mail address!");
            return;
        }
        if (!username) {
            Notify("Error", "Please provide a user name!");
            return;
        }
        if (!password && isRegistration) {
            Notify("Error", "Please provide a password!");
            return;
        }
        if (password == "chucknorris") {
            Notify("Error", "That password can't be started with lowercase!!!");
            return;
        }
        if (password == "Chucknorris") {
            Notify("Error", "This specific password's second part can't be started with lowercase!!!");
            return;
        }
        if (password == "chuckNorris") {
            Notify("Error", "How dare You to not start your password with Uppercase!!!");
            return;
        }
        if (password == "ChuckNorris") {
            Notify("Error", "Password is too strong!!!");
            return;
        }
        if (password.length < 8 && isRegistration) {
            Notify("Error", "Password has to be at least 8 characters long.");
            return;
        }

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
            setError(`Error occurred during form submission: ${error}`);
        } finally {
            setLoading(false);
        }
    };

    const handleDelete = (userid) => {
        setUserToDeleteId(userid);
        setShowDeleteModal(true);
    };

    const confirmDelete = () => {
        if (role === "User") {
            DeleteActions.deleteRecord(
                `${deleteAccount}${userToDeleteId}`,
                () => {
                    Notify("Success", "Successfully removed!");                   
                    confirmLogout();
                },
                () => {
                    Notify("Error", "Unable to delete the acc!");
                },
                navigate,
                setUserData
            );
        }
        setShowDeleteModal(false);
    };

    const confirmLogout = () => {
        setUserData(null);
        navigate(homePage);
    };

    if (loading) {
        return <Loading />;
    }

    return (
        <div>
            {errorMessage === "" ? (
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
                                        type={showPassword ? "text" : "password"}
                                    />
                                </InputWrapper>
                                <ButtonContainerWrapper>
                                    <ButtonContainer type="button" onClick={() => setShowPassword(!showPassword)}>Show Password</ButtonContainer>
                                </ButtonContainerWrapper>
                            </>
                        )}

                        {user && role === "Admin" && (
                            <>
                                <TextContainer>Role:</TextContainer>
                                <InputWrapper>
                                    <InputForm
                                        value={userRole}
                                        onChange={(e) => setUserRole(e.target.value)}
                                        name="userrole"
                                        id="userrole"
                                        placeholder=""
                                        autoComplete="on"
                                        readOnly
                                    />
                                    <ButtonContainerWrapper>
                                        <ButtonContainer type="button" onClick={() => onRole(user.email)}>
                                            Change Role
                                        </ButtonContainer>
                                    </ButtonContainerWrapper>
                                </InputWrapper>
                            </>
                        )}

                    </FormRow>

                    {user && role === "User" && (
                        <ButtonRowContainer>
                            <Link to={uPwChange} className="link">
                                <ButtonContainer type="button">Password Change</ButtonContainer>
                            </Link>
                            <Link to={`${u2fa}${user.id}`} className="link">
                                <ButtonContainer type="button">Two factor verification</ButtonContainer>
                            </Link>
                            <ButtonContainer type="button" onClick={() => handleDelete(user.id)}>
                                Delete Account
                            </ButtonContainer>
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
            ) : (
            <ErrorPage errorMessage={errorMessage} />
            )}

            {loading && <Loading />}
            {showDeleteModal && (
                <BlurredOverlay>
                    <ModalContainer>
                        <StyledModal>
                            <TextContainer>
                                <Modal.Header closeButton>
                                    <Modal.Title>Delete Confirmation</Modal.Title>
                                </Modal.Header>
                                <Modal.Body>
                                    Are you sure you want to delete your Account?
                                </Modal.Body>
                            </TextContainer>
                            <Modal.Footer>
                                <ButtonRowContainer>
                                    <ButtonContainer onClick={confirmDelete}>
                                        Delete
                                    </ButtonContainer>
                                    <ButtonContainer onClick={() => setShowDeleteModal(false)}>
                                        Cancel
                                    </ButtonContainer>
                                </ButtonRowContainer>
                            </Modal.Footer>
                        </StyledModal>
                    </ModalContainer>
                </BlurredOverlay>
            )}
        </div>
    );
};

export default UserForm;
