import React, { useState, useEffect } from "react";
import { Outlet, Link, useLocation, useNavigate } from "react-router-dom";
import { jwtDecode } from "jwt-decode";
import { ButtonRowContainer } from "../../Components/Styles/ButtonRow.styled";
import { ButtonContainer } from "../../Components/Styles/ButtonContainer.styled";
import { CenteredContainer } from "../../Components/Styles/TextContainer.styled";
import { uReg, uLogin, uPwChange, uUpdateOwn, cReg, cOwn, cUpdateOwn, uList, cList } from "../../Services/Frontend.Endpoints";
import { recentChuckNorris } from "../../Services/Backend.Endpoints";
import Cookies from "js-cookie";
import "../../index.css";

const Layout = () => {
    const location = useLocation();
    const [jwtToken, setJwtToken] = useState(Cookies.get("jwtToken"));
    const [userRoles, setUserRoles] = useState([]);
    const [updateUrl, setUpdateUrl] = useState([]);
    const [chuckNorrisFact, setChuckNorrisFact] = useState([]);
    const navigate = useNavigate();

    useEffect(() => {
        try {
            const token = Cookies.get("jwtToken");

            if (typeof token === "string" && token.length > 0) {
                setJwtToken(token);

                const decodedToken = jwtDecode(token);
                const roles = decodedToken["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] || [];
                const userIdFromToken = decodedToken["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier" || []];
                
                setUserRoles(roles);
                setUpdateUrl(userIdFromToken);
            }
        } catch (error) {
            console.error("Error decoding JWT token:", error);
        }
    }, [location]);

    useEffect(() => {
        const fetchJoke = async () => {
            try {
                const chuckNorrisData = await fetch(recentChuckNorris);
                const chuckNorrisFact = await chuckNorrisData.json();
                setChuckNorrisFact(chuckNorrisFact.value);
            } catch (error) {
                console.error('Error occurred while fetching Chuck Norris:', error);
            }
        };

        fetchJoke();

        const intervalId = setInterval(fetchJoke, 20000);

        return () => clearInterval(intervalId);
    }, []);

    const handleLogout = () => {
        Cookies.remove("jwtToken");
        navigate("/");
        setJwtToken(null);
    };

    return (
        <div className="Layout">
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
                                    <Link to={uList} className="link">
                                        <ButtonContainer type="button">List Users</ButtonContainer>
                                    </Link>
                                    <Link to={cList} className="link">
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
                                    <Link to={cOwn} className="link">
                                        <ButtonContainer type="button">My Codes</ButtonContainer>
                                    </Link>
                                    <Link to={uUpdateOwn} className="link">
                                        <ButtonContainer type="button">My Account</ButtonContainer>
                                    </Link>
                                </>
                            )}
                        </ButtonRowContainer>
                        )}
            </nav>
            <Outlet />
        </div>
    );
};

export default Layout;
