import React, { useState, useEffect } from "react";
import ConstructPagination from "../../Forms/PaginationForm/index";
import DeleteActions from "../../Delete/DeleteActions";
import Modal from 'react-bootstrap/Modal';
import Notify from "./../../../Pages/Services/ToastNotifications";
import { Link } from "react-router-dom";
import { cList, cUpdate } from "../../../Services/frontend.endpoints";
import { deleteCode, deleteSuperCode } from "../../../Services/Backend.Endpoints";
import { TextContainer } from "../../Styles/TextContainer.styled";
import { ButtonRowContainer } from "../../Styles/ButtonRow.styled";
import { TableContainer } from "../../Styles/TableContainer.styled";
import { ButtonContainer } from "../../Styles/ButtonContainer.styled";
import { StyledTable, StyledTh, StyledTr, StyledTd, RowSpacer } from "../../Styles/TableRow.styled";
import { BlurredOverlay, ModalContainer, StyledModal } from "../../Styles/Background.styled";

const CodesTable = ({ codes, headers, role, page, type }) => {
    const [updatedCodes, setUpdatedCodes] = useState(codes);
    const [showDeleteModal, setShowDeleteModal] = useState(false);
    const [selectedCodeId, setSelectedCodeId] = useState(null);
    const [recordPerPage, setRecordPerPage] = useState(5);
    const [paginationSlice, setPaginationSlice] = useState({ first: 0, second: recordPerPage - 1 });
    const isAllowed = type === "byAuth";

    useEffect(() => {
        const initialPage = page ? Math.max(1, Number(page)) : 1;

        setPaginationSlice({ first: initialPage * recordPerPage - recordPerPage, second: initialPage * recordPerPage });
    }, [page, recordPerPage]);

    useEffect(() => {
        setUpdatedCodes(codes);
    }, [codes]);

    if (!updatedCodes || updatedCodes.length === 0) {
        return <p>No code data available.</p>;
    }

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
                        {headers.map((header) => (
                            <StyledTh key={header}>{header}</StyledTh>
                        ))}
                    </tr>
                </thead>
                <tbody>
                    {updatedCodes &&
                        updatedCodes.slice(paginationSlice.first, paginationSlice.second).map((code, index) => (
                            <React.Fragment key={code.id}>
                                <StyledTr className={index % 2 === 1 ? "even-row" : "odd-row"}>
                                    <StyledTd>{index + 1}</StyledTd>
                                    {!isAllowed && (
                                        <StyledTd>{code.userName}</StyledTd>
                                    )}
                                    <StyledTd>{code.codeTitle}</StyledTd>
                                    <StyledTd>{code.myCode}</StyledTd>
                                    <StyledTd>{code.whatKindOfCode}</StyledTd>
                                    <StyledTd>{code.isBackend ? "Backend" : "Frontend"}</StyledTd>
                                    <StyledTd>{code.isVisible ? "Yes" : "Hidden"}</StyledTd>
                                    {isAllowed && (
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
                                url={cList}
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
