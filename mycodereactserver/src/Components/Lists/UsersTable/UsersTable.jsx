import React, { useState, useEffect } from "react";
import { Link } from "react-router-dom";
import Modal from "react-bootstrap/Modal";

import { Notify } from "./../../../Pages/Services";
import { formatElapsedTime, formattedTime } from "../../../Services/ElapsedTime";
import { getApi } from "../../../Services/Api";
import { uList, uUpdate } from "../../../Services/Frontend.Endpoints";
import { getAnyArc, getMessage, deleteSuperUser } from "../../../Services/Backend.Endpoints";
import ConstructPagination from "../../Forms/PaginationForm/index";
import DeleteActions from "../../Delete/index";

import { TextContainer } from "../../Styles/TextContainer.styled";
import { ButtonRowContainer } from "../../Styles/ButtonRow.styled";
import { TableContainer } from "../../Styles/TableContainer.styled";
import { ButtonContainer } from "../../Styles/ButtonContainer.styled";
import { BlurredOverlay, ModalContainer, StyledModal } from "../../Styles/Background.styled";
import { StyledTable, StyledTh, StyledTr, StyledTd, RowSpacer } from "../../Styles/TableRow.styled";

export const UsersTable = ({ users, headers, role, page }) => {
    const [updatedUsers, setUpdatedUsers] = useState(users);
    const [selectedUserMessages, setSelectedUserMessages] = useState([]);
    const [showMessagesModal, setShowMessagesModal] = useState(false);
    const [usersWithMessagesId, setUsersWithMessagesId] = useState([]);
    const [showDeleteModal, setShowDeleteModal] = useState(false);
    const [userToDeleteId, setUserToDeleteId] = useState(null);
    const [recordPerPage, setRecordPerPage] = useState(5);
    const [paginationSlice, setPaginationSlice] = useState({ first: 0, second: recordPerPage - 1 });
    const [displayNameFilter, setDisplayNameFilter] = useState("");
    const [emailFilter, setEmailFilter] = useState("");
    const [usernameFilter, setUsernameFilter] = useState("");
    const [sortOrder, setSortOrder] = useState("A-Z");

    useEffect(() => {
        const initialPage = page ? Math.max(1, Number(page)) : 1;

        setPaginationSlice({ first: initialPage * recordPerPage - recordPerPage, second: initialPage * recordPerPage });
    }, [page, recordPerPage]);

    useEffect(() => {
        if (users != null) {
            const updatedUsers = users.map(({ user, role }) => ({ ...user, role }));
            setUpdatedUsers(updatedUsers);
        }
    }, [users]);

    useEffect(() => {
        const getOldMessages = async () => {
            if (users != null) {
                try {
                    const response = await getApi(getAnyArc);
                    setUsersWithMessagesId(response);
                } catch (error) {
                    console.error("Failed to get data: ", error);
                }
            }
        }

        getOldMessages();
    }, [users]);

    const getOldMessagesByIdModal = async (id) => {
        try {
            const data = await getApi(`${getMessage}${id}`);
            if (selectedUserMessages.length != 0) {
                setSelectedUserMessages([]);
            }
            data.forEach(item => {
                setSelectedUserMessages(prevMessages => [...prevMessages, { user: item.whom, message: item.text, time: item.when }]);
            });
            setShowMessagesModal(true);
        } catch (error) {
            console.error("Failed to get data: ", error);
        }
    }

    if (!updatedUsers || updatedUsers.length === 0) {
        return <p>No user data available.</p>;
    }

    const handleSort = () => {
        const sortedUsers = [...updatedUsers].sort((a, b) => {
            if (sortOrder === "A-Z") {
                setSortOrder("Z-A");
                return b.displayName.localeCompare(a.displayName);
            } else {
                setSortOrder("A-Z");
                return a.displayName.localeCompare(b.displayName);
            }
        });
        setUpdatedUsers(sortedUsers);
    };

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
                    Notify("Success", "Successfully deleted the user!");
                    setUpdatedUsers((prevUsers) => prevUsers.filter((user) => user.id !== userToDeleteId));
                    setShowDeleteModal(false);
                },
                () => {
                    Notify("Error", "Unable to delete the user!");
                }
            );
        }
    };

    return (
        <TableContainer>
            <StyledTable className="table table-striped table-hover">
                <thead>
                    <tr>
                        <StyledTh className="search-1">
                            <input
                                type="text"
                                placeholder="Search by Display Name"
                                value={displayNameFilter}
                                onChange={(e) => setDisplayNameFilter(e.target.value)}
                                style={{ cursor: "pointer" }}
                            />
                        </StyledTh>

                        <StyledTh className="search-2">
                            <input
                                type="text"
                                placeholder="Search by Email"
                                value={emailFilter}
                                onChange={(e) => setEmailFilter(e.target.value)}
                                style={{ cursor: "pointer" }}
                            />
                        </StyledTh>

                        <StyledTh className="search-3">
                            <input
                                type="text"
                                placeholder="Search by Username"
                                value={usernameFilter}
                                onChange={(e) => setUsernameFilter(e.target.value)}
                                style={{ cursor: "pointer" }}
                            />
                        </StyledTh>

                        <StyledTh className="search-4" onClick={handleSort} style={{ cursor: "pointer" }}>
                            {sortOrder}
                        </StyledTh>
                    </tr>
                    <tr>
                        {headers.map(header => (
                            <StyledTh key={header}>{header}</StyledTh>
                        ))}
                    </tr>
                </thead>
                <tbody>
                    {updatedUsers &&
                        updatedUsers
                            .filter((user) => {
                                return (
                                    user.displayName && user.displayName.toLowerCase().includes(displayNameFilter.toLowerCase()) &&
                                    user.email && user.email.toLowerCase().includes(emailFilter.toLowerCase()) &&
                                    user.userName && user.userName.toLowerCase().includes(usernameFilter.toLowerCase())
                                );
                            })
                            .slice(paginationSlice.first, paginationSlice.second)
                            .map((user, index) => (
                            <React.Fragment key={user.id}>
                                <StyledTr className={index % 2 === 1 ? "even-row" : "odd-row"}>
                                    <StyledTd>{index + 1}</StyledTd>
                                    <StyledTd>{user.displayName}</StyledTd>
                                    <StyledTd>{formatElapsedTime(user.lastTimeLogin)}</StyledTd>
                                    <StyledTd>{user.userName}</StyledTd>
                                    <StyledTd>{user.email}</StyledTd>
                                    <StyledTd>{user.phoneNumber}</StyledTd>
                                    <StyledTd>{user.role}</StyledTd>
                                    {role === "Admin" && (
                                    <>
                                        <StyledTd>
                                            <Link to={`${uUpdate}${user.id}`} >
                                                <ButtonContainer type="button">Edit</ButtonContainer>
                                            </Link>
                                            <ButtonContainer type="button" onClick={() => handleDelete(user.id)}>Delete</ButtonContainer>
                                        </StyledTd>
                                    </>
                                    )}
                                        <StyledTd>
                                            {usersWithMessagesId.includes(user.id.toUpperCase())
                                            ?
                                                <ButtonContainer type="button" onClick={() => getOldMessagesByIdModal(user.id)}>
                                                    Available
                                                </ButtonContainer>
                                            : ""}
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
                                
                            </TextContainer>
                            <Modal.Footer>
                                <ButtonRowContainer>
                                    <ButtonContainer onClick={() => setShowDeleteModal(false)}>
                                        Cancel
                                    </ButtonContainer>
                                </ButtonRowContainer>
                            </Modal.Footer>
                        </StyledModal>
                    </ModalContainer>
                </BlurredOverlay>
            )}
            {showMessagesModal && (
                <BlurredOverlay>
                    <ModalContainer>
                        <StyledModal size="lg">
                            <TextContainer>
                                <Modal.Header closeButton>
                                    <Modal.Title>Chat History</Modal.Title>
                                </Modal.Header>
                                <Modal.Body style={{ maxHeight: "400px", overflowY: "auto" }}>
                                    {selectedUserMessages
                                        .map((message, index) => (
                                            <React.Fragment key={index}>
                                                <StyledTr className={index % 2 === 1 ? "even-row" : "odd-row"}>
                                                    <StyledTd>{index + 1}</StyledTd>
                                                    <StyledTd>{message.user} - {message.message} at {formattedTime(message.time)}</StyledTd>
                                                </StyledTr>
                                                <RowSpacer />
                                            </React.Fragment>))}
                                </Modal.Body>
                            </TextContainer>
                            <Modal.Footer>
                                <ButtonRowContainer>
                                    <ButtonContainer onClick={() => setShowMessagesModal(false)}>
                                        Back
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
