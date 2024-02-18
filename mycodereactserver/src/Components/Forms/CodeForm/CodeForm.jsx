import { useState, useRef } from "react";
import Editor from "@monaco-editor/react";

import { ErrorPage, codeTypeOptions, handleEditorDidMount, copyContentToClipboard, toggleFullscreen, changeFontSize, changeTheme } from "./../../../Pages/Services";
import Loading from "../../Loading/index";

import { ButtonContainer } from "../../Styles/ButtonContainer.styled";
import { ButtonRowButtonContainer, ButtonRowContainer } from "../../Styles/ButtonRow.styled";
import { InputForm, InputWrapper } from "../../Styles/Input.styled";
import { TextContainer } from "../../Styles/TextContainer.styled";
import { Form, FormColumn } from "../../Styles/Form.styled";

const CodeForm = ({ onSave, code, role, onCancel }) => {
    const editorRef = useRef(null);
    const originalEditorMeasure = ["25vh", "90vh"];
    const [loading, setLoading] = useState(false);
    const [codeTitle, setCodeTitle] = useState(code?.codeTitle ?? "");
    const [myCode, setMyCode] = useState(code?.myCode ?? "");
    const [whatKindOfCode, setWhatKindOfCode] = useState(code?.whatKindOfCode ?? "");
    const [isBackend, setIsBackend] = useState(code?.isBackend ?? false);
    const [isVisible, setIsVisible] = useState(code?.isVisible ?? false);
    const [otherCodeType, setOtherCodeType] = useState("");
    const [editorMeasure, setEditorMeasure] = useState([originalEditorMeasure[0], originalEditorMeasure[1]]);
    const [fontSize, setFontSize] = useState(16);
    const [theme, setTheme] = useState("vs-dark");
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

    const handleCopyToClipboard = (e) => {
        e.preventDefault();
        copyContentToClipboard(editorRef);
    };

    const handleToggleFullscreen = (e) => {
        e.preventDefault();
        toggleFullscreen(editorRef, originalEditorMeasure, setEditorMeasure);
    };

    if (loading) {
        return <Loading />;
    }

    return (
        <div>
            {errorMessage === "" ? (
                <Form onSubmit={onSubmit}>
                    <>
                        <ButtonRowButtonContainer className="control">
                            <h3>Code Title:</h3>
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

                            <h3>What kind of code:</h3>
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
                        </ButtonRowButtonContainer>

                        <FormColumn>
                            <TextContainer>The Code Itself
                                <>
                                    <Editor
                                        height={editorMeasure[0]}
                                        width={editorMeasure[1]}
                                        defaultLanguage={whatKindOfCode.toLowerCase().replace(/#/g, "sharp")}
                                        defaultValue={myCode}
                                        onChange={(newValue, e) => setMyCode(newValue)}
                                        name="mycode"
                                        id="mycode"
                                        autoComplete="off"
                                        onMount={(editor, monaco) => handleEditorDidMount(editor, monaco, editorRef)}
                                        options={{ readOnly: false, fontSize: fontSize }}
                                        theme={theme}
                                    />
                                    <div>
                                        <label htmlFor="fontSizeSelector"> Font Size: </label>
                                        <select id="fontSizeSelector" onChange={(e) => changeFontSize(e, setFontSize)} value={fontSize}>
                                            {Array.from({ length: 23 }, (_, i) => i + 8).map(size => (
                                                <option key={size} value={size}>{size}</option>
                                            ))}
                                        </select>
                                        <label htmlFor="themeSelector"> Change Theme: </label>
                                        <select id="themeSelector" onChange={(e) => changeTheme(e, setTheme)} value={theme}>
                                            <option value="vs">Light</option>
                                            <option value="vs-dark">Dark</option>
                                        </select>
                                    </div>
                                    <div>
                                        <button onClick={handleCopyToClipboard}>Copy to Clipboard</button>
                                        <button onClick={handleToggleFullscreen}>Fullscreen</button>
                                    </div>
                                </>
                            </TextContainer>

                            <ButtonRowButtonContainer>
                                <h4>Backend Code?</h4>
                                <InputWrapper>
                                    <input
                                        type="checkbox"
                                        checked={isBackend}
                                        onChange={(e) => setIsBackend(e.target.checked)}
                                        name="backend"
                                        id="backend"
                                    />
                                </InputWrapper>

                                <h4>Can others see it?</h4>
                                <InputWrapper>
                                    <input
                                        type="checkbox"
                                        checked={isVisible}
                                        onChange={(e) => setIsVisible(e.target.checked)}
                                        name="visible"
                                        id="visible"
                                    />
                                </InputWrapper>
                            </ButtonRowButtonContainer>

                            <ButtonRowContainer>
                                <ButtonContainer type="submit">
                                    {code ? "Update Code" : "Add Code"}
                                </ButtonContainer>
                                <ButtonContainer type="button" onClick={onCancel}>
                                    Cancel
                                </ButtonContainer>
                            </ButtonRowContainer>
                        </FormColumn>
                    </>
                </Form>
            ) : (
                <ErrorPage errorMessage={errorMessage} />
            )}
            {loading && <Loading />}
        </div>
    );
};

export default CodeForm;
