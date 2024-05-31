import { useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import Modal from "react-bootstrap/Modal";

import { useUser } from "../../../Services/UserContext";
import { ErrorPage, Notify } from "./../../../Pages/Services";
import { homePage, uPwChange, u2fa, priPol, accDel } from "../../../Services/Frontend.Endpoints";
import { deleteAccount } from "../../../Services/Backend.Endpoints";
import DeleteActions from "../../../Components/Delete/index";
import Loading from "../../Loading/index";

import { InputForm, Form, FormRow } from "../../Styles/Forms.styled";
import { InputWrapper, ColumnTextWrapper } from "../../Styles/Containers/Wrappers.styled";
import { StyledButton } from "../../Styles/Buttons/InternalButtons.styled";
import { RowButtonWithTopMarginContainer } from "../../Styles/Containers/Containers.styled";
import { SmallTextContainer, TextContainer } from "../../Styles/Containers/ComplexContainers.styled";
import { ModalContainer, StyledModalContainer, ModalBody, ModalTitle } from "../../Styles/CustomBoxes/Modal.styled";
import { BlurredOverlayWrapper, ButtonContainerWrapper, ButtonToRightContainerWrapper } from "../../Styles/Containers/Wrappers.styled";

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
    const [isRegistration, setIsRegistration] = useState(!user || !user.emailConfirmed);
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
            Notify("Error", "Password is too strong, also Real Chuck Norris registered already!!!");
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
        <>
            {errorMessage === "" ? (
                <Form onSubmit={onSubmit}>
                    <FormRow>
                        <ColumnTextWrapper>
                            <RowButtonWithTopMarginContainer>
                                <SmallTextContainer>E-mail:</SmallTextContainer>
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
                            </RowButtonWithTopMarginContainer>

                            <RowButtonWithTopMarginContainer>
                                <SmallTextContainer>Username:</SmallTextContainer>
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
                            </RowButtonWithTopMarginContainer>

                            <RowButtonWithTopMarginContainer>
                                <SmallTextContainer>Displayname:</SmallTextContainer>
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
                            </RowButtonWithTopMarginContainer>

                            <RowButtonWithTopMarginContainer>
                                <SmallTextContainer>Phone number:</SmallTextContainer>
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
                            </RowButtonWithTopMarginContainer>

                            {isRegistration && (
                                <RowButtonWithTopMarginContainer>
                                    <SmallTextContainer>Password:</SmallTextContainer>
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
                                        <ButtonToRightContainerWrapper>
                                            <StyledButton type="button" onClick={() => setShowPassword(!showPassword)}>
                                                Show Password
                                            </StyledButton>
                                        </ButtonToRightContainerWrapper>
                                    </InputWrapper>
                                </RowButtonWithTopMarginContainer>
                            )}

                            {user && role === "Admin" && (
                                <RowButtonWithTopMarginContainer>
                                    <SmallTextContainer>Role:</SmallTextContainer>
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
                                        <ButtonToRightContainerWrapper>
                                            <StyledButton type="button" onClick={() => onRole(user.email)}>
                                                Change Role
                                            </StyledButton>
                                        </ButtonToRightContainerWrapper>
                                    </InputWrapper>
                                </RowButtonWithTopMarginContainer>
                            )}

                            {user && role === "User" && (
                                <RowButtonWithTopMarginContainer>
                                    <Link to={uPwChange} className="link">
                                        <StyledButton type="button">Password Change</StyledButton>
                                    </Link>
                                    <Link to={`${u2fa}${user.id}`} className="link">
                                        <StyledButton type="button">Verification</StyledButton>
                                    </Link>
                                    <StyledButton type="button" onClick={() => handleDelete(user.id)}>
                                        Delete Account
                                    </StyledButton>
                                </RowButtonWithTopMarginContainer>
                            )}

                            <RowButtonWithTopMarginContainer>
                                <StyledButton type="submit">
                                    {user ? "Update User" : "Register"}
                                </StyledButton>
                                <StyledButton type="button">
                                    <Link to={priPol} className="link">Privacy Policy</Link>
                                </StyledButton>
                                <StyledButton type="button" onClick={onCancel}>
                                    Cancel
                                </StyledButton>
                            </RowButtonWithTopMarginContainer>
                        </ColumnTextWrapper>
                    </FormRow>
                </Form>
            ) : (
            <ErrorPage errorMessage={errorMessage} />
            )}

            {loading && <Loading />}
            {showDeleteModal && (
                <BlurredOverlayWrapper>
                    <ModalContainer>
                        <StyledModalContainer>
                            <TextContainer>
                                <Modal.Header closeButton>
                                    <ModalTitle>Delete Confirmation</ModalTitle>
                                </Modal.Header>
                                <ModalBody>
                                    Are you sure you want to delete this code?
                                </ModalBody>
                            </TextContainer>
                            <ButtonContainerWrapper>
                                <Link to={`${accDel}`} className="link">
                                    <StyledButton type="button">More Info</StyledButton>
                                </Link>
                            </ButtonContainerWrapper>
                            <Modal.Footer>
                                <RowButtonWithTopMarginContainer>
                                    <StyledButton onClick={confirmDelete}>
                                        Delete
                                    </StyledButton>
                                    <StyledButton onClick={() => setShowDeleteModal(false)}>
                                        Cancel
                                    </StyledButton>
                                </RowButtonWithTopMarginContainer>
                            </Modal.Footer>
                        </StyledModalContainer>
                    </ModalContainer>
                </BlurredOverlayWrapper>                
            )}
        </>
    );
};

export default UserForm;
