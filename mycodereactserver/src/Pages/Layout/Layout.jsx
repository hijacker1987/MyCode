import Cookies from "js-cookie";
import React, { useState, useEffect } from "react";
import { Outlet, Link, useLocation, useNavigate } from "react-router-dom";
import { ButtonContainer } from "../../Components/Styles/ButtonContainer.styled";
import { ButtonRowContainer } from "../../Components/Styles/ButtonRow.styled";
import { uReg, uLogin, uPwChange, uList, cList } from "../../Services/Frontend.Endpoints";
import '../../index.css';

const Layout = () => {
    const location = useLocation();
    const [jwtToken, setJwtToken] = useState(Cookies.get("jwtToken"));
    const [buttonsVisible, setButtonsVisible] = useState(true);
    const navigate = useNavigate();

    useEffect(() => {
        const token = Cookies.get("jwtToken");
        setJwtToken(token);
    }, [location]);

    const handleLogout = () => {
        Cookies.remove("jwtToken");
        navigate("/");
        setJwtToken(null);
        setButtonsVisible(true);
    };

    const handleButtonClick = () => {
        setButtonsVisible(false);
    };

    return (
        <div className="Layout">
            <nav>
                {!jwtToken ? (
                    <ButtonRowContainer>
                        {location.pathname !== uLogin && location.pathname !== uReg && (
                            <ButtonRowContainer>
                                <Link to={uLogin} className="link">
                                    <ButtonContainer type="button" onClick={handleButtonClick}>Login</ButtonContainer>
                                </Link>
                                <Link to={uReg} className="link">
                                    <ButtonContainer type="button" onClick={handleButtonClick}>Registration</ButtonContainer>
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
                            <Link to={uList} className="link">
                                <ButtonContainer type="button">List Users</ButtonContainer>
                            </Link>
                            <Link to={cList} className="link">
                                <ButtonContainer type="button">List Codes</ButtonContainer>
                            </Link>
                            <Link to={uPwChange} className="link">
                            <ButtonContainer type="button">Password Change</ButtonContainer>
                        </Link>
                    </ButtonRowContainer>
                )}
            </nav>
            <Outlet />
        </div>
    );
};

export default Layout;
