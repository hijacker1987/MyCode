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

export const toggleFullscreen = (editorRef) => {
    const editor = editorRef.current;
    if (editor) {
        if (document.fullscreenElement) {
            document.exitFullscreen();
        } else {
            editor.getDomNode().requestFullscreen();
        }
    }
};

export const changeFontSize = (e, setFontSize) => {
    setFontSize(parseInt(e.target.value));
};

export const changeTheme = (e, setTheme) => {
    setTheme(e.target.value);
};
