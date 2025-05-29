import React, { useContext } from "react";
import { LanguageContext } from "../contexts/LanguageContext";
import { Button, Tooltip } from "antd";
import { SunFilled , MoonOutlined } from "@ant-design/icons";

interface ToggleThemeButtonProps {
  darkTheme: boolean;
  toggleTheme: () => void;
}

const ToggleThemeButton: React.FC<ToggleThemeButtonProps> = ({
  darkTheme,
  toggleTheme,
}) => {
  const { locale } = useContext(LanguageContext);
  const isBG = locale.startsWith("bg");
  return (
    <div className="toggle-theme-btn">
      <Tooltip title={isBG ? "Смени темата" : "Switch Theme"}>
        <Button
          onClick={toggleTheme}
          style={{
            backgroundColor: "var(--navigation-button-color)",
            color: "#000000",
            border: "none",
            padding: "8px 16px",
            borderRadius: "4px",
            width: "50px",
          }}
        >
          {darkTheme ? <SunFilled /> : <MoonOutlined />}
        </Button>
      </Tooltip>
    </div>
  );
};

export default ToggleThemeButton;
