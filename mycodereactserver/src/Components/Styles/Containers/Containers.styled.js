import styled from "styled-components";

export const AddCodeButtonRowButtonContainer = styled.div`
    display: flex;
    gap: 20px;
    margin-bottom: 10px;
`;

export const AddCodeButtonRowContainer = styled.div`
  margin-top: 30px;
  margin-left: 10px;
  margin-bottom: 10px;
  display: flex;
  flex-direction: row;
  gap: 20px;
`;

export const CBInputContainer = styled.div`
  position: fixed;
  top: 88%;
  width: 16.3%;
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: 10px;
`;

export const CBOutputContainer = styled.div`
  color: aliceblue;
  font-family: Vancouver, sans-serif;
  font-size: 17px;
  font-weight: bold;
  margin-bottom: 45px;
  overflow-y: scroll;
`;

export const CustomerChatInputContainer = styled.div`
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-top: 10px;
  position: relative;
`;

export const LayoutPositionContainer = styled.div`
  display: flex;
  justify-content: center;
  align-items: flex-start;
  width: 100%;
  padding-top: 50px;

  @media (max-width: 768px) {
    flex-direction: column;
  }
`;

export const RightBarButtonColumnContainer = styled.div`
  display: flex;
  gap: 20px;
  flex-direction: column;
  justify-content: space-between;
  position: fixed;
  right: 20px;
`;

export const RowButtonContainer = styled.div`
  display: flex;
  gap: 20px;
`;

export const RowButtonWithTopMarginContainer = styled.div`
  display: flex;
  margin-top: 20px;
  gap: 20px;
`;

export const VerificationColumnContainer = styled.div`
  display: grid;
  margin-top: 5%;
  margin-left: 25.5%;
  grid-template-columns: auto;
  grid-row-gap: 10px;
`;
