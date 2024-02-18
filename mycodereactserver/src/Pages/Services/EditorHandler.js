import { Notify } from "../Services";

export const handleEditorDidMount = (editor, monaco, editorRef) => {
    if (editor) {
        editorRef.current = editor;
    }
};

export const copyContentToClipboard = (editorRef) => {
    const editor = editorRef.current;
    if (editor) {
        const code = editor.getValue();
        navigator.clipboard.writeText(code)
            .then(() => Notify("Success", "Code copied to clipboard"))
            .catch(error => console.error("Error copying code to clipboard:", error));
    }
};

export const toggleFullscreen = (editorRef, originalMeasure, setEditorMeasure) => {
    const editor = editorRef.current;
    if (editor) {
        const handleFullscreenChange = () => {
            if (!document.fullscreenElement) {
                setEditorMeasure([originalMeasure[0], originalMeasure[1]]);
                document.removeEventListener("fullscreenchange", handleFullscreenChange);
            }
        };

        if (document.fullscreenElement) {
            document.exitFullscreen().then(() => {
                setEditorMeasure([originalMeasure[0], originalMeasure[1]]);
            }).catch(error => console.error("Error exiting fullscreen:", error));
        } else {
            document.addEventListener("fullscreenchange", handleFullscreenChange);
            editor.getDomNode().requestFullscreen().then(() => {
                setEditorMeasure(["100vh", "100vw"]);
            }).catch(error => console.error("Error entering fullscreen:", error));
        }
    }
};

export const changeFontSize = (e, setFontSize) => {
    setFontSize(parseInt(e.target.value));
};

export const changeTheme = (e, setTheme) => {
    setTheme(e.target.value);
};
