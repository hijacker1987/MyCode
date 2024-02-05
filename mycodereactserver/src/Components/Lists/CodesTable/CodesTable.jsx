import React, { useState, useEffect } from "react";
import ConstructPagination from "../../Forms/PaginationForm/index";
import DeleteActions from "../../Delete/DeleteActions";
import Modal from 'react-bootstrap/Modal';
import Notify from "./../../../Pages/Services/ToastNotifications";
import { Link } from "react-router-dom";
import { cList, cOthers, cUpdate } from "../../../Services/frontend.endpoints";
import { deleteCode, deleteSuperCode } from "../../../Services/Backend.Endpoints";
import { TextContainer } from "../../Styles/TextContainer.styled";
import { ButtonRowContainer } from "../../Styles/ButtonRow.styled";
import { TableContainer } from "../../Styles/TableContainer.styled";
import { ButtonContainer } from "../../Styles/ButtonContainer.styled";
import { StyledTable, StyledTh, StyledTr, StyledTd, RowSpacer } from "../../Styles/TableRow.styled";
import { BlurredOverlay, ModalContainer, StyledModal } from "../../Styles/Background.styled";

const CodesTable = ({ codes, headers, kind, role, page, auth }) => {
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
    const isAllowed = auth === "byAuth";

    useEffect(() => {
        const initialPage = page ? Math.max(1, Number(page)) : 1;

        setPaginationSlice({ first: initialPage * recordPerPage - recordPerPage, second: initialPage * recordPerPage });
    }, [page, recordPerPage, displayNameFilter, codeTypeFilter, visibilityFilter]);

    useEffect(() => {
        setUpdatedCodes(codes);
    }, [codes]);

    if (!updatedCodes || updatedCodes.length === 0) {
        return <p>No code data available.</p>;
    }

    const handleSort = () => {
        const sortedCodes = [...updatedCodes].sort((a, b) => {
            if (sortOrder === "A-Z") {
                setSortOrder("Z-A");
                return !isAllowed ? a.codeTitle.localeCompare(b.codeTitle) : a.displayName.localeCompare(b.displayName);
            } else {
                setSortOrder("A-Z");
                return !isAllowed ? b.codeTitle.localeCompare(a.codeTitle) : b.displayName.localeCompare(a.displayName);
            }
        });
        setUpdatedCodes(sortedCodes);
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
            }
        );
    };

    return (
        <TableContainer>
            <StyledTable className="table table-striped table-hover">
                <thead>
                    <tr>
                        <StyledTh>
                            <input
                                type="text"
                                placeholder={!isAllowed ? "Search by Code Title" : "Search by Display Name"}
                                value={!isAllowed ? codeTitleFilter : displayNameFilter}
                                onChange={(e) => (!isAllowed ? setCodeTitleFilter(e.target.value) : setDisplayNameFilter(e.target.value))}
                            />
                        </StyledTh>
                        <StyledTh>
                            <select value={codeTypeFilter} onChange={(e) => setCodeTypeFilter(e.target.value)}>
                                <option value="">All Code Types</option>
                                <option value="C#">C#</option>
                                <option value="C++">C++</option>
                            </select>
                        </StyledTh>
                        {!isAllowed && (
                            <StyledTh>
                                <select value={visibilityFilter} onChange={(e) => setVisibilityFilter(e.target.value)}>
                                    <option value="all">All</option>
                                    <option value="visible">Visible</option>
                                    <option value="hidden">Hidden</option>
                                </select>
                            </StyledTh>
                        )}
                        <StyledTh onClick={handleSort}>
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
                                if (!isAllowed) {
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
                                        {isAllowed && (
                                            <StyledTd>{code.displayName}</StyledTd>
                                        )}
                                    <StyledTd>{code.codeTitle}</StyledTd>
                                    <StyledTd>{code.myCode}</StyledTd>
                                    <StyledTd>{code.whatKindOfCode}</StyledTd>
                                    <StyledTd>{code.isBackend ? "Backend" : "Frontend"}</StyledTd>
                                    {kind !== "visible Codes" && (
                                        <StyledTd>{code.isVisible ? "Yes" : "Hidden"}</StyledTd>
                                    )}
                                    {!isAllowed && (
                                        <StyledTd>
                                            <Link to={`${cUpdate}${code.id}`}>
                                                <ButtonContainer type="button">Edit</ButtonContainer>
                                            </Link>
                                            <ButtonContainer type="button" onClick={() => handleDelete(code.id)}>
                                                Delete
                                            </ButtonContainer>
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
                                url={auth === "byAuth" ? cList : cOthers}
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
                <BlurredOverlay>
                    <ModalContainer>
                        <StyledModal>
                            <TextContainer>
                                <Modal.Header closeButton>
                                    <Modal.Title>Delete Confirmation</Modal.Title>
                                </Modal.Header>
                                <Modal.Body>
                                    Are you sure you want to delete this code?
                                </Modal.Body>
                            </TextContainer>
                            <Modal.Footer>
                                <ButtonRowContainer>
                                    <ButtonContainer onClick={() => setShowDeleteModal(false)}>
                                        Cancel
                                    </ButtonContainer>
                                    <ButtonContainer onClick={confirmDelete}>
                                        Delete
                                    </ButtonContainer>
                                </ButtonRowContainer>
                            </Modal.Footer>
                        </StyledModal>
                    </ModalContainer>
                </BlurredOverlay>
            )}
        </TableContainer>
    );
};

export default CodesTable;
