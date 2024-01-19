import styled from "styled-components";

export const ButtonRowContainer = styled.div`
    margin-top: 50px;
    margin-bottom: 50px;
    margin-left: 50px;
    display: grid;
    grid-template-columns: repeat(12, 1fr);
    grid-template-rows: 1fr;
    grid-column-gap: 0px;
    grid-row-gap: 0px;
`

export const ButtonRowButtonContainer = styled.div`
    margin-top: 5px;
    margin-left: -70px;
    display: grid;
    grid-template-columns: repeat(2, 1fr);
    grid-template-rows: 1fr;
    gap: 20px;
`;