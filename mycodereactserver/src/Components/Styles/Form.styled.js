import styled from "styled-components";

export const Form = styled.form`
  margin-left: 50px;
`;

export const FormRow = styled.div`
  display: grid;
  grid-template-columns: repeat(2, 1fr); /* Adjust the number of columns */
  grid-column-gap: 20px; /* Adjust the gap as needed */
  margin-bottom: 20px; /* Add margin between form rows */
`;
