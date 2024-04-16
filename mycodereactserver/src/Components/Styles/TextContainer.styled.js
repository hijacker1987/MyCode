import styled, { keyframes } from "styled-components";

const wave = keyframes`
  0% {
    background-position: 0 0;
  }
  50% {
    background-position: 100% 0;
  }
  100% {
    background-position: 0 0;
  }
`;

export const TextContainer = styled.div`
  margin-top: 23px;
  margin-bottom: 14px;
  margin-left: 14px;
  position: relative;
  width: auto;
  height: auto;
  background-color: #000;
  display: flex;
  align-items: center;
  flex-direction: column;
  justify-content: center;
  border: none;
  padding: 5%;
  gap: 12px;
  border-radius: 8px;
  animation: ${wave} 10s infinite linear;
  color: lightgreen;
  font-family: Script, sans-serif;
  font-size: 15px;
  font-weight: bold;

  &::before {
    content: '';
    position: absolute;
    inset: 0;
    left: -2px;
    margin-top: -3px;
    width: 100.5%;
    height: 102%;
    border-radius: 10px;
    background: linear-gradient(45deg, #e81cff 0%, #40c9ff 100%);
    z-index: -10;
    pointer-events: none;
    transition: all 0.6s cubic-bezier(0.175, 0.885, 0.32, 1.275);
  }
`;

export const TextContainerWhite = styled.div`
  position: relative;
  top: -14%;
  left: 0%;
  width: auto;
  height: auto;
  background-color: #000;
  color: white;
  border: none;
  padding: 22px;
  border-radius: 8px;
  margin-top: 30%;
  border-radius: 8px;
  text-align: center;
  display: column;
  grid-template-columns: repeat(auto-fill, minmax(310px, 2fr));
  grid-gap: 12px;

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
  margin-left: 150%;
  display: flex;
  flex-direction: column;
  align-items: center;
`;

export const CenteredContainer = styled.div`
  display: flex;
  align-items: center;
  justify-content: center;
  position: absolute;
  top: 3%;
  left: 50%;
  transform: translate(-50%, -50%);
  width: auto;
  height: auto;
  background-color: #000;
  color: white;
  justify-content: center, center;
  border: none;
  padding: 22px;
  gap: 12px;
  border-radius: 8px;
  text-align: center;

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

export const MidContainer = styled.div`
  display: flex;
  align-items: center;
  justify-content: center;
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  width: auto;
  height: auto;
  background-color: #000;
  color: white;
  flex-direction: column;
  justify-content: center;
  border: none;
  padding: 22px;
  gap: 12px;
  border-radius: 8px;
  text-align: center;
  animation: ${wave} 10s infinite linear;
  color: aliceblue;

  &::before {
    content: '';
    position: absolute;
    inset: 0;
    left: -2px;
    margin-top: -3px;
    width: 100.5%;
    height: 102%;
    border-radius: 10px;
    background: linear-gradient(45deg, #e81cff 29%, #40c9ff 100%);
    z-index: -10;
    pointer-events: none;
    transition: all 0.6s cubic-bezier(0.175, 0.885, 0.32, 1.275);
  }
`;

export const MidTextContainer = styled.div`
  display: flex;
  align-items: center;
  justify-content: center;
  position: relative;
  top: 45%;
  left: 155%;
  transform: translate(-50%, -50%);
  width: 230%;
  height: 75%;
  background-color: #000;
  color: white;
  flex-direction: column;
  justify-content: center;
  border: none;
  padding: 22px;
  gap: 12px;
  border-radius: 8px;
  text-align: center;
  animation: ${wave} 10s infinite linear;
  color: aliceblue;

  &::before {
    content: '';
    position: absolute;
    inset: 0;
    left: -2px;
    margin-top: -3px;
    width: 100.5%;
    height: 102%;
    border-radius: 10px;
    background: linear-gradient(45deg, #e81cff 29%, #40c9ff 100%);
    z-index: -10;
    pointer-events: none;
    transition: all 0.6s cubic-bezier(0.175, 0.885, 0.32, 1.275);
  }
`;
