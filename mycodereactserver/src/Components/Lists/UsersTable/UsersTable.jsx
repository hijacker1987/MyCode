import React, { useState } from "react";
import DeleteActions from "../../Delete/DeleteActions";
import { Link } from "react-router-dom";
import { formatElapsedTime } from "../../../Services/elapsedTime";
import { uUpdate } from "../../../Services/frontend.endpoints";
import { deleteSuperUser } from "../../../Services/Backend.Endpoints";
import { TableContainer } from "../../Styles/TableContainer.styled";
import { StyledTable, StyledTh, StyledTr, StyledTd, RowSpacer } from "../../Styles/TableRow.styled";
import { ButtonContainer } from "../../Styles/ButtonContainer.styled";

const UsersTable = ({ users, headers, role }) => {
    const [updatedUsers, setUpdatedUsers] = useState(users);

    if (!updatedUsers || updatedUsers.length === 0) {
        return <p>No user data available.</p>;
    }

    const handleDelete = (userId) => {

        if (role === "Admin") {
            DeleteActions.deleteRecord(
                `${deleteSuperUser}${userId}`,
                () => {
                    console.log("User deleted successfully");
                    setUpdatedUsers((prevUsers) => prevUsers.filter((user) => user.id !== userId));
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
                        updatedUsers.map((user, index) => (
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
            </StyledTable>
        </TableContainer>
    );
};

export default UsersTable;
