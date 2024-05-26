import styled from "styled-components";

export const StyledTable = styled.table`
  width: 70%;
  border-collapse: collapse;
  margin-top: 10px;
  margin-left: 15%;
`;

export const StyledTh = styled.th`
  background-color: #f2f2f2;
  color: darkred;
  padding: 10px;
`;

export const StyledTr = styled.tr`
  &:nth-child(even) {
    background-color: #f9f9f9;
  }

  &:nth-child(odd) {
    background-color: #ffffff;
    color: black;
  }
`;

export const StyledTd = styled.td`
  padding: 10px 20px;
  text-align: center;
  border-bottom: 1px solid #ddd;
`;

export const EvenRow = styled(StyledTr)`
  color: darkgrey;
`;

export const OddRow = styled(StyledTr)`
  color: darkblue;
`;

export const RowSpacer = styled.tr`
  height: 10px;
  background-color: transparent;
`;

export const TableContainer = styled.div`
  position: flex;
  margin-top: 30px;
  overflow-x: auto;
`;

export const StyledUl = styled.ul`
  display: flex;
  justify-content: space-between;
  list-style: none;
  padding: 0;
  margin: 0;
`

export const StyledLi = styled.li`
  margin-left: auto;
`;
