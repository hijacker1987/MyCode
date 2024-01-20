import styled from "styled-components";

export const Form = styled.form`
  margin-left: 0px;
  :-webkit-autofill,
  :-webkit-autofill:hover,
  :-webkit-autofill:focus,
  :-webkit-autofill:active {
    -webkit-text-fill-color: #e81cff !important;
    -webkit-box-shadow: 0 0 0px 1000px black inset !important;
    transition: background-color 5000s ease-in-out 0s;
  }
`;

export const FormRow = styled.div`
  display: grid;
  grid-template-columns: 1fr 2fr;
  grid-template-rows: 1fr;
  grid-column-gap: 10px;
  grid-row-gap: 0px;
`;
