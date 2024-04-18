import React, { useState, useEffect } from "react";
import { Outlet, Link, useLocation, useNavigate } from "react-router-dom";
import Modal from "react-bootstrap/Modal";
import Cookies from "js-cookie";

import { deleteApi } from "../../Services/Api";
import { ErrorPage, Notify } from "../Services/index";
import { useUser, logoutUser } from "../../Services/UserContext";
import { uReg, uLogin, uPwChange, uUpdateOwn, cReg, cOwn, cOthers, uList, cList, homePage, priPol } from "../../Services/Frontend.Endpoints";
import { facebookLogin, gitHubLogin, googleLogin, recentChuckNorris, revoke } from "../../Services/Backend.Endpoints";
import { backendUrl } from "../../Services/Config";
import Loading from "../../Components/Loading/index";

import { ButtonRowContainer, ButtonRowButtonContainer, ButtonColumnContainer } from "../../Components/Styles/ButtonRow.styled";
import { ButtonContainer } from "../../Components/Styles/ButtonContainer.styled";
import { BlurredOverlay, ModalContainer, StyledModal } from "../../Components/Styles/Background.styled";
import { CenteredContainer, ColumnTextWrapper, TextWrapper } from "../../Components/Styles/TextContainer.styled";
import { TextContainer } from "../../Components/Styles/TextContainer.styled";
import "../../index.css";

const Layout = () => {
    const navigate = useNavigate();
    const location = useLocation();
    const { userData, setUserData } = useUser();
    const { role, userid } = userData;
    const [showLogoutModal, setShowLogoutModal] = useState(false);
    const [logoutSuccess, setLogoutSuccess] = useState(null);
    const [chuckNorrisFact, setChuckNorrisFact] = useState([]);
    const [loading, setLoading] = useState(false);
    const [errorMessage, setError] = useState("");

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

    const confirmLogout = async (loggedOut) => {
        setShowLogoutModal(false);
        logoutUser();
        setUserData(null);
        const data = await deleteApi(revoke);

        if (loggedOut && data) {
            Notify("Success", "You logged out successfully!");
            setLogoutSuccess(true);
        } else {
            Notify("Error", "Session has expired. Please log in again.");
            setLogoutSuccess(false);
        }
    };

    const handleOnExternalLogin = async (ext) => {
        setLoading(true);
        try {
            const whichExtLogin = ext == "GitHub" ? gitHubLogin : ext == "Google" ? googleLogin : facebookLogin;

            window.location.href = await `${backendUrl}${whichExtLogin}`;

            const userId = Cookies.get("UI");
            const userRole = Cookies.get("UR");

            if (userId != "" || userId != undefined) {
                setUserData(userRole, userId);

                Notify("Success", `Successful Login via ${ext}!`);
            } else {
                Notify("Error", "Probably invalid username or password. Please try again.");
            }
        } catch (error) {
            setError(`Error occurred during login: ${error}`);
        } finally {

            Cookies.remove("UI");
            Cookies.remove("UR");

            setLoading(false);
        }
    };

    useEffect(() => {
        if (logoutSuccess !== null) {
            if (logoutSuccess) {
                setLogoutSuccess(null);
                navigate(homePage);
            } else {
                setLogoutSuccess(null);
                navigate(uLogin);
            }
        }
    }, [logoutSuccess]);

    if (loading) {
        return <Loading />;
    }

    return (
        <div className="Layout">
            {errorMessage === "" ? (
                <nav>
                    <CenteredContainer>
                        {chuckNorrisFact}
                    </CenteredContainer>
                    {!role && !userid ? (
                        <ButtonRowContainer>
                            {location.pathname !== uLogin && location.pathname !== uReg && (
                                <ButtonRowButtonContainer style={{ marginLeft: "17%" }}>
                                    <Link to={uLogin} className="link">
                                        <ButtonContainer type="button">Login</ButtonContainer>
                                    </Link>
                                    <Link to={uReg} className="link">
                                        <ButtonContainer type="button">Registration</ButtonContainer>
                                    </Link>
                                    <ButtonColumnContainer style={{ marginTop: "-6%", marginLeft: "147%" }}>
                                        <ButtonContainer type="button" style={{ textAlign: "center", marginLeft: "8%" }}>
                                            <Link to={priPol} className="link">Privacy Policy</Link>
                                        </ButtonContainer>
                                        <TextContainer type="text">You can also Login with:</TextContainer>
                                        <ButtonContainer type="button" onClick={() => handleOnExternalLogin("Google")} style={{ marginTop: "-10%", marginLeft: "8%" }}>
                                            Google
                                        </ButtonContainer>
                                        <ButtonContainer type="button" onClick={() => handleOnExternalLogin("Facebook")} style={{ marginLeft: "8%" }}>
                                            Facebook
                                        </ButtonContainer>
                                        <ButtonContainer type="button" onClick={() => handleOnExternalLogin("GitHub")} style={{ marginLeft: "8%" }}>
                                            GitHub
                                        </ButtonContainer>
                                        <ColumnTextWrapper style={{ marginTop: "-6%", marginLeft: "10%" }}>Accounts</ColumnTextWrapper>
                                    </ButtonColumnContainer>
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
                                {role === "Admin" ? (
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
