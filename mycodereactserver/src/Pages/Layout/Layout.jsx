import React, { useState, useEffect } from "react";
import { Outlet, Link, useLocation, useNavigate } from "react-router-dom";
import Modal from "react-bootstrap/Modal";
import Cookies from "js-cookie";

import { deleteApi } from "../../Services/Api";
import { ErrorPage, Notify } from "../Services/index";
import { useUser, logoutUser } from "../../Services/UserContext";
import {uReg, uLogin, uPwChange, uUpdateOwn,
        cReg, cOwn, cOthers, uList, cList,
        homePage, priPol
        } from "../../Services/Frontend.Endpoints";
import { facebookLogin, gitHubLogin, googleLogin, recentChuckNorris, revoke } from "../../Services/Backend.Endpoints";
import { backendUrl } from "../../Services/Config";
import CustomerChat from "../../Pages/Services/CustomerChat";
import Loading from "../../Components/Loading/index";

import { StyledButton } from "../../Components/Styles/Buttons/InternalButtons.styled";
import { CenteredContainer, RightBarTextContainer, TextContainer } from "../../Components/Styles/Containers/ComplexContainers.styled";
import { ModalContainer, ModalFooter, ModalTitle, ModalBody, StyledModalContainer } from "../../Components/Styles/CustomBoxes/Modal.styled";
import { BlurredOverlayWrapper, ButtonItemWrapper, ButtonContentsWrapper, ColumnTextWrapper } from "../../Components/Styles/Containers/Wrappers.styled";
import { LayoutPositionContainer, RightBarButtonColumnContainer, RowButtonWithTopMarginContainer } from "../../Components/Styles/Containers/Containers.styled";
import { FacebookButton, FacebookIcon,
         GitHubButton, GitHubIcon,
         GoogleButton, GoogleIcon
        } from "../../Components/Styles/Buttons/ExternalButtons.styled";

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
            const userName = Cookies.get("UD");

            if (userId != "" || userId != undefined) {
                setUserData(userRole, userId, userName);

                Notify("Success", `Successful Login via ${ext}!`);
            } else {
                Notify("Error", "Probably invalid username or password. Please try again.");
            }
        } catch (error) {
            setError(`Error occurred during login: ${error}`);
        } finally {

            Cookies.remove("UI");
            Cookies.remove("UR");
            Cookies.remuve("UD");

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
        <>
            {errorMessage === "" ? (
                <nav>
                    <CenteredContainer>
                        {chuckNorrisFact}
                    </CenteredContainer>
                    {!role && !userid ? (
                        <LayoutPositionContainer>
                            {location.pathname !== uLogin && location.pathname !== uReg && (
                            <>
                                <RowButtonWithTopMarginContainer>
                                    <Link to={uLogin} className="link">
                                        <StyledButton>
                                            Login
                                        </StyledButton>
                                    </Link>
                                    <Link to={uReg} className="link">
                                        <StyledButton>
                                            Registration
                                        </StyledButton>
                                    </Link>
                                </RowButtonWithTopMarginContainer>
                                <RightBarButtonColumnContainer>
                                    <Link to={priPol} className="link">
                                        <StyledButton>
                                                Privacy Policy
                                        </StyledButton>
                                    </Link>
                                    <RightBarTextContainer>You can also Login with:</RightBarTextContainer>
                                    <GoogleButton onClick={() => handleOnExternalLogin("Google")}>
                                        <ButtonItemWrapper>
                                            <GoogleIcon />
                                            <ButtonContentsWrapper>oogle</ButtonContentsWrapper>
                                        </ButtonItemWrapper>
                                    </GoogleButton>

                                    <FacebookButton onClick={() => handleOnExternalLogin("Facebook")}>
                                        <ButtonItemWrapper>
                                            <FacebookIcon />
                                            <ButtonContentsWrapper>acebook</ButtonContentsWrapper>
                                        </ButtonItemWrapper>
                                    </FacebookButton>

                                    <GitHubButton onClick={() => handleOnExternalLogin("GitHub")}>
                                        <GitHubIcon xmlns="http://www.w3.org/2000/svg" viewBox="0 0 1792 1792">
                                            <path d="M896 128q209 0 385.5 103t279.5 279.5 103 385.5q0 251-146.5 451.5t-378.5 277.5q-27 5-40-7t-13-30q0-3 .5-76.5t.5-134.5q0-97-52-142 57-6 102.5-18t94-39 81-66.5 53-105 20.5-150.5q0-119-79-206 37-91-8-204-28-9-81 11t-92 44l-38 24q-93-26-192-26t-192 26q-16-11-42.5-27t-83.5-38.5-85-13.5q-45 113-8 204-79 87-79 206 0 85 20.5 150t52.5 105 80.5 67 94 39 102.5 18q-39 36-49 103-21 10-45 15t-57 5-65.5-21.5-55.5-62.5q-19-32-48.5-52t-49.5-24l-20-3q-21 0-29 4.5t-5 11.5 9 14 13 12l7 5q22 10 43.5 38t31.5 51l10 23q13 38 44 61.5t67 30 69.5 7 55.5-3.5l23-4q0 38 .5 88.5t.5 54.5q0 18-13 30t-40 7q-232-77-378.5-277.5t-146.5-451.5q0-209 103-385.5t279.5-279.5 385.5-103zm-477 1103q3-7-7-12-10-3-13 2-3 7 7 12 9 6 13-2zm31 34q7-5-2-16-10-9-16-3-7 5 2 16 10 10 16 3zm30 45q9-7 0-19-8-13-17-6-9 5 0 18t17 7zm42 42q8-8-4-19-12-12-20-3-9 8 4 19 12 12 20 3zm57 25q3-11-13-16-15-4-19 7t13 15q15 6 19-6zm63 5q0-13-17-11-16 0-16 11 0 13 17 11 16 0 16-11zm58-10q-2-11-18-9-16 3-14 15t18 8 14-14z"></path>
                                        </GitHubIcon>
                                        GitHub
                                    </GitHubButton>
                                    <ColumnTextWrapper>Accounts</ColumnTextWrapper>
                                </RightBarButtonColumnContainer>
                            </>
                            )}
                        </LayoutPositionContainer>
                    ) : (
                        <LayoutPositionContainer>
                            <RowButtonWithTopMarginContainer>
                                <StyledButton type="button" onClick={handleLogout}>Logout</StyledButton>
                                <StyledButton type="button" onClick={() => navigate(homePage)}>MyCode Home</StyledButton>
                                {role === "Admin" || role === "Support" ? (
                                    <>
                                        <Link to={`${uList}1`} className="link">
                                            <StyledButton>
                                                List Users
                                            </StyledButton>
                                        </Link>
                                        <Link to={`${cList}1`} className="link">
                                            <StyledButton>
                                                List Codes
                                            </StyledButton>
                                        </Link>
                                        <Link to={uPwChange} className="link">
                                            <StyledButton>
                                                Password Change
                                            </StyledButton>
                                        </Link>
                                        <CustomerChat />
                                    </>
                                ) : (
                                    <>
                                        <Link to={cReg} className="link">
                                            <StyledButton>
                                                Add Code
                                            </StyledButton>
                                        </Link>
                                        <Link to={`${cOwn}1`} className="link">
                                            <StyledButton>
                                                My Codes
                                            </StyledButton>
                                        </Link>
                                        <Link to={`${cOthers}1`} className="link">
                                            <StyledButton>
                                                Visible Codes
                                            </StyledButton>
                                        </Link>
                                        <Link to={uUpdateOwn} className="link">
                                            <StyledButton>
                                                My Account
                                            </StyledButton>
                                        </Link>
                                        <CustomerChat />
                                    </>
                                )}
                                </RowButtonWithTopMarginContainer>
                        </LayoutPositionContainer>
                    )}
                </nav>
            ) : (
                <ErrorPage errorMessage={errorMessage} />
            )}
            <Outlet />

            {showLogoutModal && (
                <BlurredOverlayWrapper>
                    <ModalContainer>
                        <StyledModalContainer>
                            <TextContainer>
                                <Modal.Header closeButton>
                                    <ModalTitle>Logout Confirmation</ModalTitle>
                                </Modal.Header>
                                <ModalBody>
                                    Are you sure you want to logout?
                                </ModalBody>
                            </TextContainer>
                            <ModalFooter>
                                <RowButtonWithTopMarginContainer>
                                    <StyledButton onClick={() => setShowLogoutModal(false)}>
                                        Cancel
                                    </StyledButton>
                                    <StyledButton onClick={() => confirmLogout(true)}>
                                        Logout
                                    </StyledButton>
                                </RowButtonWithTopMarginContainer>
                            </ModalFooter>
                        </StyledModalContainer>
                    </ModalContainer>
                </BlurredOverlayWrapper>
            )}
        </>
    );
};

export default Layout;
