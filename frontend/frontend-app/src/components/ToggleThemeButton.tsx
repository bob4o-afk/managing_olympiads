import React from "react";
import { Button } from "antd";
import { HiOutlineSun, HiOutlineMoon } from "react-icons/hi";

interface ToggleThemeButtonProps {
  darkTheme: boolean;
  toggleTheme: () => void;
}

const ToggleThemeButton: React.FC<ToggleThemeButtonProps> = ({ darkTheme, toggleTheme }) => {
  return (
    <div className="toggle-theme-btn">
      <Button
        onClick={toggleTheme}
        style={{
          backgroundColor: 'var(--navigation-button-color)',
          color: '#000000',
          border: 'none',
          padding: '8px 16px',
          borderRadius: '4px',
        }}
      >
        {darkTheme ? <HiOutlineSun /> : <HiOutlineMoon />}
      </Button>
    </div>
  );
};

export default ToggleThemeButton;
