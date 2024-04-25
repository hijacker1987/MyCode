import React, { useState, useRef, useEffect } from "react";

import "../../Components/Styles/CustomCss/ChuckBot.css";
import "../../Components/Styles/CustomCss/CBButton.css";

const ChuckBot = () => {
    const outputRef = useRef(null);
    const [messages, setMessages] = useState([]);
    const [inputText, setInputText] = useState("");
    const [botOpen, setBotOpen] = useState(false);

    const botName = "Chat Norris";
    const userName = "Quest";

    const toggleBot = () => {
        setBotOpen(!botOpen);
    };

    const handleMessageSend = async () => {
        const newMessage = { text: inputText };
        try {
            const response = await fetch(`https://localhost:7001/cncbot/messages`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(newMessage)
            });

            const responseData = await response.json();

            setMessages(prevMessages => [
                ...prevMessages,
                { text: `${userName}: ${inputText}`, mine: true },
                { text: `${botName}: ${responseData}`, mine: false }
            ]);
            setInputText("");
        } catch (error) {
            console.error("Error sending message:", error);
        }
    };

    useEffect(() => {
        if (outputRef.current) {
            outputRef.current.scrollTop = outputRef.current.scrollHeight;
        }
    }, [messages]);

    return (
        <div>
            <div className={botOpen ? "ChuckBot" : "ChuckBot closed"}>
                <div className="output-container" ref={outputRef}>
                    {messages.map((message, index) => (
                        <div key={index} style={{ textAlign: message.mine ? "right" : "left" }}>
                            {message.text}
                        </div>
                    ))}
                </div>
                <div className="input-container">
                    <input
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
