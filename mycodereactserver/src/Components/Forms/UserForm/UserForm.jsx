import { useState, useEffect } from "react";
import { Link, useNavigate } from "react-router-dom";
import { useUser } from "../../../Services/UserContext";
import { homePage, uPwChange } from "../../../Services/Frontend.Endpoints";
import { deleteAccount } from "../../../Services/Backend.Endpoints";
import { BlurredOverlay, ModalContainer, StyledModal } from "../../Styles/Background.styled";
import { ButtonContainer } from "../../Styles/ButtonContainer.styled";
import { ButtonRowContainer } from "../../Styles/ButtonRow.styled";
import { InputForm, InputWrapper } from "../../Styles/Input.styled";
import { TextContainer } from "../../Styles/TextContainer.styled";
import { Form, FormRow } from "../../Styles/Form.styled";
import Notify from "./../../../Pages/Services/ToastNotifications";
import DeleteActions from "../../../Components/Delete/DeleteActions";
import Loading from "../../Loading/Loading";
import Modal from 'react-bootstrap/Modal';
import ErrorPage from "./../../../Pages/Services/ErrorPage";

const UserForm = ({ onSave, user, onCancel }) => {
    const navigate = useNavigate();
    const { userData, setUserData } = useUser();
    const { role, userid } = userData;
    const [loading, setLoading] = useState(false);
    const [email, setEmail] = useState(user?.email ?? "");
    const [username, setUsername] = useState(user?.userName ?? "");
    const [password, setPassword] = useState(user?.password ?? "");
    const [displayname, setDisplayname] = useState(user?.displayName ?? "");
    const [phoneNumber, setPhone] = useState(user?.phoneNumber ?? "");
    const [isRegistration, setIsRegistration] = useState(!user);
    const [updateUrl, setUpdateUrl] = useState([]);
    const [showDeleteModal, setShowDeleteModal] = useState(false);
    const [userToDeleteId, setUserToDeleteId] = useState(null);
    const [errorMessage, setError] = useState("");

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
            setError(`Error occurred during form submission: ${error}`);
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        try {
            setUpdateUrl(userid);
        } catch (error) {
            setError(`Token error: ${error}`);
        }
    }, [location]);

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
        setUpdateUrl([]);
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
                                        type="password"
                                    />
                                </InputWrapper>
                            </>
                        )}
                    </FormRow>

                    {user && role === "User" && (
                        <ButtonRowContainer>
                            <Link to={uPwChange} className="link">
                                <ButtonContainer type="button">Password Change</ButtonContainer>
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
                                    <ButtonContainer onClick={() => setShowDeleteModal(false)}>
                                        Cancel
                                    </ButtonContainer>
                                    <ButtonContainer onClick={confirmDelete}>
                                        Delete
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
