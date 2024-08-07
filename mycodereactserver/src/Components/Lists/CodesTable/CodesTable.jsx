import React, { useState, useEffect, useRef } from "react";
import { Link, useNavigate } from "react-router-dom";
import Editor from "@monaco-editor/react";
import Modal from "react-bootstrap/Modal";

import { useUser } from "../../../Services/UserContext";
import { codeTypeOptions } from "../../../Pages/Services/CodeLanguages";
import { Notify, handleEditorDidMount, copyContentToClipboard, toggleFullscreen, changeFontSize, changeTheme } from "./../../../Pages/Services";
import { cList, cOwn, cOthers, cUpdate } from "../../../Services/Frontend.Endpoints";
import { deleteCode, deleteSuperCode } from "../../../Services/Backend.Endpoints";
import ConstructPagination from "../../Forms/PaginationForm/index";
import DeleteActions from "../../Delete/index";

import { RelHeading3 } from "../../Styles/InputOutput.styled";
import { StyledButton } from "../../Styles/Buttons/InternalButtons.styled";
import { BlurredOverlayWrapper, ButtonRowWrapper } from "../../Styles/Containers/Wrappers.styled";
import { TextContainer } from "../../Styles/Containers/ComplexContainers.styled";
import { CodeTextContainer } from "../../Styles/Containers/ComplexContainers.styled";
import { RowButtonWithTopMarginContainer } from "../../Styles/Containers/Containers.styled";
import { EditorSelect, EditorOption, EditorLabel } from "../../Styles/CustomBoxes/Editor.styled";
import { StyledTable, StyledTh, StyledTr, StyledTd, RowSpacer, TableContainer } from "../../Styles/CustomBoxes/Table.styled";
import { ModalBody, ModalTitle, ModalContainer, PreviewModalContainer, StyledModalContainer } from "../../Styles/CustomBoxes/Modal.styled";

export const CodesTable = ({ codes, headers, kind, role, page, auth }) => {
    const navigate = useNavigate();
    const editorRef = useRef(null);
    const originalEditorMeasure = ["50vh", "152vh"];
    const originalEditorMeasurePreview = ["30vh", "91vh"];
    const { userData, setUserData } = useUser();
    const { username } = userData;
    const [updatedCodes, setUpdatedCodes] = useState(codes);
    const [showDeleteModal, setShowDeleteModal] = useState(false);
    const [selectedCodeId, setSelectedCodeId] = useState(null);
    const [recordPerPage, setRecordPerPage] = useState(5);
    const [paginationSlice, setPaginationSlice] = useState({ first: 0, second: recordPerPage - 1 });
    const [displayNameFilter, setDisplayNameFilter] = useState("");
    const [codeTitleFilter, setCodeTitleFilter] = useState("");
    const [codeTypeFilter, setCodeTypeFilter] = useState("");
    const [visibilityFilter, setVisibilityFilter] = useState("all"); //"all", "visible", or "hidden"
    const [sortOrder, setSortOrder] = useState("A-Z");
    const [showPreviewModal, setShowPreviewModal] = useState(false);
    const [previewCode, setPreviewCode] = useState(null);
    const [editorMeasure, setEditorMeasure] = useState([originalEditorMeasure[0], originalEditorMeasure[1]]);
    const [editorMeasurePreview, setEditorMeasurePreview] = useState([originalEditorMeasurePreview[0], originalEditorMeasurePreview[1]]);
    const [fontSize, setFontSize] = useState(localStorage.getItem(`fontsize-${username}`) ?? 16);
    const [theme, setTheme] = useState(localStorage.getItem(`theme-${username}`) ?? "vs-dark");
    const isAllowed = auth === "byAuth";

    useEffect(() => {
        const initialPage = page ? Math.max(1, Number(page)) : 1;

        setPaginationSlice({ first: initialPage * recordPerPage - recordPerPage, second: initialPage * recordPerPage });
    }, [page, recordPerPage, displayNameFilter, codeTypeFilter, visibilityFilter]);

    useEffect(() => {
        setUpdatedCodes(codes);
    }, [codes]);

    if (!updatedCodes || updatedCodes.length === 0) {
        return <RelHeading3>No code data available.</RelHeading3>;
    }

    const handleSort = () => {
        const sortedCodes = [...updatedCodes].sort((a, b) => {
            if (sortOrder === "A-Z") {
                setSortOrder("Z-A");
                return !isAllowed ? b.codeTitle.localeCompare(a.codeTitle) : b.displayName.localeCompare(a.displayName);
            } else {
                setSortOrder("A-Z");
                return !isAllowed ? a.codeTitle.localeCompare(b.codeTitle) : a.displayName.localeCompare(b.displayName);
            }
        });
        setUpdatedCodes(sortedCodes);
    };

    const handleCodePreview = (code) => {
        setPreviewCode(code);
        setShowPreviewModal(true);
    };

    const handleDelete = (codeId) => {
        setShowDeleteModal(true);
        setSelectedCodeId(codeId);
    };

    const confirmDelete = () => {
        const deleteUrl = role === "Admin" ? deleteSuperCode : deleteCode;

        DeleteActions.deleteRecord(
            `${deleteUrl}${selectedCodeId}`,
            () => {
                Notify("Success", "Successfully deleted the code!");
                setUpdatedCodes((prevCodes) => prevCodes.filter((code) => code.id !== selectedCodeId));
                setShowDeleteModal(false);
                setSelectedCodeId(null);
            },
            () => {
                Notify("Error", "Unable to delete the code!")
            },
            navigate,
            setUserData
        );
    };

    return (
        <TableContainer>
            <StyledTable>
                <thead>
                    <tr>
                        {!isAllowed && kind !== "visible Codes" && (
                            <StyledTh className="search-1">
                                <input
                                    type="text"
                                    placeholder={"Search by Code Title"}
                                    value={codeTitleFilter}
                                    onChange={(e) => (setCodeTitleFilter(e.target.value))}
                                    style={{ cursor: "pointer" }}
                                />
                            </StyledTh>                       
                        )}

                        {!isAllowed && auth === "byVis" && kind === "visible Codes" && (
                            <StyledTh className="search-1">
                                <input
                                    type="text"
                                    placeholder={"Search by Display Name"}
                                    value={displayNameFilter}
                                    onChange={(e) => (setDisplayNameFilter(e.target.value))}
                                    style={{ cursor: "pointer" }}
                                />
                            </StyledTh>
                        )}

                        <StyledTh className="search-2">
                            <select value={codeTypeFilter} onChange={(e) => setCodeTypeFilter(e.target.value)} style={{ cursor: "pointer" }}>
                                <option value="">All Code Types</option>
                                {codeTypeOptions.map(option => (
                                    <option key={option} value={option}>{option}</option>
                                ))}
                            </select>
                        </StyledTh>

                        {isAllowed && auth === "byVis" && (
                            <StyledTh className="search-3">
                            <select value={visibilityFilter} onChange={(e) => setVisibilityFilter(e.target.value)} style={{ cursor: "pointer" }}>
                                    <option value="all">All</option>
                                    <option value="visible">Visible</option>
                                    <option value="hidden">Hidden</option>
                                </select>
                            </StyledTh>
                        )}

                        {!isAllowed && kind !== "visible Codes" && (
                        <StyledTh className="search-3">
                            <select value={visibilityFilter} onChange={(e) => setVisibilityFilter(e.target.value)} style={{ cursor: "pointer" }}>
                                <option value="all">All</option>
                                <option value="visible">Visible</option>
                                <option value="hidden">Hidden</option>
                            </select>
                        </StyledTh>
                        )}

                        <StyledTh className="search-4" onClick={handleSort} style={{ cursor: "pointer" }}>
                            {sortOrder}
                        </StyledTh>                      
                    </tr>
                    <tr>
                        {headers.map((header) => (
                            <StyledTh key={header}>{header}</StyledTh>
                        ))}
                    </tr>
                </thead>
                <tbody>
                    {updatedCodes &&
                        updatedCodes
                            .filter((code) => {
                                if (!isAllowed && kind !== "visible Codes" || role === "Admin" || role === "Support") {
                                    return code.codeTitle && code.codeTitle.toLowerCase().includes(codeTitleFilter.toLowerCase().slice(0, 3));
                                } else {
                                    return code.displayName && code.displayName.toLowerCase().includes(displayNameFilter.toLowerCase().slice(0, 3));
                                }
                                if (!isAllowed && kind === "visible Codes") {
                                    return code.codeTitle && code.codeTitle.toLowerCase().includes(codeTitleFilter.toLowerCase().slice(0, 3));
                                } else {
                                    return code.displayName && code.displayName.toLowerCase().includes(displayNameFilter.toLowerCase().slice(0, 3));
                                }
                            })
                            .filter((code) => (codeTypeFilter ? code.whatKindOfCode === codeTypeFilter : true))
                            .filter((code) => (visibilityFilter === "all" ? true : code.isVisible === (visibilityFilter === "visible")))
                            .slice(paginationSlice.first, paginationSlice.second)
                            .map((code, index) => (
                            <React.Fragment key={code.id}>
                                <StyledTr className={index % 2 === 1 ? "even-row" : "odd-row"}>
                                    <StyledTd>{index + 1}</StyledTd>
                                    {!isAllowed && kind === "visible Codes" && (
                                        <StyledTd>{code.displayName}</StyledTd>
                                    )}

                                    <StyledTd>{code.codeTitle}</StyledTd>
                                    <StyledTd
                                        onClick={() => handleCodePreview(code)}
                                        style={{ cursor: "pointer" }}
                                    >
                                        {code.myCode.length <= 20 ? code.myCode : code.myCode.slice(0, 17) + "..."}
                                    </StyledTd>

                                    <StyledTd>{code.whatKindOfCode}</StyledTd>
                                    <StyledTd>{code.isBackend ? "Backend" : "Frontend"}</StyledTd>
                                    {isAllowed && kind === "visible Codes" || (role === "Admin" || role === "Support") && (
                                        <StyledTd>{code.isVisible ? "Yes" : "Hidden"}</StyledTd>
                                    )}

                                    {!isAllowed && kind !== "visible Codes" && (
                                        <StyledTd>{code.isVisible ? "Yes" : "Hidden"}</StyledTd>
                                    )}

                                    {!isAllowed && auth !== "byVis" || role === "Admin" && (
                                        <StyledTd>
                                            <ButtonRowWrapper>
                                                <Link to={`${cUpdate}${code.id}`}>
                                                    <StyledButton type="button">Edit</StyledButton>
                                                </Link>
                                                <StyledButton type="button" onClick={() => handleDelete(code.id)}>
                                                    Delete
                                                </StyledButton>
                                            </ButtonRowWrapper>
                                        </StyledTd>
                                    )}

                                    {!isAllowed && kind !== "visible Codes" && (
                                        <StyledTd>
                                            <ButtonRowWrapper>
                                                <Link to={`${cUpdate}${code.id}`}>
                                                    <StyledButton type="button">Edit</StyledButton>
                                                </Link>
                                                <StyledButton type="button" onClick={() => handleDelete(code.id)}>
                                                    Delete
                                                </StyledButton>
                                            </ButtonRowWrapper>
                                        </StyledTd>
                                    )}
                                </StyledTr>
                                <RowSpacer />
                            </React.Fragment>
                        ))}
                </tbody>
                <tfoot>
                    <tr>
                        <td colSpan={headers.length}>
                            <ConstructPagination
                                element={codes}
                                url={isAllowed && (role === "Admin" || role === "Support") ? cList : (!isAllowed && kind !== "visible Codes" ? cOwn : cOthers)}
                                page={Number(page)}
                                recordPerPage={recordPerPage}
                                setRecordPerPage={setRecordPerPage}
                                paginationSlice={paginationSlice}
                                setPaginationSlice={setPaginationSlice}
                                totalPages={Math.ceil(updatedCodes.length / recordPerPage)}
                            />
                        </td>
                    </tr>
                </tfoot>
            </StyledTable>
            {showDeleteModal && (
                <BlurredOverlayWrapper>
                    <ModalContainer>
                        <StyledModalContainer>
                            <TextContainer>
                                <Modal.Header closeButton>
                                    <ModalTitle>Delete Confirmation</ModalTitle>
                                </Modal.Header>
                                <ModalBody>
                                    Are you sure you want to delete this code?
                                </ModalBody>
                            </TextContainer>
                            <Modal.Footer>
                                <RowButtonWithTopMarginContainer>
                                    <StyledButton onClick={confirmDelete}>
                                        Delete
                                    </StyledButton>
                                    <StyledButton onClick={() => setShowDeleteModal(false)}>
                                        Cancel
                                    </StyledButton>
                                </RowButtonWithTopMarginContainer>
                            </Modal.Footer>
                        </StyledModalContainer>
                    </ModalContainer>
                </BlurredOverlayWrapper>
            )}
            {showPreviewModal && (
                <BlurredOverlayWrapper>
                    <PreviewModalContainer>
                        <StyledModalContainer>
                            <CodeTextContainer>
                                <Modal.Header closeButton>
                                    <ModalTitle>Code Preview</ModalTitle>
                                </Modal.Header>
                                <ModalBody>
                                    <Editor
                                        height={editorMeasurePreview[0]}
                                        width={editorMeasurePreview[1]}
                                        defaultLanguage={previewCode.whatKindOfCode.toLowerCase().replace(/#/g, "sharp")}
                                        defaultValue={previewCode.myCode}
                                        name="mycode"
                                        id="mycode"
                                        autoComplete="off"
                                        onMount={(editor, monaco) => handleEditorDidMount(editor, monaco, editorRef)}
                                        options={{ readOnly: false, fontSize: fontSize }}
                                        theme={theme}
                                    />
                                    <>
                                        <EditorLabel htmlFor="fontSizeSelector"> Font Size: </EditorLabel>
                                        <EditorSelect id="fontSizeSelector" onChange={(e) => changeFontSize(e, setFontSize, username)} value={fontSize}>
                                            {Array.from({ length: 23 }, (_, i) => i + 8).map(size => (
                                                <option key={size} value={size}>{size}</option>
                                            ))}
                                        </EditorSelect>
                                        <EditorLabel htmlFor="themeSelector"> Change Theme: </EditorLabel>
                                        <EditorSelect id="themeSelector" onChange={(e) => changeTheme(e, setTheme, username)} value={theme}>
                                            <EditorOption value="vs">Light</EditorOption>
                                            <EditorOption value="vs-dark">Dark</EditorOption>
                                        </EditorSelect>
                                    </>
                                </ModalBody>
                            </CodeTextContainer>
                            <ButtonRowWrapper>
                                <StyledButton onClick={() => copyContentToClipboard(editorRef)}>Copy to Clipboard</StyledButton>
                                <StyledButton onClick={() => toggleFullscreen(editorRef, originalEditorMeasurePreview, setEditorMeasurePreview)}>Fullscreen</StyledButton>
                                <StyledButton onClick={() => setShowPreviewModal(false)}>
                                    Close
                                </StyledButton>
                            </ButtonRowWrapper>
                        </StyledModalContainer>
                    </PreviewModalContainer>
                </BlurredOverlayWrapper>
            )}
        </TableContainer>
    );
};
