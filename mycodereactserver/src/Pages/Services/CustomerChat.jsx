import React, { useState, useRef, useEffect } from "react";
import * as signalR from "@microsoft/signalr";

import { useUser } from "../../Services/UserContext";
import { getApi } from "../../Services/Api";
import { backendUrl } from "../../Services/Config";
import { getRoom, getOwn } from "../../Services/Backend.endpoints";
import { formatElapsedTime } from "../../Services/ElapsedTime";

import { ButtonContainer } from "../../Components/Styles/ButtonContainer.styled";
import { MidContainer } from "../../Components/Styles/TextContainer.styled";
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
    const [showChat, setShowChat] = useState(false);
    const [inputDisabled, setInputDisabled] = useState(false);
    const [sendButtonDisabled, setSendButtonDisabled] = useState(false);
    const [getButtonHidden, setGetButtonHidden] = useState(false);

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

    const getOldMessages = async () => {
        try {
            const data = await getApi(getOwn);
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
        }
        if (response.statuscode === 409) {
            console.log("Failed to remove and archive properly.")
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

            const response = await getApi(getOwn);

            if (response.status === 404) {
                setGetButtonHidden(true);
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
                {role === "Support" && (
                    <MidContainer style={{ marginTop: "-10%", marginLeft: "-35.2%", width: "500px" }}>
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
                    </MidContainer>
                )}
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
        </div>
    );
};

export default CustomerChat;
