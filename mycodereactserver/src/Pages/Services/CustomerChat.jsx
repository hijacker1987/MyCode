import React, { useState, useRef, useEffect } from "react";
import * as signalR from "@microsoft/signalr";
import Modal from "react-bootstrap/Modal";

import { useUser } from "../../Services/UserContext";
import { getApi, putStatApi } from "../../Services/Api";
import { backendUrl } from "../../Services/Config";
import { getActive, getAnyArc, getOwn, getRoom, getActRoom, dropBack } from "../../Services/Backend.Endpoints";
import { formatElapsedTime, formattedTime } from "../../Services/ElapsedTime";
import { Notify } from "./../../Pages/Services/index";

import { BlurredOverlayWrapper } from "../../Components/Styles/Containers/Wrappers.styled";
import { StyledTr, StyledTd, RowSpacer } from "../../Components/Styles/CustomBoxes/Table.styled";
import { CustomerChatButton, StyledButton } from "../../Components/Styles/Buttons/InternalButtons.styled";
import { ChatModalBody, ModalContainer, StyledModalContainer } from "../../Components/Styles/CustomBoxes/Modal.styled";
import { CustomerChatTextInput, MessageText, MessageColorPicker, MessagesList } from "../../Components/Styles/InputOutput.styled";
import { ChatContainer, CustomerChatContainer, TextContainer } from "../../Components/Styles/Containers/ComplexContainers.styled";
import { CustomerChatInputContainer, RowButtonContainer, RowButtonWithTopMarginContainer } from "../../Components/Styles/Containers/Containers.styled";

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
            const loadedMessages = data.map(item => ({
                user: item.whom,
                message: item.text,
                time: item.when,
            }));
            setMessages(prevMessages => [...loadedMessages, ...prevMessages]);
        } catch (error) {
            console.error("Failed to get archived messages: ", error);
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

    const scrollToBottom = () => {
        if (chatContainerRef.current) {
            setTimeout(() => {
                chatContainerRef.current.scrollIntoView({ behavior: "smooth" });
            }, 100);
        }
    };

    useEffect(() => {
        scrollToBottom();
    }, [messages, showChat]);

    return (
        <>
            <>
                {role === "Admin" && (
                    <ChatContainer className={isSyncRoomsOpen ? "sync-rooms-open" : "sync-rooms-closed"}>
                        <button onClick={getActiveGroups}>Sync Active Rooms</button>
                        <ul>
                            {requests.map((room, index) => (
                                <li key={index}>
                                    <button onClick={() => handleObserveRoom(room)}>Observe Room {room}</button>
                                </li>
                            ))}
                        </ul>
                        <button onClick={toggleSyncRooms}>Minimize</button>
                    </ChatContainer>
                )}
                <>
                    {role === "Support" && (
                        <ChatContainer className={isSyncRoomsOpen ? "sync-rooms-open" : "sync-rooms-closed"}>
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
                        </ChatContainer>
                    )}
                </>
                <>
                    {showChat && (
                        <CustomerChatContainer>
                            <MessagesList>
                                {messages.map((msg, index) => (
                                    <MessageColorPicker key={index} own={msg.user === "Me"}>
                                        <h4>{msg.user}: </h4>
                                        <MessageText>{msg.message} - </MessageText>
                                        <MessageText>{formatElapsedTime(msg.time)}</MessageText>
                                    </MessageColorPicker>
                                ))}
                                <CustomerChatInputContainer>
                                    <CustomerChatTextInput
                                        ref={chatContainerRef}
                                        type="text"
                                        value={inputMessage}
                                        onChange={(e) => setInputMessage(e.target.value)}
                                        disabled={inputDisabled}
                                    />
                                    <CustomerChatButton onClick={sendMessage} disabled={sendButtonDisabled}>Send</CustomerChatButton>
                                </CustomerChatInputContainer>
                            </MessagesList>
                        </CustomerChatContainer>
                    )}
                    {role === "User" && (
                        <RowButtonContainer>
                            <StyledButton onClick={toggleChat}>
                                {showChat ? "Hide Chat" : "Cus. Service Chat"}
                            </StyledButton>
                            {!getButtonHidden && (
                                <StyledButton onClick={getOldMessages}>
                                    Load Archieved Chat
                                </StyledButton>
                            )}
                        </RowButtonContainer>
                    )}
                </>
            </>
            <>
                {showMessagesModal && (
                    <BlurredOverlayWrapper>
                        <ModalContainer>
                            <StyledModalContainer size="lg">
                                <TextContainer>
                                    <Modal.Header closeButton>
                                        <Modal.Title>Currently stored Chat</Modal.Title>
                                    </Modal.Header>
                                    <ChatModalBody>
                                        {selectedUserMessages
                                            .map((message, index) => (
                                                <React.Fragment key={index}>
                                                    <StyledTr className={index % 2 === 1 ? "even-row" : "odd-row"}>
                                                        <StyledTd>{index + 1}</StyledTd>
                                                        <StyledTd>{message.user} - {message.message} at {formattedTime(message.time)}</StyledTd>
                                                    </StyledTr>
                                                    <RowSpacer />
                                                </React.Fragment>))}
                                    </ChatModalBody>
                                </TextContainer>
                                <Modal.Footer>
                                    <RowButtonWithTopMarginContainer>
                                        <StyledButton onClick={() => setShowMessagesModal(false)}>
                                            Back
                                        </StyledButton>
                                        <StyledButton onClick={() => handleDropBackRoom(room)}>
                                            Drop back to support
                                        </StyledButton>
                                    </RowButtonWithTopMarginContainer>
                                </Modal.Footer>
                            </StyledModalContainer>
                        </ModalContainer>
                    </BlurredOverlayWrapper>
                )}
            </>
        </>
    );
};

export default CustomerChat;
