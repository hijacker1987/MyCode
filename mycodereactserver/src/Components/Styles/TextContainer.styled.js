import styled from "styled-components";

export const TextContainer = styled.div`
  margin-top: 23px;
  margin-bottom: 14px;
  position: relative;
  width: 98px;
  height: 16px;
  background-color: #000;
  display: flex;
  align-items: center;
  color: green;
  flex-direction: column;
  justify-content: center;
  border: none;
  padding: 13px;
  gap: 12px;
  border-radius: 8px;

  &::before {
    content: '';
    position: absolute;
    inset: 0;
    left: -2px;
    margin-top: -3px;
    width: 128px;
    height: 47px;
    border-radius: 10px;
    background: linear-gradient(-45deg, #e81cff 0%, #40c9ff 100%);
    z-index: -10;
    pointer-events: none;
    transition: all 0.6s cubic-bezier(0.175, 0.885, 0.32, 1.275);
  }
`;

export const ErrorTextContainerRed = styled.div`
  position: relative;
  top: 30%;
  left: 40%;
  width: auto;
  height: auto;
  background-color: #000;
  display: flex;
  align-items: center;
  color: red;
  flex-direction: column;
  justify-content: center;
  border: none;
  padding: 22px;
  gap: 12px;
  border-radius: 8px;
  text-align: center;
  gap: 12px;
  margin-top: 10px;
  border-radius: 8px;

  &::before {
    content: '';
    position: absolute;
    inset: 0;
    left: -2px;
    margin-top: -2px;
    margin-right: -2px;
    width: auto;
    height: auto;
    border-radius: 10px;
    background: linear-gradient(-45deg, #e81cff 0%, #40c9ff 100%);
    z-index: -10;
    pointer-events: none;
    transition: all 0.6s cubic-bezier(0.175, 0.885, 0.32, 1.275);
  }
`;

export const ErrorTextContainerWhite = styled.div`
  position: relative;
  top: 30%;
  left: 40%;
  width: auto;
  height: auto;
  background-color: #000;
  display: flex;
  align-items: center;
  color: white;
  flex-direction: column;
  justify-content: center;
  border: none;
  padding: 22px;
  gap: 12px;
  border-radius: 8px;
  text-align: center;
  gap: 12px;
  margin-top: 10px;
  border-radius: 8px;

  &::before {
    content: '';
    position: absolute;
    inset: 0;
    left: -2px;
    margin-top: -2px;
    margin-right: -2px;
    width: auto;
    height: auto;
    border-radius: 10px;
    background: linear-gradient(-45deg, #e81cff 0%, #40c9ff 100%);
    z-index: -10;
    pointer-events: none;
    transition: all 0.6s cubic-bezier(0.175, 0.885, 0.32, 1.275);
  }
`;

export const TextWrapper = styled.div`
  margin-top: -600px;
  margin-left: 50px;
  margin-right: 1535px;
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  grid-column-gap: 0px;
  grid-row-gap: 0px;
`;

export const ColumnTextWrapper = styled.div`
  margin-top: 50px;
  margin-left: 50px;
  display: flex;
  flex-direction: column;
  align-items: center;
`;