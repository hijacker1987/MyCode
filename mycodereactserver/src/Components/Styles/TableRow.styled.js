import styled from "styled-components";

export const StyledTable = styled.table`
  width: 100%;
  border-collapse: collapse;
  margin-top: 10px;
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
  padding: 10px;
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
