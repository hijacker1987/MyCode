import { useState } from "react";
import { codeTypeOptions } from "../../../Pages/Services/CodeLanguages";
import { ButtonContainer } from "../../Styles/ButtonContainer.styled";
import { ButtonRowContainer } from "../../Styles/ButtonRow.styled";
import { InputForm, InputWrapper } from "../../Styles/Input.styled";
import { TextContainer } from "../../Styles/TextContainer.styled";
import { Form, FormRow } from "../../Styles/Form.styled";
import ErrorPage from "./../../../Pages/Services/ErrorPage";
import Loading from "../../Loading/Loading";

const CodeForm = ({ onSave, code, role, onCancel }) => {
    const [loading, setLoading] = useState(false);
    const [codeTitle, setCodeTitle] = useState(code?.codeTitle ?? "");
    const [myCode, setMyCode] = useState(code?.myCode ?? "");
    const [whatKindOfCode, setWhatKindOfCode] = useState(code?.whatKindOfCode ?? "");
    const [isBackend, setIsBackend] = useState(code?.isBackend ?? false);
    const [isVisible, setIsVisible] = useState(code?.isVisible ?? false);
    const [otherCodeType, setOtherCodeType] = useState("");
    const [errorMessage, setError] = useState("");

    const onSubmit = async (e) => {
        e.preventDefault();

        try {
            setLoading(true);

            let modifiedCodeTitle = codeTitle;

            if (code && role === "Admin") {
                modifiedCodeTitle = "* " + codeTitle + " *";
            }

            if (code) {
                await onSave({
                    ...code,
                    codeTitle: modifiedCodeTitle,
                    myCode,
                    whatKindOfCode,
                    isBackend,
                    isVisible
                });
            } else {
                await onSave({
                    codeTitle: modifiedCodeTitle,
                    myCode,
                    whatKindOfCode,
                    isBackend,
                    isVisible
                });
            }
        } catch (error) {
            setError(`Error occurred during form submission: ${error}`);
        } finally {
            setLoading(false);
        }
    };

    if (loading) {
        return <Loading />;
    }

    return (
        <div>
            {errorMessage === "" ? (
                <Form onSubmit={onSubmit}>
                    <FormRow className="control">
                        <TextContainer>Code Title:</TextContainer>
                        <InputWrapper>
                            <InputForm
                                value={codeTitle}
                                onChange={(e) => setCodeTitle(e.target.value)}
                                name="title"
                                id="title"
                                placeholder="Your Code's Title"
                                autoComplete="off"
                            />
                        </InputWrapper>

                        <TextContainer>The Code Itself:</TextContainer>
                        <InputWrapper>
                            <InputForm
                                value={myCode}
                                onChange={(e) => setMyCode(e.target.value)}
                                name="mycode"
                                id="mycode"
                                placeholder="Your creation comes here..."
                                autoComplete="off"
                            />
                        </InputWrapper>

                        <TextContainer>What kind of code:</TextContainer>
                        <InputWrapper>
                            <select
                                value={whatKindOfCode}
                                onChange={(e) => setWhatKindOfCode(e.target.value)}
                                name="codetype"
                                id="codetype"
                            >
                                {codeTypeOptions.map(option => (
                                    <option key={option} value={option}>{option}</option>
                                ))}
                                <option value="Other">Other</option>
                            </select>
                            {whatKindOfCode === "Other" && (
                                <InputForm
                                    value={otherCodeType}
                                    onChange={(e) => setOtherCodeType(e.target.value)}
                                    name="othercodetype"
                                    id="othercodetype"
                                    placeholder="Specify other code type"
                                    autoComplete="off"
                                />
                            )}
                        </InputWrapper>

                        <TextContainer>Backend Code?</TextContainer>
                        <InputWrapper>
                            <input
                                type="checkbox"
                                checked={isBackend}
                                onChange={(e) => setIsBackend(e.target.checked)}
                                name="backend"
                                id="backend"
                            />
                        </InputWrapper>

                        <TextContainer>Can others see it?</TextContainer>
                        <InputWrapper>
                            <input
                                type="checkbox"
                                checked={isVisible}
                                onChange={(e) => setIsVisible(e.target.checked)}
                                name="visible"
                                id="visible"
                            />
                        </InputWrapper>
                    </FormRow>

                    <ButtonRowContainer>
                        <ButtonContainer type="submit">
                            {code ? "Update Code" : "Add Code"}
                        </ButtonContainer>
                        <ButtonContainer type="button" onClick={onCancel}>
                            Cancel
                        </ButtonContainer>
                    </ButtonRowContainer>
                </Form>
            ) : (
                <ErrorPage errorMessage={errorMessage} />
            )}
            {loading && <Loading />}
        </div>
    );
};

export default CodeForm;
