import styled from "styled-components";
import Modal from "react-bootstrap/Modal";

export const ModalContainer = styled.div`
  position: fixed;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
`;

export const PreviewModalContainer = styled.div`
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
`;

export const StyledModalContainer = styled.div`
  display: flex;
  flex-direction: column;
  border-radius: 8px;
  box-shadow: 0 0 10px rgba(0, 0, 0, 0.2);
  padding: 20px;
`;

export const ModalTitle = styled(Modal.Title)`
  text-align: center;
  margin-bottom: 10px;
`;

export const ModalBody = styled(Modal.Body)`
  text-align: center;
`;

export const ChatModalBody = styled(Modal.Body)`
  text-align: center;
  max-height: 400px;
  min-width: 250px;
  overflow-y: auto;
`;

export const ModalFooter = styled(Modal.Footer)`
  display: flex;
  flex-direction: column;
  align-items: center;
`;

