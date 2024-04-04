import styled from "styled-components";

export const ButtonContainer = styled.button`/
  margin-top: 20px;
  margin-right: 10px;
  position: relative;
  width: 120px;
  height: 40px;
  background-color: #000;
  display: flex;
  align-items: center;
  color: green;
  flex-direction: column;
  justify-content: center;
  border: none;
  padding: 12px;
  gap: 12px;
  font-family: Inter, sans-serif;
  font-size: 15px;
  font-weight: bold;
  border-radius: 8px;
  cursor: pointer;

  &::before {
    content: '';
    position: absolute;
    inset: 0;
    left: -4px;
    top: -1px;
    margin: auto;
    width: 128px;
    height: 48px;
    border-radius: 10px;
    background: linear-gradient(-45deg, #e81cff 0%, #40c9ff 100%);
    z-index: -10;
    pointer-events: none;
    transition: all 0.6s cubic-bezier(0.175, 0.885, 0.32, 1.275);
  }

  &::after {
    content: "";
    z-index: -1;
    position: absolute;
    inset: 0;
    background: linear-gradient(-45deg, #fc00ff 0%, #00dbde 100%);
    transform: translate3d(0, 0, 0) scale(0.95);
    filter: blur(20px);
  }

   &:hover {
    color: lime;
  }

  &:hover::after {
    filter: blur(30px);
  }

  &:hover::before {
    transform: rotate(-180deg);
  }

  &:active::before {
    transform: scale(0.7);
  }
`;

export const ButtonContainerWrapper = styled.div`
    display: center;
    justify-content: center;
    margin-top: -19.5%;
    margin-left: 105%;
`;

export const CancelButtonWrapper = styled.div`
  margin-top: 50px;
  margin-left: 130px;
`;
