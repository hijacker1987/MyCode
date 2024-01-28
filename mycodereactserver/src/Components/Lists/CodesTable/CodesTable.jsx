import React, { useState, useEffect } from "react";
import ConstructPagination from "../../Forms/PaginationForm/index";
import DeleteActions from "../../Delete/DeleteActions";
import { Link } from "react-router-dom";
import { cList, cUpdate } from "../../../Services/frontend.endpoints";
import { deleteCode, deleteSuperCode } from "../../../Services/Backend.Endpoints";
import { TableContainer } from "../../Styles/TableContainer.styled";
import { ButtonContainer } from "../../Styles/ButtonContainer.styled";
import { StyledTable, StyledTh, StyledTr, StyledTd, RowSpacer } from "../../Styles/TableRow.styled";

const CodesTable = ({ codes, headers, role, page, type }) => {
    const [updatedCodes, setUpdatedCodes] = useState(codes);
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
        const deleteUrl = role === "Admin" ? deleteSuperCode : deleteCode;

        DeleteActions.deleteRecord(
            `${deleteUrl}${codeId}`,
            () => {
                console.log("Code deleted successfully");
                setUpdatedCodes((prevCodes) => prevCodes.filter((code) => code.id !== codeId));
            },
            () => {
                console.error("Error deleting code");
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
                                            <ButtonContainer type="button" onClick={() => handleDelete(code.id)}>Delete</ButtonContainer>
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
        </TableContainer>
    );
};

export default CodesTable;
