import styled from "styled-components";

export const BlurredOverlayWrapper = styled.div`
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0, 0, 0, 0.5);
  backdrop-filter: blur(5px);
  z-index: 999;
`;

export const ButtonContainerWrapper = styled.div`
    display: center;
    justify-content: center;
    margin-top: 10%;
    margin-left:25%;
`;

export const ButtonToRightContainerWrapper = styled.div`
    display: center;
    justify-content: center;
    margin-top: -19%;
    margin-left: 105%;
`;

export const ButtonItemWrapper = styled.div`
  display: flex;
  align-items: center;
  justify-content: space-between;
  height: 100%;
  width: 100%;
  position: relative;
`;

export const ButtonContentsWrapper = styled.span`
  flex-grow: 1;
  font-family: "Roboto", arial, sans-serif;
  font-weight: 500;
  overflow: hidden;
  text-overflow: ellipsis;
  vertical-align: top;
`;

export const ButtonRowWrapper = styled.div`
  display: flex;
  flex-direction: row;
  z-index: 100;
  gap: 20px;
`;

export const ButtonVerContainerWrapper = styled.div`
    display: center;
    justify-content: center;
    margin-top: 10px;
    margin-left: 66%;
`;

export const ButtonVerColumnContainerWrapper = styled.div`
  margin-top: 150px;
  margin-left: 300px;
  margin-bottom: 10px;
  display: flex;
  gap: 20px;
  flex-direction: column;
  justify-content: space-between;
`;

export const ColumnTextWrapper = styled.div`
  display: flex;
  flex-direction: column;
  align-items: center;
`;

export const InputWrapper = styled.div`
  position: relative;
  margin-top: 21px;
  margin-left: 20px;
  margin-right: 21px;
  width: 215px;
  height: 47px;
  border-radius: 10px;
  background: linear-gradient(-45deg, #e81cff 0%, #40c9ff 100%);
  }
`;
