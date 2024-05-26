import { useState, useRef } from "react";
import Editor from "@monaco-editor/react";

import { useUser } from "../../../Services/UserContext";
import { ErrorPage, codeTypeOptions, handleEditorDidMount, copyContentToClipboard, toggleFullscreen, changeFontSize, changeTheme } from "./../../../Pages/Services";
import Loading from "../../Loading/index";

import { Heading3, Heading4 } from "../../Styles/InputOutput.styled";
import { StyledButton } from "../../Styles/Buttons/InternalButtons.styled";
import { CodeFormStyle, FormColumn, InputForm } from "../../Styles/Forms.styled";
import { CodeTextContainer } from "../../Styles/Containers/ComplexContainers.styled";
import { InputWrapper, ButtonRowWrapper } from "../../Styles/Containers/Wrappers.styled";
import { AddCodeButtonRowContainer, AddCodeButtonRowButtonContainer, RowButtonContainer } from "../../Styles/Containers/Containers.styled";
import { EditorRowButtonContainer, EditorLabel, EditorCheckbox, EditorSelect, EditorOption } from "../../Styles/CustomBoxes/Editor.styled";

const CodeForm = ({ onSave, code, role, onCancel }) => {
    const editorRef = useRef(null);
    const originalEditorMeasure = ["30vh", "90vh"];
    const { userData } = useUser();
    const { userid } = userData;
    const [loading, setLoading] = useState(false);
    const [codeTitle, setCodeTitle] = useState(code?.codeTitle ?? "");
    const [myCode, setMyCode] = useState(code?.myCode ?? "");
    const [whatKindOfCode, setWhatKindOfCode] = useState(code?.whatKindOfCode ?? "");
    const [isBackend, setIsBackend] = useState(code?.isBackend ?? false);
    const [isVisible, setIsVisible] = useState(code?.isVisible ?? false);
    const [otherCodeType, setOtherCodeType] = useState("");
    const [editorMeasure, setEditorMeasure] = useState([originalEditorMeasure[0], originalEditorMeasure[1]]);
    const [fontSize, setFontSize] = useState(localStorage.getItem(`fontsize-${userid}`) ?? 16);
    const [theme, setTheme] = useState(localStorage.getItem(`theme-${userid}`) ?? "vs-dark");
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
        <>
            {errorMessage === "" ? (
                <>
                    <CodeFormStyle onSubmit={onSubmit}>
                        <FormColumn>
                            <AddCodeButtonRowButtonContainer>
                                <Heading3>Code Title:</Heading3>
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

                                <Heading3>What kind of code:</Heading3>
                                <InputWrapper>
                                    <EditorSelect
                                        value={whatKindOfCode}
                                        onChange={(e) => setWhatKindOfCode(e.target.value)}
                                        name="codetype"
                                        id="codetype"
                                    >
                                        {codeTypeOptions.map(option => (
                                            <EditorOption key={option} value={option}>{option}</EditorOption>
                                        ))}
                                        <EditorOption value="Other">Other</EditorOption>
                                    </EditorSelect>
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
                            </AddCodeButtonRowButtonContainer>
                            <CodeTextContainer>The Code Itself
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
                                    <EditorRowButtonContainer>
                                        <EditorLabel htmlFor="fontSizeSelector"> Font Size: </EditorLabel>
                                        <EditorSelect id="fontSizeSelector" onChange={(e) => changeFontSize(e, setFontSize, userid)} value={fontSize}>
                                            {Array.from({ length: 23 }, (_, i) => i + 8).map(size => (
                                                <option key={size} value={size}>{size}</option>
                                            ))}
                                        </EditorSelect>
                                        <ButtonRowWrapper>
                                            <StyledButton onClick={handleCopyToClipboard}>Copy to Clipboard</StyledButton>
                                            <StyledButton onClick={handleToggleFullscreen}>Fullscreen</StyledButton>
                                        </ButtonRowWrapper>
                                        <EditorLabel htmlFor="themeSelector"> Change Theme: </EditorLabel>
                                        <EditorSelect id="themeSelector" onChange={(e) => changeTheme(e, setTheme, userid)} value={theme}>
                                            <EditorOption value="vs">Light</EditorOption>
                                            <EditorOption value="vs-dark">Dark</EditorOption>
                                        </EditorSelect>
                                    </EditorRowButtonContainer>
                                </>
                            </CodeTextContainer>

                            <RowButtonContainer>
                                <Heading4>Backend Code?</Heading4>
                                <InputWrapper>
                                    <EditorCheckbox
                                        type="checkbox"
                                        checked={isBackend}
                                        onChange={(e) => setIsBackend(e.target.checked)}
                                        name="backend"
                                        id="backend"
                                    />
                                </InputWrapper>

                                <Heading4>Can others see it?</Heading4>
                                <InputWrapper>
                                    <EditorCheckbox
                                        type="checkbox"
                                        checked={isVisible}
                                        onChange={(e) => setIsVisible(e.target.checked)}
                                        name="visible"
                                        id="visible"
                                    />
                                </InputWrapper>
                            </RowButtonContainer>

                            <AddCodeButtonRowContainer>
                                <StyledButton type="submit">
                                    {code ? "Update Code" : "Add Code"}
                                </StyledButton>
                                <StyledButton type="button" onClick={onCancel}>
                                    Cancel
                                </StyledButton>
                            </AddCodeButtonRowContainer>
                        </FormColumn>
                    </CodeFormStyle>
                </>
            ) : (
                <ErrorPage errorMessage={errorMessage} />
            )}
            {loading && <Loading />}
        </>
    );
};

export default CodeForm;
