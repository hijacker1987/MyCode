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
import "../../Components/Styles/CustomCss/google.css";
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
                        <ButtonRowContainer style={{ marginTop: "5%" }}>
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
                                        <button className="gsi-material-button" onClick={() => handleOnExternalLogin("Google")} style={{ marginTop: "-10%", marginLeft: "8%" }}>
                                            <div className="gsi-material-button-content-wrapper">
                                                <span className="gsi-material-button-icon" style={{ marginLeft: "-13%" }}></span>
                                                <span className="gsi-material-button-contents">Google</span>
                                            </div>
                                        </button>
                                        <button className="loginBtn--facebook" onClick={() => handleOnExternalLogin("Facebook")} style={{ width: "120px", height: "40px", marginLeft: "8%" }}>
                                            <div className="gsi-material-button-content-wrapper">
                                                <span className="loginBtn--facebook-icon" style={{ marginLeft: "-13%" }}></span>
                                                <span className="gsi-material-button-contents" style={{ marginLeft: "2%", color: "aliceblue" }}>acebook</span>
                                            </div>
                                        </button>
                                        <button type="button" onClick={() => handleOnExternalLogin("GitHub")} style={{ width: "120px", height: "40px", marginLeft: "8%", cursor: "pointer" }} className="py-2 px-4 max-w-md flex justify-center items-center bg-gray-600 hover:bg-gray-700 focus:ring-gray-500 focus:ring-offset-gray-200 text-white w-full transition ease-in duration-200 text-center text-base font-semibold shadow-md focus:outline-none focus:ring-2 focus:ring-offset-2 rounded-lg">
                                            <svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" fill="currentColor" className="mr-2" viewBox="0 0 1792 1792">
                                                <path d="M896 128q209 0 385.5 103t279.5 279.5 103 385.5q0 251-146.5 451.5t-378.5 277.5q-27 5-40-7t-13-30q0-3 .5-76.5t.5-134.5q0-97-52-142 57-6 102.5-18t94-39 81-66.5 53-105 20.5-150.5q0-119-79-206 37-91-8-204-28-9-81 11t-92 44l-38 24q-93-26-192-26t-192 26q-16-11-42.5-27t-83.5-38.5-85-13.5q-45 113-8 204-79 87-79 206 0 85 20.5 150t52.5 105 80.5 67 94 39 102.5 18q-39 36-49 103-21 10-45 15t-57 5-65.5-21.5-55.5-62.5q-19-32-48.5-52t-49.5-24l-20-3q-21 0-29 4.5t-5 11.5 9 14 13 12l7 5q22 10 43.5 38t31.5 51l10 23q13 38 44 61.5t67 30 69.5 7 55.5-3.5l23-4q0 38 .5 88.5t.5 54.5q0 18-13 30t-40 7q-232-77-378.5-277.5t-146.5-451.5q0-209 103-385.5t279.5-279.5 385.5-103zm-477 1103q3-7-7-12-10-3-13 2-3 7 7 12 9 6 13-2zm31 34q7-5-2-16-10-9-16-3-7 5 2 16 10 10 16 3zm30 45q9-7 0-19-8-13-17-6-9 5 0 18t17 7zm42 42q8-8-4-19-12-12-20-3-9 8 4 19 12 12 20 3zm57 25q3-11-13-16-15-4-19 7t13 15q15 6 19-6zm63 5q0-13-17-11-16 0-16 11 0 13 17 11 16 0 16-11zm58-10q-2-11-18-9-16 3-14 15t18 8 14-14z"></path>
                                            </svg>
                                            GitHub
                                        </button>
                                        <ColumnTextWrapper style={{ marginTop: "-6%", marginLeft: "10%" }}>Accounts</ColumnTextWrapper>
                                    </ButtonColumnContainer>
                                </ButtonRowButtonContainer>
                            )}
                        </ButtonRowContainer>
                    ) : (
                        <ButtonRowContainer style={{ marginTop: "5%" }}>
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
