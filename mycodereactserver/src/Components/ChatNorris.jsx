import React, { useState, useEffect } from "react";
import ReactWebChat, { createDirectLine } from "botframework-webchat";

const ChatNorris = () => {
    const [directLine, setDirectLine] = useState(null);
    const [userMessage, setUserMessage] = useState("");

    useEffect(() => {
        const fetchData = async () => {
            try {
                const dl = createDirectLine({ token: 'try' });
                setDirectLine(dl);
            } catch (error) {
                console.error("Error initializing Direct Line:", error);
            }
        };

        fetchData();
    }, []);

    const sendMessage = async () => {
        try {
            directLine && directLine.postActivity({
                from: { id: 'user', name: 'User' },
                type: 'message',
                text: userMessage
            });
            setUserMessage("");
        } catch (error) {
            console.error("Error sending message:", error);
        }
    };

    return (
        <>
            <div className="cn-chatbot">
                {directLine ? <ReactWebChat directLine={directLine} /> : <p>Waiting for Chat Norris...</p>}
            </div>
            <input
                type="text"
                value={userMessage}
                onChange={(e) => setUserMessage(e.target.value)}
            />
            <button onClick={sendMessage}>Send Message</button>
        </>
    );
};

export default ChatNorris;
