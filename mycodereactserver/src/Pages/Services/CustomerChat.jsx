import React, { useState, useRef, useEffect } from "react";
import * as signalR from "@microsoft/signalr";

import { useUser } from "../../Services/UserContext";
import { backendUrl } from "../../Services/Config";

import "../../Components/Styles/CustomCss/CustomerChat.css";
import "../../Components/Styles/CustomCss/CCButton.css";

const CustomerChat = () => {
    const [connection, setConnection] = useState(null);
    const [messages, setMessages] = useState([]);
    const [inputMessage, setInputMessage] = useState("");
    const [showChat, setShowChat] = useState(false);
    const { userData } = useUser();
    const { username } = userData;
    const [displayName, setDisplayName] = useState("Guest");

    useEffect(() => {
        if (username !== "" && username != undefined && username != null) {
            setDisplayName(username);
        } else {
            setDisplayName("Guest");
        }
    }, [username]);

    const sendMessage = () => {
        if (inputMessage.trim() !== "") {
            connection.invoke("SendMessage", displayName, inputMessage)
                .catch((error) => console.error("Failed to send message: ", error));
            setInputMessage("");
        }
    };

    useEffect(() => {
        const newConnection = new signalR.HubConnectionBuilder()
            .withUrl(`${backendUrl}message`)
            .configureLogging(signalR.LogLevel.Information)
            .build();

        setConnection(newConnection);

        newConnection.on("ReceiveMessage", (user, message) => {
            setMessages(prevMessages => [...prevMessages, { user, message }]);
        });

        newConnection.onclose(() => {
            console.log("Connection closed, trying to reconnect...");
            setTimeout(startConnection, 5000);
        });

        const startConnection = async () => {
            try {
                await newConnection.start();
                console.log("Connection re-established");
            } catch (error) {
                console.error("Connection failed: ", error);
            }
        };

        startConnection();

        return () => {
            newConnection.stop();
        };
    }, []);

    useEffect(() => {
        if (connection) {
            connection.start()
                .then(() => console.log("Connection established"))
                .catch((error) => console.error("Connection failed: ", error));
        }
    }, [connection]);



    useEffect(() => {
        if (connection) {
            connection.start()
                .then(() => console.log("Connection established"))
                .catch((error) => console.error("Connection failed: ", error));
        }
    }, [connection]);

    const toggleChat = () => {
        setShowChat(prevState => !prevState);
    };

    return (
        <div>
            <button className="toogle-chat-button" onClick={toggleChat}>
                {showChat ? "Hide Chat" : "Show Chat"}
            </button>
            {showChat && (
                <div className="customer-chat-container">
                    <ul>
                        {messages.map((msg, index) => (
                            <li key={index}>
                                <strong>{msg.user}: </strong>
                                {msg.message}
                            </li>
                        ))}
                    </ul>
                    <div className="input-container">
                        <input
                            className="customer-chat-input"
                            type="text"
                            value={inputMessage}
                            onChange={(e) => setInputMessage(e.target.value)}
                        />
                    </div>
                    <button onClick={sendMessage}>Send</button>
                </div>
            )}
        </div>
    );
};

export default CustomerChat;
