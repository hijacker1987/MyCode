import React, { useState, useRef, useEffect } from "react";

import { useUser } from "../../Services/UserContext";
import { chatBot } from "../../Services/Backend.Endpoints";
import { postApi } from "../../Services/Api";

import { ChuckBotInput, MessageColorPicker } from "../../Components/Styles/InputOutput.styled";
import { CBToggleButton, ChuckBotButton, } from "../../Components/Styles/Buttons/InternalButtons.styled";
import { CBInputContainer, CBOutputContainer } from "../../Components/Styles/Containers/Containers.styled";
import { ChuckBotClosed, ChuckBotContainer } from "../../Components/Styles/Containers/ComplexContainers.styled";

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

    const Form = botOpen ? ChuckBotContainer : ChuckBotClosed;

    return (
        <>
            <Form>
                <CBOutputContainer ref={outputRef}>
                    {messages.map((message, index) => ( 
                        <MessageColorPicker key={index} own={message.mine}>
                            {message.text}
                        </MessageColorPicker>
                    ))}
                    {isTyping && (
                        <div style={{ textAlign: "right", color: "rebeccapurple" }}>
                            Chuck is typing{typingAnimation}
                        </div>
                    )}
                </CBOutputContainer>
                <CBInputContainer>
                    <ChuckBotInput
                        className="chuckText"
                        type="text"
                        value={inputText}
                        onChange={(e) => setInputText(e.target.value)}
                    />
                    <ChuckBotButton onClick={handleMessageSend}>Send</ChuckBotButton>
                </CBInputContainer>
            </Form>
            <CBToggleButton className="toogle" onClick={toggleBot}>Chat Norris</CBToggleButton>
        </>
    );
};

export default ChuckBot;
