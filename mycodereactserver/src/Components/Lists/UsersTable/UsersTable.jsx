import React from 'react';
import { formatElapsedTime } from '../../../Services/elapsedTime'
import { TableContainer } from '../../Styles/TableContainer.styled';
import { StyledTable, StyledTh, StyledTr, StyledTd, RowSpacer } from '../../Styles/TableRow.styled';

const UsersTable = ({ users, headers }) => {

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
                    {users && users.map((user, index) => (
                        <React.Fragment key={user.email}>
                            <StyledTr className={index % 2 === 1 ? "even-row" : "odd-row"}>
                                <StyledTd>{index + 1}</StyledTd>
                                <StyledTd>{user.displayName}</StyledTd>
                                <StyledTd>{formatElapsedTime(user.lastTimeLogin)}</StyledTd>
                                <StyledTd>{user.userName}</StyledTd>
                                <StyledTd>{user.email}</StyledTd>
                                <StyledTd>{user.phoneNumber}</StyledTd>
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
