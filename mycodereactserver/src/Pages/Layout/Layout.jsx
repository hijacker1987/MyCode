import React, { useState, useEffect } from "react";
import { Outlet, Link, useLocation, useNavigate } from "react-router-dom";
import { getToken, getUserRoles, getUserIdFromToken } from "../../Services/AuthService";
import { toast } from "react-toastify";
import { ButtonRowContainer } from "../../Components/Styles/ButtonRow.styled";
import { ButtonContainer } from "../../Components/Styles/ButtonContainer.styled";
import { BlurredOverlay, ModalContainer, StyledModal } from "../../Components/Styles/Background.styled";
import { CenteredContainer } from "../../Components/Styles/TextContainer.styled";
import { TextContainer } from "../../Components/Styles/TextContainer.styled";
import { uReg, uLogin, uPwChange, uUpdateOwn, cReg, cOwn, cOthers, uList, cList } from "../../Services/Frontend.Endpoints";
import { recentChuckNorris, deleteAccount } from "../../Services/Backend.Endpoints";
import DeleteActions from "../../Components/Delete/DeleteActions";
import Modal from 'react-bootstrap/Modal';
import Cookies from "js-cookie";
import ErrorPage from "../Services/ErrorPage";
import "react-toastify/dist/ReactToastify.css";
import "../../index.css";

const Layout = () => {
    const location = useLocation();
    const [jwtToken, setJwtToken] = useState(getToken);
    const [showLogoutModal, setShowLogoutModal] = useState(false);
    const [showDeleteModal, setShowDeleteModal] = useState(false);
    const [userToDeleteId, setUserToDeleteId] = useState(null);
    const [userRoles, setUserRoles] = useState([]);
    const [updateUrl, setUpdateUrl] = useState([]);
    const [chuckNorrisFact, setChuckNorrisFact] = useState([]);
    const [errorMessage, setError] = useState("");
    const navigate = useNavigate();

    useEffect(() => {
        try {
            const token = getToken();

            if (typeof token === "string" && token.length > 0) {
                const roles = getUserRoles();
                const userIdFromToken = getUserIdFromToken();
                
                setJwtToken(token);
                setUserRoles(roles);
                setUpdateUrl(userIdFromToken);
            }
        } catch (error) {
            setError(`Token error: ${error}`);
        }
    }, [location]);

    const handleDelete = (userId) => {
        setUserToDeleteId(userId);
        setShowDeleteModal(true);
    };

    const confirmDelete = () => {
        if (userRoles.includes("User")) {
            DeleteActions.deleteRecord(
                `${deleteAccount}${userToDeleteId}`,
                () => {
                    toast.success("Successfully removed!", {
                        position: "top-right",
                        autoClose: 5000,
                        hideProgressBar: false,
                        closeOnClick: true,
                        pauseOnHover: true,
                        draggable: true,
                        progress: undefined,
                        theme: "dark",
                    });

                    setJwtToken(null);
                    setUserRoles([]);
                    setUpdateUrl([]);
                    confirmLogout();
                },
                () => {
                    toast.error("Unable to delete the acc!", {
                        position: "top-right",
                        autoClose: 5000,
                        hideProgressBar: false,
                        closeOnClick: true,
                        pauseOnHover: true,
                        draggable: true,
                        progress: undefined,
                        theme: "dark"
                    });
                }
            );
        }
        setShowDeleteModal(false);
    };

    useEffect(() => {
        const fetchJoke = async () => {
            try {
                const chuckNorrisData = await fetch(recentChuckNorris);
                const chuckNorrisFact = await chuckNorrisData.json();
                setChuckNorrisFact(chuckNorrisFact.value);
            } catch (error) {
                setError(`Error occurred while fetching Chuck Norris: ${error}`);
            }
        };

        fetchJoke();

        const intervalId = setInterval(fetchJoke, 20000);

        return () => clearInterval(intervalId);
    }, []);

    const handleLogout = () => {
        setShowLogoutModal(true);
    };

    const confirmLogout = () => {
        Cookies.remove("jwtToken");
        setShowLogoutModal(false);
        navigate("/");
        window.location.reload();
    };

    return (
        <div className="Layout">
            {errorMessage === "" ? (
            <nav>
                <CenteredContainer>
                    {chuckNorrisFact}
                </CenteredContainer>
                {!jwtToken ? (
                    <ButtonRowContainer>
                        {location.pathname !== uLogin && location.pathname !== uReg && (
                            <ButtonRowContainer>
                                <Link to={uLogin} className="link">
                                    <ButtonContainer type="button">Login</ButtonContainer>
                                </Link>
                                <Link to={uReg} className="link">
                                    <ButtonContainer type="button">Registration</ButtonContainer>
                                </Link>
                            </ButtonRowContainer>
                        )}
                    </ButtonRowContainer>
                ) : (
                        <ButtonRowContainer>
                            <ButtonContainer type="button" onClick={handleLogout}>Logout</ButtonContainer>
                            <Link to="/" className="link">
                                <ButtonContainer type="button">MyCode Home</ButtonContainer>
                            </Link>
                            {userRoles.includes("Admin") ? (
                                <>
                                    <Link to={`${uList}1`} className="link">
                                        <ButtonContainer type="button">List Users</ButtonContainer>
                                    </Link>
                                    <Link to={`${cList}1`} className="link">
                                        <ButtonContainer type="button">List Codes</ButtonContainer>
                                    </Link>
                                    <Link to={uPwChange} className="link">
                                        <ButtonContainer type="button">Password Change</ButtonContainer>
                                    </Link>
                                </>
                            ) : (
                                <>
                                    <Link to={cReg} className="link">
                                        <ButtonContainer type="button">Add Code</ButtonContainer>
                                        </Link>
                                        <Link to={`${cOwn}1`} className="link">
                                        <ButtonContainer type="button">My Codes</ButtonContainer>
                                    </Link>
                                        <Link to={`${cOthers}1`} className="link">
                                        <ButtonContainer type="button">Visible Codes</ButtonContainer>
                                    </Link>
                                    <Link to={uUpdateOwn} className="link">
                                        <ButtonContainer type="button">My Account</ButtonContainer>
                                    </Link>
                                    <ButtonContainer type="button" onClick={() => handleDelete(updateUrl)}>
                                        Delete Account
                                    </ButtonContainer>
                                </>
                            )}
                        </ButtonRowContainer>
                        )}
            </nav>
            ) : (
                <ErrorPage errorMessage={errorMessage} />
            )}
            <Outlet />
            {showLogoutModal && (
                <BlurredOverlay>
                    <ModalContainer>
                        <StyledModal>
                            <TextContainer>
                                <Modal.Header closeButton>
                                    <Modal.Title>Logout Confirmation</Modal.Title>
                                </Modal.Header>
                                <Modal.Body>
                                    Are you sure you want to logout?
                                </Modal.Body>
                            </TextContainer>
                            <Modal.Footer>
                                <ButtonRowContainer>
                                    <ButtonContainer onClick={() => setShowLogoutModal(false)}>
                                        Cancel
                                    </ButtonContainer>
                                    <ButtonContainer onClick={confirmLogout}>
                                        Logout
                                    </ButtonContainer>
                                </ButtonRowContainer>
                            </Modal.Footer>
                        </StyledModal>
                    </ModalContainer>
                </BlurredOverlay>
            )}
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

export default Layout;
