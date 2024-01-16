import Cookies from "js-cookie";
import React, { useState, useEffect } from "react";
import { Outlet, Link, useLocation, useNavigate } from "react-router-dom";
import { ButtonContainer } from "../../Components/Styles/ButtonContainer.styled";
import { ButtonRowContainer } from "../../Components/Styles/ButtonRow.styled";
import '../../index.css';

const Layout = () => {
    const location = useLocation();
    const [jwtToken, setJwtToken] = useState(Cookies.get("jwtToken"));
    const navigate = useNavigate();

    useEffect(() => {
        const token = Cookies.get("jwtToken");
        setJwtToken(token);
    }, [location]);

    const handleLogout = () => {
        Cookies.remove("jwtToken");
        navigate("/");
        setJwtToken(null);
    };

    return (
        <div className="Layout">
            <nav>
                {!jwtToken ? (
                    <ButtonRowContainer>
                        {location.pathname !== "/account/login" && (
                            <Link to="/account/login" className="link">
                                <ButtonContainer type="button">Login</ButtonContainer>
                            </Link>
                        )}
                        <Link to="/account/register" className="link">
                            <ButtonContainer type="button">Registration</ButtonContainer>
                        </Link>
                    </ButtonRowContainer>
                ) : (
                    <ButtonRowContainer>
                        <ButtonContainer type="button" onClick={handleLogout}>Logout</ButtonContainer>
                        <Link to="/" className="link">
                            <ButtonContainer type="button">MyCode Home</ButtonContainer>
                        </Link>
                        <Link to="/pwchange" className="link">
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
