import styled from "styled-components";

const baseFormStyles = `
  :-webkit-autofill,
  :-webkit-autofill:hover,
  :-webkit-autofill:focus,
  :-webkit-autofill:active {
    -webkit-text-fill-color: #e81cff !important;
    -webkit-box-shadow: 0 0 0px 1000px black inset !important;
    transition: background-color 5000s ease-in-out 0s;
  }
`;

export const Form = styled.form`
  margin-left: 0px;
  ${baseFormStyles}
`;

export const CodeFormStyle = styled.form`
  margin-top: 30%;
  margin-left: 7%;
  ${baseFormStyles}
`;

export const FormRow = styled.div`
  display: grid;
  margin-left: 36%;
  width: 500px;
  grid-template-columns: 1fr 2fr;
  grid-template-rows: auto;
  grid-column-gap: 10px;
  grid-row-gap: 0px;
`;

export const FormColumn = styled.div`
  display: grid;
  margin-top: -30%;
  margin-left: 20.5%;
  grid-template-columns: auto;
  grid-row-gap: 10px;
`;

export const InputForm = styled.input`
  position: relative;
  margin-top: 2.5px;
  margin-left: 4px;
  width: 201px;
  height: 37px;
  background: black !important;
  color: #e81cff;
  border-radius: 8px;
  font-family: Vancouver, sans-serif;
  font-size: 15px;
  font-weight: bold;
`;
