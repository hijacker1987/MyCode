import React, { useState, useRef, useEffect } from "react";

import { useUser } from "../../Services/UserContext";
import { chatBot } from "../../Services/Backend.Endpoints";
import { postApi } from "../../Services/Api";

import "../../Components/Styles/CustomCss/ChuckBot.css";
import "../../Components/Styles/CustomCss/CBButton.css";

const ChuckBot = () => {
    const outputRef = useRef(null);
    const { userData } = useUser();
    const { username } = userData;
    const [displayName, setDisplayName] = useState("Guest");
    const [messages, setMessages] = useState([]);
    const [inputText, setInputText] = useState("");
    const [typingAnimation, setTypingAnimation] = useState(".");
    const [isTyping, setIsTyping] = useState(false);
    const [botOpen, setBotOpen] = useState(false);

    const botName = "Chat Norris";

    useEffect(() => {
        if (username !== "" && username != undefined && username != null) {
            setDisplayName(username);
        } else {
            setDisplayName("Guest");
            setMessages([]);
            setBotOpen(false);
        }
    }, [username]);

    useEffect(() => {
        const typingInterval = setInterval(() => {
            setTypingAnimation(prevAnimation => {
                switch (prevAnimation) {
                    case ".":
                        return "..";
                    case "..":
                        return "...";
                    default:
                        return ".";
                }
            });
        }, 500);

        return () => clearInterval(typingInterval);
    }, []);

    const toggleBot = () => {
        setBotOpen(!botOpen);
    };

    const handleMessageSend = async () => {
        const newMessage = { text: inputText };
        setIsTyping(true);

        try {
            setMessages(prevMessages => [
                ...prevMessages,
                { text: `${displayName}: ${inputText}`, mine: true },
            ]);
            setInputText("");

            const response = await postApi(`${chatBot}`, newMessage);

            setTimeout(() => {
                setMessages(prevMessages => [
                    ...prevMessages,
                    { text: `${botName}: ${response}`, mine: false }
                ]);
                setIsTyping(false);
            }, 2500);
        } catch (error) {
            console.error("Error sending message:", error);
        }
    };

    useEffect(() => {
        if (outputRef.current) {
            outputRef.current.scrollTop = outputRef.current.scrollHeight;
        }
    }, [messages, isTyping]);

    return (
        <div>
            <div className={botOpen ? "ChuckBot" : "ChuckBot closed"}>
                <div className="output-container" ref={outputRef}>
                    {messages.map((message, index) => (
                        <div key={index} className={message.mine ? "user-message" : "bot-message"} style={{ textAlign: message.mine ? "right" : "left" }}>
                            {message.text}
                        </div>
                    ))}
                    {isTyping && (
                        <div style={{ textAlign: "left", color: "rebeccapurple" }}>
                            Chuck is typing{typingAnimation}
                        </div>
                    )}
                </div>
                <div className="input-container">
                    <input
                        className="chuckText"
                        type="text"
                        value={inputText}
                        onChange={(e) => setInputText(e.target.value)}
                    />
                    <button onClick={handleMessageSend}>Send</button>
                </div>
            </div>
            <button className="toogle" onClick={toggleBot}>Chat Norris</button>
        </div>
    );
};

export default ChuckBot;
