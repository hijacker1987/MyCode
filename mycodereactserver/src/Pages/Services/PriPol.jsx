import React from "react";
import { Link } from "react-router-dom";

import { Form, FormRow } from "../../Components/Styles/Forms.styled";
import { StyledButton } from "../../Components/Styles/Buttons/InternalButtons.styled";
import { MidTextContainer, TextContainerWhite } from "../../Components/Styles/Containers/ComplexContainers.styled";

const PriPolPage = () => {
    return (
        <>
            <Form>
                <FormRow>
                    <MidTextContainer>
                        <h2>Privacy Policy</h2>
                        <TextContainerWhite>
                            <h3>
                                Welcome to MyCode!
                            </h3>
                            <p>
                                Below, we provide detailed information about the data we collect and process, as well as our privacy practices.
                            </p>
                            <h4>1. Data Collection and Use</h4>
                            <p>
                                Personal data provided by visitors: We may request a user and display name, email address, and password during registration (phonenumber optional).
                            </p>
                            <p>
                                Automatically collected information: Certain technical data, such as IP address, browser type, and visit timestamp, are automatically recorded when visiting the website.
                            </p>
                            <p>
                                Data is used solely for the operation and improvement of the website.
                            </p>
                            <h4>2. Data Storage and Security</h4>
                            <p>
                                Data is stored on secure servers protected from unauthorized access and use.
                                We implement strict security measures to protect data.
                            </p>
                            <h4>3. Data Sharing</h4>
                            <p>
                                We do not share personal data with third parties without the prior consent of users.
                                Exceptions may include service providers necessary for the operation of the website (e.g., external login).
                            </p>
                            <h4>4. Cookies and Tracking</h4>
                            <p>
                                The website may use cookies to enhance the user experience and personalize content.
                            </p>
                            <h4>5. User Rights</h4>
                            <p>
                                Users are entitled to access, modify, or delete personal data provided by them.
                                Users can request data deletion or restriction at any time.
                            </p>
                            <h4>6. Contact Us</h4>
                            <p>
                                If you have any questions or concerns regarding privacy, please contact us at: privacy@example.com.
                            </p>
                            <h5>This privacy policy was last updated on April 16, 2024.</h5>
                        </TextContainerWhite>
                        <Link to="#" className="link" onClick={() => window.history.back()} >
                            <StyledButton>Back</StyledButton>
                        </Link>
                    </MidTextContainer>
                </FormRow>
            </Form>
        </>
    );
};

export default PriPolPage;
