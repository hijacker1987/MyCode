import React from 'react';
import { TableContainer } from '../../Styles/TableContainer.styled';
import { StyledTable, StyledTh, StyledTr, StyledTd, RowSpacer } from '../../Styles/TableRow.styled';

const CodesTable = ({ codes, headers }) => {
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
                    {codes &&
                        codes.map((code, index) => (
                            <React.Fragment key={code.codeTitle}>
                                <StyledTr className={index % 2 === 1 ? "even-row" : "odd-row"}>
                                    <StyledTd>{index + 1}</StyledTd>
                                    <StyledTd>{code.codeTitle}</StyledTd>
                                    <StyledTd>{code.myCode}</StyledTd>
                                    <StyledTd>{code.whatKindOfCode}</StyledTd>
                                    <StyledTd>{code.isBackend ? "Backend" : "Frontend"}</StyledTd>
                                    <StyledTd>{code.isVisible ? "Yes" : "Hidden"}</StyledTd>
                                </StyledTr>
                                <RowSpacer />
                            </React.Fragment>
                        ))}
                </tbody>
            </StyledTable>
        </TableContainer>
    );
};

export default CodesTable;
