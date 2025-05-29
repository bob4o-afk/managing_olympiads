import { useEffect } from "react";
import { Button } from "antd";
import "./ui/OlympiadsAnimation.css";

const OlympiadsAnimation = ({ onSkip }: { onSkip: () => void }) => {
  useEffect(() => {
    const timeout = setTimeout(() => {
      const wordElement = document.querySelector(".word");
      if (wordElement) {
        wordElement.classList.add("animate-word");
      }
    }, 1000);

    return () => clearTimeout(timeout);
  }, []);

  return (
    <div>
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
      <div className="button-wrapper">
        <Button className="button" size="large" onClick={onSkip}>
          Skip Animation
        </Button>
      </div>
    </div>
  );
};

export default OlympiadsAnimation;
