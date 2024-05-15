import React, { useState, useEffect, useRef } from "react";
import { useNavigate } from "react-router-dom";
import Editor from "@monaco-editor/react";
import Cookies from "js-cookie";

import { useUser } from "../Services/UserContext";
import { getApi, handleResponse } from "../Services/Api";
import { handleEditorDidMount, copyContentToClipboard, toggleFullscreen, changeFontSize, changeTheme } from "../Pages/Services";
import { getCodesByVisibility } from "../Services/Backend.Endpoints";

import { MidContainer } from "./Styles/TextContainer.styled";

const Homepage = () => {
    const navigate = useNavigate();
    const editorRef = useRef(null);
    const originalEditorMeasure = ["30vh", "90vh"];
    const { userData, setUserData } = useUser();
    const { role, userid } = userData;
    const [visibleCodes, setVisibleCodes] = useState([]);
    const [randomCodeIndex, setRandomCodeIndex] = useState(null);
    const [editorMeasure, setEditorMeasure] = useState([originalEditorMeasure[0], originalEditorMeasure[1]]);
    const [fontSize, setFontSize] = useState(localStorage.getItem(`fontsize-${userid}`) ?? 16);
    const [theme, setTheme] = useState(localStorage.getItem(`theme-${userid}`) ?? "vs-dark");

    useEffect(() => {
        const cookieDataASync = async () => {
            if (userid == null || userid == "" || userid == undefined) {
                const userId = Cookies.get("UI");
                const userName = Cookies.get("UD");
                const userRole = Cookies.get("UR");

                setUserData(userRole, userId, userName);

                Cookies.remove("UI");
                Cookies.remove("UD");
                Cookies.remove("UR");
            }
        };

        cookieDataASync();
    }, []);

    useEffect(() => {
        const fetchVisibleCodes = async () => {
            const responseData = await getApi(getCodesByVisibility);

            if (responseData === "Unauthorized") {
                handleResponse(responseData, navigate, setUserData);
            } else {
                setVisibleCodes(responseData);
            }
        };

        fetchVisibleCodes();
    }, []);

    useEffect(() => {
        const intervalId = setInterval(() => {
            if (visibleCodes.length > 0) {
                const newIndex = Math.floor(Math.random() * visibleCodes.length);
                setRandomCodeIndex(newIndex);
            }
        }, 30000);

        return () => clearInterval(intervalId);
    }, [visibleCodes]);

    useEffect(() => {
        const initialRandomCode = setTimeout(() => {
            if (visibleCodes.length > 0) {
                const newIndex = Math.floor(Math.random() * visibleCodes.length);
                setRandomCodeIndex(newIndex);
            }
        }, 0);

        return () => clearTimeout(initialRandomCode);
    }, [visibleCodes]);

    useEffect(() => {
        const storedTheme = localStorage.getItem(`theme-${userid}`);
        if (storedTheme !== theme) {
            setTheme(storedTheme);
        }
    }, [theme, userid]);

    useEffect(() => {
        const storedFontSize = localStorage.getItem(`fontsize-${userid}`);
        if (storedFontSize !== theme) {
            setFontSize(storedFontSize);
        }
    }, [fontSize, userid]);

    return (
        <div>
            {!role && !userid ? (
                <MidContainer className="welcome-text">
                    Welcome Code Fanatic!
                    <p>Please login or register</p>
                </MidContainer>
            ) : (
                role === "User" && randomCodeIndex !== null && visibleCodes.length > 0 && (
                        <MidContainer style={{marginTop:"10px"}} className="random-code">
                        <div>
                            Random Code of <h3>{visibleCodes[randomCodeIndex].displayName}</h3>
                            <h4>Title:</h4>
                            <h2>{visibleCodes[randomCodeIndex].codeTitle}</h2>
                        </div>

                        <Editor
                            height={editorMeasure[0]}
                            width={editorMeasure[1]}
                            defaultLanguage={visibleCodes[randomCodeIndex].whatKindOfCode.toLowerCase().replace(/#/g, "sharp")}
                            defaultValue={visibleCodes[randomCodeIndex].myCode}
                            onChange={(newValue, e) => setMyCode(newValue)}
                            name="mycode"
                            id="mycode"
                            autoComplete="off"
                            onMount={(editor, monaco) => handleEditorDidMount(editor, monaco, editorRef)}
                            options={{ readOnly: true, fontSize: fontSize }}
                            theme={theme}
                        />

                        <div>
                            <label htmlFor="fontSizeSelector"> Font Size: </label>
                            <select id="fontSizeSelector" onChange={(e) => changeFontSize(e, setFontSize, userid)} value={fontSize}>
                                {Array.from({ length: 23 }, (_, i) => i + 8).map(size => (
                                    <option key={size} value={size}>{size}</option>
                                ))}
                            </select>
                            <label htmlFor="themeSelector"> Change Theme: </label>
                            <select id="themeSelector" onChange={(e) => changeTheme(e, setTheme, userid)} value={theme}>
                                <option value="vs">Light</option>
                                <option value="vs-dark">Dark</option>
                            </select>
                        </div>

                        <div>
                            <button onClick={() => copyContentToClipboard(editorRef)}>Copy to Clipboard</button>
                            <button onClick={() => toggleFullscreen(editorRef, originalEditorMeasure, setEditorMeasure)}>Fullscreen</button>
                        </div>
                    </MidContainer>
                )
            )}
        </div>
    );
};

export default Homepage;
