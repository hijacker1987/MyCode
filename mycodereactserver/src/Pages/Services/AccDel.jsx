import React from "react";
import { Link } from "react-router-dom";

import { Form, FormRow } from "../../Components/Styles/Forms.styled";
import { StyledButton } from "../../Components/Styles/Buttons/InternalButtons.styled";
import { MidTextContainer, TextContainerWhite } from "../../Components/Styles/Containers/ComplexContainers.styled";

const AccDelPage = () => {
    return (
        <div>
            <Form>
                <FormRow>
                    <MidTextContainer>
                        <h1>Data Deletion Section</h1>
                        <TextContainerWhite>
                            <h2>
                                Welcome to MyCode!
                            </h2>
                            <p>
                                Below, we provide detailed information about the data deletion.
                            </p>
                            <h2>Data Storage and Security</h2>
                            <p>
                                Data is stored on secure servers protected from unauthorized access and use.
                                We implement strict security measures to protect data.
                            </p>
                            <h2>User Rights</h2>
                            <p>
                                Users are entitled to access, modify, or delete personal data provided by them.
                                Users can request data deletion or restriction at any time.
                            </p>
                            <h2>HOW TO DELETE</h2>
                            <h3>- Before at all, login!</h3>
                            <h3>- Than go to the my account!</h3>
                            <h3>- Click on Delete Account and than confirm!</h3>
                            <h4>We would like to let You know, all of your stored data will be pernamently deleted and won't be able to restore it for You!</h4>
                            <h2>Contact Us</h2>
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
        </div>
    );
};

export default AccDelPage;
