import React, { useState } from 'react';
import { Menu } from 'antd';
import {
  HomeOutlined, FormOutlined, InfoCircleOutlined,
  SettingOutlined, UserOutlined, BarChartOutlined,
  SnippetsOutlined, DatabaseOutlined, UnorderedListOutlined
} from '@ant-design/icons';

import './ui/Sidebar.css';


interface SidebarProps {
  darkTheme: boolean;
  onSelect: (key: string) => void;
  isMobile?: boolean;
  closeDrawer?: () => void;
}

const Sidebar: React.FC<SidebarProps> = ({ darkTheme, onSelect, isMobile, closeDrawer }) => {
  const [openKeys, setOpenKeys] = useState<string[]>([]);

  const handleOpenChange = (keys: string[]) => {
    setOpenKeys(keys.length ? [keys[keys.length - 1]] : []);
  };

  const items = [
    {
      key: 'home',
      icon: <HomeOutlined />,
      label: 'Home',
    },
    {
      key: 'olympiads',
      icon: <SnippetsOutlined />,
      label: 'Olympiads',
      children: [
        { key: 'all-olympiads', icon: <BarChartOutlined />, label: 'All olympiads' },
        { key: 'enrollment', icon: <FormOutlined />, label: 'Enrollment' },
        { key: 'enrollments', icon: <UnorderedListOutlined />, label: 'Enrollments' },
        { key: 'documents', icon: <DatabaseOutlined />, label: 'Documents' },
      ],
    },
    { key: 'for-me', icon: <InfoCircleOutlined />, label: 'For me' },
    { key: 'settings', icon: <SettingOutlined />, label: 'Settings' },
    {
      key: 'account',
      icon: <UserOutlined />,
      label: 'Account',
      children: [{ key: 'my-profile', icon: <UserOutlined />, label: 'My profile' }],
    },
  ];

  return (
    <Menu
      className="sidebar-menu"
      theme={darkTheme ? 'dark' : 'light'}
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
