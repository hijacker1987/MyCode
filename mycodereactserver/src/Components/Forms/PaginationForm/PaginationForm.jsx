import React, { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { StyledUl, StyledLi } from "../../Styles/Pagination.styled";

const ConstructPagination = ({ element, url, page, recordPerPage, setRecordPerPage, paginationSlice, setPaginationSlice }) => {
    const navigate = useNavigate();

    const totalPages = element ? Math.ceil(element.length / recordPerPage) : 0;

    useEffect(() => {
        const initialPage = page ? Math.max(1, Number(page)) : 1;
        setPaginationSlice({ first: initialPage * recordPerPage - recordPerPage, second: initialPage * recordPerPage });
    }, [page, recordPerPage]);

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
        })
    }

    return (
        <nav aria-label="Page navigation example">
            <StyledUl className="pagination">
                <li className="page-item">
                    <button className="btn btn-primary btn-floating mb-4" style={{ marginLeft: "1vw" }} onClick={handlePreviousButton} disabled={page === 1}>
                        Previous
                    </button>
                </li>
                <li className="page-item">
                    <button className="btn btn-primary btn-floating mb-4" style={{ marginLeft: "1vw" }} onClick={handleNextButton} disabled={page === totalPages}>
                        Next
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
