import styled from "styled-components";
import GoogleSvg from "../../../assets/web_dark_sq_na.svg?react";

export const GoogleButton = styled.button`
  -moz-user-select: none;
  -webkit-user-select: none;
  -ms-user-select: none;
  -webkit-appearance: none;
  background-color: #131314;
  border: 1px solid #747775;
  border-radius: 4px;
  box-sizing: border-box;
  color: #e3e3e3;
  cursor: pointer;
  font-family: "Roboto", arial, sans-serif;
  font-size: 14px;
  height: 40px;
  letter-spacing: 0.25px;
  outline: none;
  overflow: hidden;
  padding: 0 12px;
  position: relative;
  text-align: center;
  transition: background-color .218s, border-color .218s, box-shadow .218s;
  vertical-align: middle;
  white-space: nowrap;
  width: 120px;
  min-width: min-content;
  border-color: #8e918f;
  margin-top: -10%;
  margin-left: 8%;

  &:hover {
    background-color: darkgray;
    color: black;
  }

  &:disabled {
    cursor: default;
    background-color: #13131461;
    border-color: #8e918f1f;

    .gsi-material-button-state {
      background-color: #e3e3e31f;
    }

    .gsi-material-button-contents,
    .gsi-material-button-icon {
      opacity: 38%;
    }
  }

  &:not(:disabled):active .gsi-material-button-state,
  &:not(:disabled):focus .gsi-material-button-state {
    background-color: white;
    opacity: 12%;
  }

  &:not(:disabled):hover {
    box-shadow: 0 1px 2px 0 rgba(60, 64, 67, .30), 0 1px 3px 1px rgba(60, 64, 67, .15);
    
    .gsi-material-button-state {
      background-color: white;
      opacity: 8%;
    }
  }
`;

export const FacebookButton = styled.button`
  background-color: #4C69BA;
  background-image: linear-gradient(#4C69BA, #3B55A0);
  font-family: "Helvetica neue", Helvetica Neue, Helvetica, Arial, sans-serif;
  text-shadow: 0 -1px 0 #354C8C;
  cursor: pointer;
  color: aliceblue;
  width: 120px;
  height: 40px;
  margin-left: 8%;

  &:hover,
  &:focus {
    background-color: #5B7BD5;
    background-image: linear-gradient(#5B7BD5, #4864B1);
  }
`;

export const FacebookIcon = styled.span`
  background: url("https://s3-us-west-2.amazonaws.com/s.cdpn.io/14082/icon_facebook.png") 0px 5px no-repeat;
  height: 20px;
  margin-right: -38px;
  margin-left: 20px;
  min-width: 20px;
  width: 20px;
  padding: 9px;
`;

export const GitHubButton = styled.button`
  width: 120px;
  height: 40px;
  margin-left: 8%;
  cursor: pointer;
  display: flex;
  justify-content: center;
  align-items: center;
  border: none;
  border-radius: 4px;
  transition: background-color .3s;

  &:hover {
    background-color: black;
    color: white;
  }

  svg {
    margin-right: 8px;
  }
`;

export const GoogleIcon = styled(GoogleSvg)`
  height: 40px;
  margin-bottom: 2px;
  margin-right: -40px;
  margin-left: -6px;
  min-width: 20px;
  width: 40px;
  padding: 9px;
  clip-path: inset(20px);
`;

export const GitHubIcon = styled.svg`
  margin-left: 4px;
  width: 20px;
  height: 20px;
  fill: currentColor;
`;