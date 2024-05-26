import styled from "styled-components";

export const ChuckBotInput = styled.input`
  background: lightgrey;
  height: 30px;
  width: calc(70% - 10px);
  font-family: Vancouver, sans-serif;
  font-size: 17px;
  font-weight: bold;
`;

export const CustomerChatTextInput = styled.input.attrs({ type: 'text' })`
  background: lightgrey;
  height: 30px;
  width: 200px;
  font-family: Vancouver, sans-serif;
  font-size: 17px;
  font-weight: bold;
`;

export const MessagesList = styled.ul`
  flex-grow: 1;
  overflow-y: auto;
  padding: 0;
  margin: 0;
  list-style-type: none;
`;

export const MessageColorPicker = styled.li`
    color: ${({ own }) => (own ? "darkgreen" : "limegreen")};
    textAlign: ${({ own }) => (own ? "right" : "left")};
`;

export const MessageText = styled.strong`
    color: white;
`;

export const Heading2 = styled.h3`
  color: #ffffff;
  margin-left: 20px;
  font-size: 20px;
`;

export const Heading3 = styled.h3`
  color: #ffffff;
  margin-left: 20px;
  font-size: 18px;
`;

export const RelHeading3 = styled.h3`
  color: #ffffff;
  margin-top: 75px;
  margin-left: 800px;
  font-size: 18px;
`;

export const VerHeading3 = styled.h3`
  margin-top: -30px;
  color: #ffffff;
  margin-left: 20px;
  font-size: 18px;
`;

export const Heading4 = styled.h3`
  color: #ffffff;
  margin-left: 20px;
  font-size: 16px;
`;

export const Image = styled.img`
    display: block;
    margin: 0 auto;
    border: 5px solid white;
`;
