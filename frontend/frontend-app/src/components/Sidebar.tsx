import React, { useState, useContext } from "react";
import { Menu } from "antd";
import {
  HomeOutlined,
  FormOutlined,
  InfoCircleOutlined,
  SettingOutlined,
  UserOutlined,
  BarChartOutlined,
  SnippetsOutlined,
  DatabaseOutlined,
  UnorderedListOutlined,
} from "@ant-design/icons";

import "./ui/Sidebar.css";
import { LanguageContext } from "../contexts/LanguageContext";

interface SidebarProps {
  darkTheme: boolean;
  onSelect: (key: string) => void;
  isMobile?: boolean;
  closeDrawer?: () => void;
}

const Sidebar: React.FC<SidebarProps> = ({
  darkTheme,
  onSelect,
  isMobile,
  closeDrawer,
}) => {
  const [openKeys, setOpenKeys] = useState<string[]>([]);
  const { locale, } = useContext(LanguageContext);
  const isBG = locale.startsWith("bg");

  const handleOpenChange = (keys: string[]) => {
    setOpenKeys(keys.length ? [keys[keys.length - 1]] : []);
  };

  const items = [
    {
      key: "home",
      icon: <HomeOutlined />,
      label: isBG ? "Начало" : "Home",
    },
    {
      key: "olympiads",
      icon: <SnippetsOutlined />,
      label: isBG ? "Олимпиади" : "Olympiads",
      children: [
        {
          key: "all-olympiads",
          icon: <BarChartOutlined />,
          label: isBG ? "Всички олимпиади" : "All Olympiads",
        },
        {
          key: "enrollment",
          icon: <FormOutlined />,
          label: isBG ? "Записване" : "Enrollment",
        },
        {
          key: "enrollments",
          icon: <UnorderedListOutlined />,
          label: isBG ? "Всички записвания" : "Enrollments",
        },
        {
          key: "documents",
          icon: <DatabaseOutlined />,
          label: isBG ? "Документи" : "Documents",
        },
      ],
    },
    {
      key: "for-me",
      icon: <InfoCircleOutlined />,
      label: isBG ? "За създателя" : "For the creator",
    },
    {
      key: "settings",
      icon: <SettingOutlined />,
      label: isBG ? "Настройки" : "Settings",
    },
    {
      key: "account",
      icon: <UserOutlined />,
      label: isBG ? "Акаунт" : "Account",
      children: [
        {
          key: "my-profile",
          icon: <UserOutlined />,
          label: isBG ? "Моят профил" : "My Profile",
        },
      ],
    },
  ];

  return (
    <Menu
      className="sidebar-menu"
      theme={darkTheme ? "dark" : "light"}
      mode="inline"
      items={items}
      openKeys={openKeys}
      onOpenChange={handleOpenChange}
      onClick={({ key }) => {
        onSelect(key);
        if (isMobile && closeDrawer) closeDrawer();
      }}
    />
  );
};

export default Sidebar;
