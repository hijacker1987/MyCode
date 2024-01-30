import React, { useState, useEffect } from "react";
import ConstructPagination from "../../Forms/PaginationForm/index";
import DeleteActions from "../../Delete/DeleteActions";
import Modal from 'react-bootstrap/Modal';
import { Link } from "react-router-dom";
import { formatElapsedTime } from "../../../Services/elapsedTime";
import { uList, uUpdate } from "../../../Services/frontend.endpoints";
import { deleteSuperUser } from "../../../Services/Backend.Endpoints";
import { TextContainer } from "../../Styles/TextContainer.styled";
import { ButtonRowContainer } from "../../Styles/ButtonRow.styled";
import { TableContainer } from "../../Styles/TableContainer.styled";
import { ButtonContainer } from "../../Styles/ButtonContainer.styled";
import { BlurredOverlay, ModalContainer, StyledModal } from "../../Styles/Background.styled";
import { StyledTable, StyledTh, StyledTr, StyledTd, RowSpacer } from "../../Styles/TableRow.styled";

const UsersTable = ({ users, headers, role, page }) => {
    const [updatedUsers, setUpdatedUsers] = useState(users);
    const [showDeleteModal, setShowDeleteModal] = useState(false);
    const [userToDeleteId, setUserToDeleteId] = useState(null);
    const [recordPerPage, setRecordPerPage] = useState(5);
    const [paginationSlice, setPaginationSlice] = useState({ first: 0, second: recordPerPage - 1 });

    useEffect(() => {
        const initialPage = page ? Math.max(1, Number(page)) : 1;

        setPaginationSlice({ first: initialPage * recordPerPage - recordPerPage, second: initialPage * recordPerPage });
    }, [page, recordPerPage]);

    useEffect(() => {
        setUpdatedUsers(users);
    }, [users]);

    if (!updatedUsers || updatedUsers.length === 0) {
        return <p>No user data available.</p>;
    }

    const handleDelete = (userId) => {
        if (role === "Admin") {
            setUserToDeleteId(userId);
            setShowDeleteModal(true);
        }
    };

    const confirmDelete = () => {
        if (role === "Admin") {
            DeleteActions.deleteRecord(
                `${deleteSuperUser}${userToDeleteId}`,
                () => {
                    console.log("User deleted successfully");
                    setUpdatedUsers((prevUsers) => prevUsers.filter((user) => user.id !== userToDeleteId));
                    setShowDeleteModal(false);
                },
                () => {
                    console.error("Error deleting user");
                }
            );
        }
    };

    return (
        <TableContainer>
            <StyledTable className="table table-striped table-hover">
                <thead>
                    <tr>
                        {headers.map(header => (
                            <StyledTh key={header}>{header}</StyledTh>
                        ))}
                    </tr>
                </thead>
                <tbody>
                    {updatedUsers &&
                        updatedUsers.slice(paginationSlice.first, paginationSlice.second).map((user, index) => (
                        <React.Fragment key={user.id}>
                            <StyledTr className={index % 2 === 1 ? "even-row" : "odd-row"}>
                                <StyledTd>{index + 1}</StyledTd>
                                <StyledTd>{user.displayName}</StyledTd>
                                <StyledTd>{formatElapsedTime(user.lastTimeLogin)}</StyledTd>
                                <StyledTd>{user.userName}</StyledTd>
                                <StyledTd>{user.email}</StyledTd>
                                <StyledTd>{user.phoneNumber}</StyledTd>
                                <StyledTd>
                                    <Link to={`${uUpdate}${user.id}`} >
                                        <ButtonContainer type="button">Edit</ButtonContainer>
                                    </Link>
                                    <ButtonContainer type="button" onClick={() => handleDelete(user.id)}>Delete</ButtonContainer>
                                </StyledTd>
                            </StyledTr>
                            <RowSpacer />
                        </React.Fragment>
                    ))}
                </tbody>
                <tfoot>
                    <tr>
                        <td colSpan={headers.length}>
                            <ConstructPagination
                                element={users}
                                url={uList}
                                page={Number(page)}
                                recordPerPage={recordPerPage}
                                setRecordPerPage={setRecordPerPage}
                                paginationSlice={paginationSlice}
                                setPaginationSlice={setPaginationSlice}
                                totalPages={Math.ceil(updatedUsers.length / recordPerPage)}
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
                                    Are you sure you want to delete this user?
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

export default UsersTable;
