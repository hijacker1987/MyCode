import styled from "styled-components";

export const EditorRowButtonContainer = styled.div`
    margin-top: 3%;
    margin-left: 9%;
    display: flex;
    gap: 20px;
`;

export const EditorLabel = styled.label`
  color: darkgreen;
  font-size: 14px;
`;

export const EditorMiddleLabel = styled.label`
  color: white;
  font-size: 14px;
`;

export const EditorOption = styled.option`
  margin-top: 3px;
  width: 207px;
  height: 40px;
  font-size: 18px;
  text-align: center;
  background: black;
  color: #e81cff;
  border-radius: 8px;
`;

export const EditorSelect = styled.select`
  margin-top: 3px;
  margin-left: 4px;
  width: 80px;
  height: 30px;
  font-size: 16px;
  text-align: center;
  background: black;
  color: limegreen;
  border-radius: 8px;
`;

export const EditorSelectBig = styled.select`
  margin-top: 3px;
  margin-left: 4px;
  width: 208px;
  height: 40px;
  font-size: 16px;
  text-align: center;
  background: black;
  color: limegreen;
  border-radius: 8px;
`;

export const EditorCheckbox = styled.input.attrs({ type: "checkbox" })`
  margin-left: 86px;
  width: 40px;
  height: 40px;
  cursor: pointer;
  appearance: none;
  background-color: black;
  border-radius: 4px;

  display: flex;
  align-items: center;
  justify-content: center;

  &:checked {
    background-color: black;
  }

  &:checked::after {
    content: "✔";
    color: #e81cff;
    font-size: 24px;
  }
`;