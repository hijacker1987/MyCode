import React from "react";
import { Link } from "react-router-dom";
import { cUpdate } from "../../../Services/frontend.endpoints";
import { TableContainer } from "../../Styles/TableContainer.styled";
import { StyledTable, StyledTh, StyledTr, StyledTd, RowSpacer } from "../../Styles/TableRow.styled";
import { ButtonContainer } from "../../Styles/ButtonContainer.styled";

const CodesTable = ({ codes, headers }) => {
    if (!codes || codes.length === 0) {
        return <p>No code data available.</p>;
    }

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
                            <React.Fragment key={code.id}>
                                <StyledTr className={index % 2 === 1 ? "even-row" : "odd-row"}>
                                    <StyledTd>{index + 1}</StyledTd>
                                    <StyledTd>{code.codeTitle}</StyledTd>
                                    <StyledTd>{code.myCode}</StyledTd>
                                    <StyledTd>{code.whatKindOfCode}</StyledTd>
                                    <StyledTd>{code.isBackend ? "Backend" : "Frontend"}</StyledTd>
                                    <StyledTd>{code.isVisible ? "Yes" : "Hidden"}</StyledTd>
                                    <StyledTd>
                                        <Link to={`${cUpdate}${code.id}`}>
                                            <ButtonContainer type="button">Edit</ButtonContainer>
                                        </Link>
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

export default CodesTable;
