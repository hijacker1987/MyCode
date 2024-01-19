import styled from "styled-components";

export const TextContainer = styled.div`
  margin-top: 24px;
  margin-bottom: 25px;
  position: relative;
  width: 95px;
  height: 17px;
  background-color: #000;
  display: flex;
  align-items: center;
  color: green;
  flex-direction: column;
  justify-content: center;
  border: none;
  padding: 12px;
  gap: 12px;
  border-radius: 8px;

  &::before {
    content: '';
    position: absolute;
    inset: 0;
    left: -4px;
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

export const TextWrapper = styled.div`
  margin-top: -600px;
  margin-left: 50px;
  margin-right: 1535px;
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  grid-column-gap: 0px;
  grid-row-gap: 0px;
`;