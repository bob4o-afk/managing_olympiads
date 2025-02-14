import React, { useEffect } from "react";
import "./ui/OlympiadsAnimation.css";

const OlympiadsAnimation = () => {
    useEffect(() => {
        setTimeout(() => {
            const wordElement = document.querySelector(".word");
            if (wordElement) {
                wordElement.classList.add("animate-word");
            }
        }, 1000);
    }, []);

    return (
        <div className="container-for-animation">
            <div className="text-for-animation">
                <span className="letter-o animate-o"></span>
                <span className="word">lympiads</span>
            </div>
            <div className="rings-container">
                <div className="ring"></div>
                <div className="ring"></div>
                <div className="ring"></div>
                <div className="ring"></div>
            </div>
        </div>
    );
};

export default OlympiadsAnimation;
