import React, { useState, useRef, useEffect } from "react";
import * as signalR from "@microsoft/signalr";
import Modal from "react-bootstrap/Modal";

import { useUser } from "../../Services/UserContext";
import { getApi, putStatApi } from "../../Services/Api";
import { backendUrl } from "../../Services/Config";
import { getActive, getAnyArc, getOwn, getRoom, getActRoom, dropBack } from "../../Services/Backend.endpoints";
import { formatElapsedTime, formattedTime } from "../../Services/ElapsedTime";
import { Notify } from "./../../Pages/Services/Index";

import { BlurredOverlay, ModalContainer, StyledModal } from "../../Components/Styles/Background.styled";
import { MidContainer, TextContainer } from "../../Components/Styles/TextContainer.styled";
import { StyledTr, StyledTd, RowSpacer } from "../../Components/Styles/TableRow.styled";
import { ButtonContainer } from "../../Components/Styles/ButtonContainer.styled";
import { ButtonRowContainer } from "../../Components/Styles/ButtonRow.styled";

import "../../Components/Styles/CustomCss/CustomerChat.css";

const CustomerChat = () => {
    const chatContainerRef = useRef(null);
    const { userData } = useUser();
    const { role, userid } = userData;
    const [connection, setConnection] = useState(null);
    const [connectedUsers, setConnectedUsers] = useState([]);
    const [requests, setRequests] = useState([]);
    const [room, setRoom] = useState();
    const [userAddedToGroup, setUserAddedToGroup] = useState(false);
    const [messages, setMessages] = useState([]);
    const [inputMessage, setInputMessage] = useState("");
    const [selectedUserMessages, setSelectedUserMessages] = useState([]);
    const [showMessagesModal, setShowMessagesModal] = useState(false);
    const [showChat, setShowChat] = useState(false);
    const [inputDisabled, setInputDisabled] = useState(false);
    const [sendButtonDisabled, setSendButtonDisabled] = useState(false);
    const [getButtonHidden, setGetButtonHidden] = useState(false);
    const [isSyncRoomsOpen, setIsSyncRoomsOpen] = useState(false);

    const toggleSyncRooms = () => {
        setIsSyncRoomsOpen(prevState => !prevState);
    };

    const sendMessage = async () => {
        if (inputMessage.trim() !== "") {
            if (!connection || connection.state !== signalR.HubConnectionState.Connected || connection === null) {
                try {
                    const newerConnection = await startConnection();
                    setConnection(newerConnection);
                } catch (error) {
                    console.error("Failed to reconnect: ", error);
                    return;
                }
            }

            connection.invoke("SendMessage", { RoomId: room, UserId: userid, Message: inputMessage })
                .catch((error) => console.error("Failed to send message: ", error));
            setInputMessage("");
        }
    };

    const getGroups = async () => {
        try {
            const data = await getApi(getRoom);
            setRequests(data);
        } catch (error) {
            console.error("Failed to get groups: ", error);
        }
    };

    const getActiveGroups = async () => {
        try {
            const data = await getApi(getActRoom);
            setRequests(data);
        } catch (error) {
            console.error("Failed to get groups: ", error);
        }
    };

    const handleObserveRoom = async (id) => {
        try {
            setRoom(id);
            const data = await getApi(`${getActive}${id}`);

            if (selectedUserMessages.length != 0) {
                setSelectedUserMessages([]);
            }
            data.forEach(item => {
                setSelectedUserMessages(prevMessages => [...prevMessages, { user: item.whom, message: item.text, time: item.when }]);
            });
            setShowMessagesModal(true);
        } catch (error) {
            console.error("Failed to get groups: ", error);
        }
    };

    const handleDropBackRoom = async (id) => {
        try {
            const data = await putStatApi(`${dropBack}${id}`);
            if (data === 200) {
                Notify("Success", "New support member can choose it now!")
            } else {
                Notify("Error", "Something went wrong!")
            }
            getGroups();
            setShowMessagesModal(false);
        } catch (error) {
            console.error("Failed to get groups: ", error);
        }
    }

    const getOldMessages = async () => {
        try {
            const data = await getApi(getOwn);
            if (messages.length != 0) {
                setMessages([]);
            }
            data.forEach(item => {
                setMessages(prevMessages => [...prevMessages, { user: item.whom, message: item.text, time: item.when }]);
            });
        } catch (error) {
            console.error("Failed to get groups: ", error);
        }
    };

    const addToGroup = async (setRoomId) => {
        setRoom(setRoomId);
        await connection.invoke("JoinRoom", { user: userid, chatroom: setRoomId })
            .catch((error) => console.error("Failed to add user to group: ", error));
        Notify("Success", "Successfully joined!");
        setUserAddedToGroup(true);
        setConnection(connection);
    };

    const removeGroup = async (setRoomId) => {
        setRoom(setRoomId);
        const response = await connection.invoke("LeaveRoom", { user: userid, chatroom: setRoomId })
            .catch((error) => console.error("Failed to remove user from group: ", error));

        if (response.statuscode === 200) {
            setUserAddedToGroup(false);
            setRoom(null);
            setConnection(null);
            setConnectedUsers([]);
            Notify("Success", "Successfully archived!");
        }
        if (response.statuscode === 409) {
            console.log("Failed to remove and archive properly.")
            Notify("Error", "Something went wrong!");
        }
    }

    const startConnection = async () => {
        try {
            const newConnection = new signalR.HubConnectionBuilder()
                .withUrl(`${backendUrl}message`)
                .configureLogging(signalR.LogLevel.Information)
                .build();

            newConnection.on("ReceiveMessage", (user, message, time) => {
                setMessages(prevMessages => [...prevMessages, { user, message, time }]);

                if (user === "Server feedback") {
                    setInputMessage("Input disabled!");
                    setInputDisabled(true);
                    setSendButtonDisabled(true);

                    setTimeout(() => {
                        setInputDisabled(false);
                        setSendButtonDisabled(false);
                        setShowChat(false);
                        setMessages([]);
                        setInputMessage("");
                    }, 10000);

                    connection.stop();
                }
            });

            newConnection.on("FailedToJoinRoom", (errorMessage) => {
                setMessages(prevMessages => [...prevMessages, { user, message, time }]);
                console.log(errorMessage);
            });

            newConnection.onclose("UserDisconnected", (user, message, time) => {
                setMessages(prevMessages => [...prevMessages, { user, message, time }]);
            });

            newConnection.onclose(() => {
                console.log("Connection closed, trying to reconnect...");
                Notify("Error", "Chat connection lost, trying to reconnect!");
                setTimeout(startConnection, 5000);
            });

            await newConnection.start();

            return newConnection;
        } catch (error) {
            console.error("Connection failed: ", error);
            return null;
        }
    };

    useEffect(() => {
        const connect = async () => {
            const connection = await startConnection();
            setConnection(connection);
            Notify("Success", "Connection to support is live, but try Chat Norris first!!!");

            try {
                const response = await getApi(getAnyArc);
                
                if (response.status === 404) {
                    setGetButtonHidden(true);
                }
            } catch (error) {
                console.error("Failed to get data: ", error);
            }
        };

        connect();
    }, []);

    useEffect(() => {
        if (connection != null) {
            connection.on("ConnectedUser", (users) => {
                setConnectedUsers(users);
            });
            console.log(connectedUsers);
        }
    }, [connection]);

    const toggleChat = () => {
        if (role === "User" && !userAddedToGroup) {
            addToGroup(userid.toLowerCase());
        }

        setShowChat(prevState => !prevState);
    };

    const handleJoinRoom = (roomId) => {
        if (!userAddedToGroup) {
            addToGroup(roomId.toLowerCase());
            setUserAddedToGroup(true);
        }
    };

    const handleLeaveRoom = async (roomId) => {
        if (userAddedToGroup) {
            await removeGroup(roomId.toLowerCase());

            setRequests(prevRequests => prevRequests.filter(room => room !== roomId));
        }
    };

    useEffect(() => {
        if (chatContainerRef.current) {
            chatContainerRef.current.scrollTop = chatContainerRef.current.scrollHeight;
        }
    }, [messages]);

    return (
        <div>
            <div>
                {role === "Admin" && (
                    <MidContainer className={isSyncRoomsOpen ? "sync-rooms-open" : "sync-rooms-closed"} style={{ marginTop: "-10%", marginLeft: "-35.2%", width: "500px" }}>
                        <button onClick={getActiveGroups}>Sync Active Rooms</button>
                        <ul>
                            {requests.map((room, index) => (
                                <li key={index}>
                                    <button onClick={() => handleObserveRoom(room)}>Observe Room {room}</button>
                                </li>
                            ))}
                        </ul>
                        <button onClick={toggleSyncRooms}>Minimize</button>
                    </MidContainer>
                )}
                <div>
                    {role === "Support" && (
                        <MidContainer className={isSyncRoomsOpen ? "sync-rooms-open" : "sync-rooms-closed"} style={{ marginTop: "-10%", marginLeft: "-35.2%", width: "500px" }}>
                            <button onClick={getGroups}>Sync Active Rooms</button>
                            <ul>
                                {requests.map((room, index) => (
                                    <li key={index}>
                                        <button onClick={() => handleJoinRoom(room)}>Join Room {room}</button>
                                        <button onClick={() => handleLeaveRoom(room)}>Close Room</button>
                                    </li>
                                ))}
                            </ul>
                            <button onClick={toggleChat}>
                                {showChat ? "Hide Chat" : "Show Chat Window"}
                            </button>
                            <button onClick={toggleSyncRooms}>Minimize</button>
                        </MidContainer>
                    )}
                </div>
                <div>
                    {showChat && (
                        <div ref={chatContainerRef} className="customer-chat-container">
                            <ul>
                                {messages.map((msg, index) => (
                                    <li key={index}>
                                        <h4>{msg.user}: </h4>
                                        <strong>{msg.message} - </strong>
                                        {formatElapsedTime(msg.time)}
                                    </li>
                                ))}
                                <div>
                                    <input
                                        className="customer-chat-input"
                                        type="text"
                                        value={inputMessage}
                                        onChange={(e) => setInputMessage(e.target.value)}
                                        disabled={inputDisabled}
                                    />
                                <button onClick={sendMessage} disabled={sendButtonDisabled}>Send</button>
                                </div>
                            </ul>
                        </div>
                    )}
                    {role === "User" && (
                        <>
                            <ButtonContainer type="button" style={{ marginLeft: "100%" }} onClick={toggleChat}>
                                {showChat ? "Hide Chat" : "Cus. Service Chat"}
                            </ButtonContainer>
                            {!getButtonHidden && (
                                <ButtonContainer type="button" style={{ marginTop: "480%", marginLeft: "100%" }} onClick={getOldMessages}>
                                    Show Closed Chat
                                </ButtonContainer>
                            )}
                        </>
                    )}
                </div>
            </div>
            <div>
                {showMessagesModal && (
                    <BlurredOverlay>
                        <ModalContainer>
                            <StyledModal size="lg">
                                <TextContainer>
                                    <Modal.Header closeButton>
                                        <Modal.Title>Currently stored Chat</Modal.Title>
                                    </Modal.Header>
                                    <Modal.Body style={{ maxHeight: "400px", overflowY: "auto" }}>
                                        {selectedUserMessages
                                            .map((message, index) => (
                                                <React.Fragment key={index}>
                                                    <StyledTr className={index % 2 === 1 ? "even-row" : "odd-row"}>
                                                        <StyledTd>{index + 1}</StyledTd>
                                                        <StyledTd>{message.user} - {message.message} at {formattedTime(message.time)}</StyledTd>
                                                    </StyledTr>
                                                    <RowSpacer />
                                                </React.Fragment>))}
                                    </Modal.Body>
                                </TextContainer>
                                <Modal.Footer>
                                    <ButtonRowContainer>
                                        <ButtonContainer onClick={() => setShowMessagesModal(false)}>
                                            Back
                                        </ButtonContainer>
                                        <ButtonContainer onClick={() => handleDropBackRoom(room)}>
                                            Drop back to support
                                        </ButtonContainer>
                                    </ButtonRowContainer>
                                </Modal.Footer>
                            </StyledModal>
                        </ModalContainer>
                    </BlurredOverlay>
                )}
            </div>
        </div>
    );
};

export default CustomerChat;
