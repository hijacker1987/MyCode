import React, { useState, useEffect } from "react";
import { Outlet, Link, useLocation, useNavigate } from "react-router-dom";
import { getToken, getUserRoles } from "../../Services/AuthService";
import { ButtonRowContainer, ButtonRowButtonContainer } from "../../Components/Styles/ButtonRow.styled";
import { ButtonContainer } from "../../Components/Styles/ButtonContainer.styled";
import { BlurredOverlay, ModalContainer, StyledModal } from "../../Components/Styles/Background.styled";
import { CenteredContainer } from "../../Components/Styles/TextContainer.styled";
import { TextContainer } from "../../Components/Styles/TextContainer.styled";
import { uReg, uLogin, uPwChange, uUpdateOwn, cReg, cOwn, cOthers, uList, cList, homePage } from "../../Services/Frontend.Endpoints";
import { recentChuckNorris } from "../../Services/Backend.Endpoints";
import Modal from 'react-bootstrap/Modal';
import Cookies from "js-cookie";
import ErrorPage from "../Services/ErrorPage";
import "../../index.css";
import Notify from "../Services/ToastNotifications";

const Layout = () => {
    const location = useLocation();
    const [jwtToken, setJwtToken] = useState(getToken);
    const [showLogoutModal, setShowLogoutModal] = useState(false);
    const [logoutSuccess, setLogoutSuccess] = useState(null);
    const [userRoles, setUserRoles] = useState([]);
    const [chuckNorrisFact, setChuckNorrisFact] = useState([]);
    const [errorMessage, setError] = useState("");
    const navigate = useNavigate();

    useEffect(() => {
        try {
            const token = getToken();

            if (typeof token === "string" && token.length > 0) {
                const roles = getUserRoles();

                setJwtToken(token);
                setUserRoles(roles);
            }
        } catch (error) {
            setError(`Token error: ${error}`);
        }
    }, [location]);

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

    const confirmLogout = (loggedOut) => {
        setShowLogoutModal(false);
        if (loggedOut) {
            setLogoutSuccess(true);
        } else {
            setLogoutSuccess(false)
        }
        Cookies.remove("jwtToken");
        setJwtToken(null);
    };

    useEffect(() => {
        if (jwtToken === null && !logoutSuccess) {
            navigate(uLogin);
            Notify("Error", "Session has expired. Please log in again.");
        }
        else if (jwtToken === null && logoutSuccess) {
            navigate(homePage);
            Notify("Success", "You logged out successfully!");
        }
    }, [jwtToken, navigate]);

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
                            <ButtonRowButtonContainer>
                                <Link to={uLogin} className="link">
                                    <ButtonContainer type="button">Login</ButtonContainer>
                                </Link>
                                <Link to={uReg} className="link">
                                    <ButtonContainer type="button">Registration</ButtonContainer>
                                </Link>
                            </ButtonRowButtonContainer>
                        )}
                    </ButtonRowContainer>
                ) : (
                    <ButtonRowContainer>
                        <ButtonRowButtonContainer>

                            <ButtonContainer type="button" onClick={handleLogout}>Logout</ButtonContainer>
                                <Link to={`${homePage}`} className="link">
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
                            </>
                            )}
                        </ButtonRowButtonContainer>
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
                                    <ButtonContainer onClick={() => confirmLogout(true)}>
                                        Logout
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
