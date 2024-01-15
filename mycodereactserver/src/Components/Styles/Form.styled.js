import styled from "styled-components";

export const Form = styled.form`
    margin-left: 50px;
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
    grid-template-columns: repeat(8, 1fr);
    grid-template-rows: 1fr;
    grid-column-gap: 0px;
    grid-row-gap: 0px;
`;
