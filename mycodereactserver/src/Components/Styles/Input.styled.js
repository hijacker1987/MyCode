import styled from "styled-components";

export const InputWrapper = styled.div`
  position: relative;
  margin-top: 21px;
  margin-left: 20px;
  margin-right: 21px;
  width: 215px;
  height: 47px;
  border-radius: 10px;
  background: linear-gradient(-45deg, #e81cff 0%, #40c9ff 100%);

  input[type="checkbox"] {
    width: 100px;
    height: 40px;
  }

  select[id="codetype"] {
    margin-top: 3px;
    width: 150px;
    height: 40px;
    font-size: 18px;
    text-align: center;
  }
`;

export const InputForm = styled.input`
  position: relative;
  margin-top: 2.5px;
  margin-left: 4px;
  width: 200px;
  height: 37px;
  background: black !important;
  color: #e81cff;
  border-radius: 8px;
  font-family: Vancouver, sans-serif;
  font-size: 15px;
  font-weight: bold;
`;

export const SelectForm = styled.select`
  position: relative;
  margin-top: 2.5px;
  margin-left: 4px;
  width: 200px;
  height: 37px;
  background: black !important;
  color: #e81cff;
  border-radius: 8px;
`;
