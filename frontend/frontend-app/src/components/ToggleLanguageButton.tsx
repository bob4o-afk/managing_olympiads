import React, { useContext } from "react";
import { Button, Tooltip } from "antd";
import { GlobalOutlined } from "@ant-design/icons";
import { LanguageContext } from "../contexts/LanguageContext";

const ToggleLanguageButton: React.FC = () => {
  const { locale, setLocale } = useContext(LanguageContext);
  const isBG = locale.startsWith("bg");

  const toggleLanguage = () => {
    setLocale(isBG ? "en" : "bg");
  };

  return (
    <div className="toggle-language-btn">
      <Tooltip title={isBG ? "Превключи на английски" : "Switch to Bulgarian"}>
        <Button
          icon={<GlobalOutlined />}
          onClick={toggleLanguage}
          style={{
            backgroundColor: "var(--navigation-button-color)",
            color: "#000000",
            border: "none",
            padding: "8px 16px",
            borderRadius: "4px",
            width: "50px",
          }}
        />
      </Tooltip>
    </div>
  );
};

export default ToggleLanguageButton;
