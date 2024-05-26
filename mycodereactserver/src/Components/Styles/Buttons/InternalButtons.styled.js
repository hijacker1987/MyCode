import styled from "styled-components";

export const StyledButton = styled.button`
  min-width: 120px;
  height: 40px;
  background-color: #000;
  display: flex;
  align-items: center;
  color: rgba(0, 255, 0, 0.6);
  justify-content: center;
  border: none;
  gap: 12px;
  font-family: Inter;
  font-size: 15px;
  font-weight: bold;
  border-radius: 8px;
  cursor: pointer;
  position: relative;

  &::before {
    content: '';
    position: absolute;
    inset: 0;
    left: -4px;
    top: -1px;
    margin: auto;
    width: calc(100% + 8px);
    height: 48px;
    border-radius: 10px;
    background: linear-gradient(-45deg, #e81cff 0%, #40c9ff 100%);
    z-index: -2;
    pointer-events: none;
    transition: all 0.6s cubic-bezier(0.175, 0.885, 0.32, 1.275);
  }

  &::after {
    content: "";
    z-index: -1;
    position: absolute;
    inset: 0;
    background: linear-gradient(-45deg, #fc00ff 0%, #00dbde 100%);
    transform: scale(0.95);
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

export const CustomerChatButton = styled.button`
  height: 38px;
  width: 80px;
  margin-right: 100px;
  background-color: rgba(40, 10, 50, 1.8);
  color: white;
  border: none;
  padding: 10px;
  cursor: pointer;
  font-family: Vancouver, sans-serif;
  font-size: 17px;
  font-weight: bold;

  &:hover {
    background-color: #0056b3;
  }
`;

export const ChuckBotButton = styled.button`
  height: 38px;
  width: calc(29% - 5px);
  margin-right: 2%;
  background-color: rgba(40, 10, 50, 1.8);
  color: white;
  border: none;
  padding: 10px;
  cursor: pointer;
  font-family: Vancouver, sans-serif;
  font-size: 17px;
  font-weight: bold;
`;

export const CBToggleButton = styled.button`
  position: fixed;
  bottom: 11px;
  right: 20px;
  z-index: 9999;
  background: black;
  color: darkgreen;
  font-family: "Segoe UI", Tahoma, Geneva, Verdana, sans-serif;
  font-size: 18px;
  cursor: pointer;
  border: none;
  padding: 10px;
  border-radius: 5px;

  &:hover {
    background-color: #393939;
    color: limegreen;
  }
`;
