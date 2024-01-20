import React from 'react';
import { Link, useNavigate } from 'react-router-dom';
import { formatElapsedTime } from '../../../Services/elapsedTime';
import { uUpdate } from '../../../Services/frontend.endpoints';
import { TableContainer } from '../../Styles/TableContainer.styled';
import { StyledTable, StyledTh, StyledTr, StyledTd, RowSpacer } from '../../Styles/TableRow.styled';
import { ButtonContainer } from '../../Styles/ButtonContainer.styled';

const UsersTable = ({ users, headers }) => {
    const navigate = useNavigate();

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
                        <React.Fragment key={user.id}>
                            <StyledTr className={index % 2 === 1 ? "even-row" : "odd-row"}>
                                <StyledTd>{index + 1}</StyledTd>
                                <StyledTd>{user.displayName}</StyledTd>
                                <StyledTd>{formatElapsedTime(user.lastTimeLogin)}</StyledTd>
                                <StyledTd>{user.userName}</StyledTd>
                                <StyledTd>{user.email}</StyledTd>
                                <StyledTd>{user.phoneNumber}</StyledTd>
                                <StyledTd>
                                    <ButtonContainer type="button">
                                        <Link to={`${uUpdate}${user.id}`} >
                                            Edit
                                        </Link>
                                    </ButtonContainer>
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
