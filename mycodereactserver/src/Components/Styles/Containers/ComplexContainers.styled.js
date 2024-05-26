import styled, { keyframes } from "styled-components";

const wave = keyframes`
  0% { background-position: 0 0; }
  50% { background-position: 100% 0; }
  100% { background-position: 0 0; }
`;

const baseStyles = `
  background-color: #000;
  color: lightgreen;
  font-family: Script, sans-serif;
  font-size: 15px;
  font-weight: bold;
`;

const gradientStyles = `
  background: linear-gradient(-45deg, #e81cff 0%, #40c9ff 100%);
`;

const BaseContainer = styled.div`
  ${baseStyles}
  display: flex;
  align-items: center;
  justify-content: center;
  border: none;
  border-radius: 8px;
  padding: 22px;
  gap: 12px;
  text-align: center;
  animation: ${wave} 10s infinite linear;
  position: relative;

  &::before {
    content: "";
    position: absolute;
    inset: 0;
    margin-top: -2px;
    margin-right: -2px;
    border-radius: 10px;
    background: ${gradientStyles};
    z-index: -10;
    pointer-events: none;
    transition: all 0.6s cubic-bezier(0.175, 0.885, 0.32, 1.275);
  }
`;

const ContainerWithGradient = styled(BaseContainer)`
  &::before {
    ${gradientStyles}
  }
`;

export const CodeTextContainer = styled.div`
  ${baseStyles}
  margin: 24px 16px;
  position: relative;
  width: 91vh;
  height: 43vh;

  &::before {
    content: '';
    position: absolute;
    inset: 0;
    left: -4px;
    margin-top: -5px;
    width: 92vh;
    height: 44vh;
    border-radius: 10px;
    background: linear-gradient(45deg, #e81cff 0%, #40c9ff 100%);
    z-index: -10;
    pointer-events: none;
    transition: all 0.6s cubic-bezier(0.175, 0.885, 0.32, 1.275);
  }
`;

export const ChuckBotContainer = styled.div`
  position: fixed;
  bottom: 55px;
  right: 20px;
  width: 300px;
  height: 400px;
  max-height: 400px;
  border: 1px solid #ccc;
  border-radius: 5px;
  overflow-y: auto;
  transition: all 0.3s ease;
  display: flex;
  flex-direction: column;
  padding: 10px;
  background: linear-gradient(45deg, rgba(40, 10, 50, 2.8) 5%, rgb(0, 47, 43) 75%);
  background-size: 400% 400%;
  animation: ${wave} 30s infinite linear;
`;

export const ChuckBotClosed = styled(ChuckBotContainer)`
  height: 0;
  width: 0;
  opacity: 0;
  overflow: hidden;
`;

export const CustomerChatContainer = styled.div`
  border: 1px solid #ccc;
  border-radius: 5px;
  overflow-y: auto;
  list-style-type: none;
  padding: 10px;
  transition: all 0.3s ease;
  display: flex;
  flex-direction: column;
  background: linear-gradient(-45deg, rgb(76, 255, 0) 10%, rgb(0, 47, 43) 70%);
  background-size: 400% 400%;
  animation: ${wave} 30s infinite linear;
  max-width: 450px;
  max-height: 550px;
  right: 15px;
  bottom: 102px;
  position: absolute;
  justify-content: flex-end;
  z-index: 2000;
`;

export const ChatContainer = styled(BaseContainer)`
  top: 20%;
  left: 15%;
  transform: translate(-50%, -50%);
  width: 400px;
  height: 300px;
  position: fixed;
  flex-direction: column;
  justify-content: center;
  color: aliceblue;
  text-align: center;

  &::before {
    background: linear-gradient(45deg, #e81cff 29%, #40c9ff 100%);
  }

  &.sync-rooms-open {
    transform: translateX(-500px);
  }

  &.sync-rooms-closed {
    transform: translateX(-300px);
  }
`;

export const EditorContainer = styled(BaseContainer)`
  display: flex;
  align-items: center;
  justify-content: center;
  position: absolute;
  top: 65%;
  left: 50%;
  transform: translate(-50%, -50%);
  flex-direction: column;

  &::before {
    background: linear-gradient(45deg, #e81cff 29%, #40c9ff 100%);
  }
`;

export const ErrorTextContainerRed = styled(BaseContainer)`
  color: red;
  top: 30%;
  margin-top: 10px;

  &::before {
    ${gradientStyles};
  }
`;

export const ErrorTextContainerWhite = styled(BaseContainer)`
  color: white;
  top: 30%;
  margin-top: 10px;

  &::before {
    ${gradientStyles};
  }
`;

export const MidContainer = styled(BaseContainer)`
  position: relative;
  margin-top: 100px;
  left: 50%;
  transform: translate(-50%, -50%);
  width: 400px;
  height: auto;
  color: aliceblue;

  &::before {
    content: "";
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

  &.sync-rooms-open {
    transform: translateX(-500px);
  }

  &.sync-rooms-closed {
    transform: translateX(-300px);
  }
`;

export const MidTextContainer = styled(BaseContainer)`
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  width: 550px;
  height: auto;
  color: aliceblue;
  display: flex;
  flex-direction: column;

  &::before {
    ${gradientStyles};
  }
`;

export const RightBarTextContainer = styled.div`
  margin: 23px 14px;
  width: 116px;
  height: 73px;
  background-color: #000;
  display: flex;
  align-items: center;
  justify-content: center;
  text-align: center;
  border: none;
  gap: 12px;
  border-radius: 8px;
  animation: ${wave} 10s infinite linear;
  color: lightgreen;
  font-family: Script, sans-serif;
  font-size: 15px;
  font-weight: bold;
  position: relative;

  &::before {
    content: '';
    position: absolute;
    inset: 0;
    left: -2px;
    margin-top: -3px;
    width: 120px;
    height: 78px;
    border-radius: 10px;
    background: linear-gradient(45deg, #e81cff 0%, #40c9ff 100%);
    z-index: -10;
    pointer-events: none;
    transition: all 0.6s cubic-bezier(0.175, 0.885, 0.32, 1.275);
  }
`;

export const TextContainerWhite = styled(BaseContainer)`
  color: white;
  width: 99%;
  padding: 20px;
  border-radius: 10px;
  display: flex;
  justify-content: center;
  align-items: flex-start;
  flex-direction: column;
  background-color: rgba(0, 0, 0, 0.5);
  text-align: left;
  height: auto;
  overflow-y: auto;
  overflow-x: hidden;
`;

export const CenteredContainer = styled(ContainerWithGradient)`
  top: 0.5%;
  left: 50%;
  max-width: 33.33%;
  transform: translateX(-50%);
  color: white;
`;

export const TextContainer = styled(ContainerWithGradient)`
  margin: 23px 14px;
  max-width: 215px;
  flex-direction: column;
`;

export const SmallTextContainer = styled(ContainerWithGradient)`
  margin-top: 22px;
  width: 200px;
  max-height: 40px;
  padding: 5px;
`;
