import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { StyledUl, StyledLi } from "../../Styles/Pagination.styled";

const ConstructPagination = ({ element, url, page, recordPerPage, setRecordPerPage, paginationSlice, setPaginationSlice }) => {
    const [availablePages, setAvailablePages] = useState([]);
    const navigate = useNavigate();

    const totalPages = element ? Math.ceil(element.length / recordPerPage) : 0;

    useEffect(() => {
        const initialPage = page ? Math.max(1, Number(page)) : 1;

        setPaginationSlice({ first: initialPage * recordPerPage - recordPerPage, second: initialPage * recordPerPage });

        const totalPages = element ? Math.ceil(element.length / recordPerPage) : 0;
        const pages = Array.from({ length: totalPages }, (_, index) => index + 1);

        setAvailablePages(pages);
    }, [page, recordPerPage, element, totalPages]);

    const handleLastButton = () => {
        navigate(`${url}${totalPages}`);
    };

    const handlePreviousButton = () => {
        const prevPage = Math.max(1, Number(page) - 1);
        navigate(`${url}${prevPage}`);
    };

    const handleNextButton = () => {
        const nextPage = Math.min(totalPages, Number(page) + 1);
        navigate(`${url}${nextPage}`);
    };

    const handleRecordPerPageButton = (number) => {
        setRecordPerPage(Number(number));

        navigate(`${url}${Math.max(1, Math.ceil(paginationSlice.second / Number(number)))}`);

        setPaginationSlice((prevSlice) => {
            return {
                ...prevSlice,
                second: Number(number)
            };
        });
    };

    return (
        <nav aria-label="Page navigation example">
            <StyledUl className="pagination">
                <li className="page-item">
                    <button className="btn btn-primary btn-floating mb-4" style={{ marginLeft: "1vw", color: "black" }} onClick={handlePreviousButton} disabled={page === 1}>
                        Previous
                    </button>
                </li>
                {availablePages.map((pageNumber, index) => {
                    const isCurrentPage = pageNumber === Number(page);

                    if (index === 0 || (index >= Math.max(1, page - 2) && index <= Math.min(totalPages, page))) {
                        return (
                            <li key={index} className="page-item">
                                <button
                                    className={`btn btn-primary btn-floating mb-4 ${isCurrentPage ? 'current-page' : ''}`}
                                    style={{ marginLeft: "1vw", color: isCurrentPage ? "white" : "black" }}
                                    onClick={() => navigate(`${url}${pageNumber}`)}
                                    disabled={isCurrentPage}
                                >
                                    {pageNumber}
                                </button>
                            </li>
                        );
                    } else if ((index === 1 && page > 3) || (index === availablePages.length - 2 && page < totalPages - 2)) {
                        return (
                            <li key={index} className="page-item">
                                <span style={{ marginLeft: "1vw", color: "black" }}>...</span>
                            </li>
                        );
                    }
                    return null;
                })}
                <li className="page-item">
                    <button className="btn btn-primary btn-floating mb-4" style={{ marginLeft: "1vw", color: "black" }} onClick={handleNextButton} disabled={page === totalPages}>
                        Next
                    </button>
                </li>
                <li className="page-item">
                    <button className="btn btn-primary btn-floating mb-4" style={{ marginLeft: "1vw", color: "black" }} onClick={handleLastButton} disabled={page === totalPages}>
                        Last
                    </button>
                </li>
                <StyledLi>
                    <select className="btn btn-primary" style={{ marginRight: "1vw" }} onChange={(event) => handleRecordPerPageButton(event.target.value)} value={recordPerPage}>
                        <option>5</option>
                        <option>10</option>
                        <option>25</option>
                        <option>50</option>
                        <option>100</option>
                    </select>
                </StyledLi>
            </StyledUl>
        </nav>
    );
};

export default ConstructPagination;
