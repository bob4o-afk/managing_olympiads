import React, { useState, useEffect } from 'react';
import { Button, Layout } from 'antd';
import { MenuUnfoldOutlined, MenuFoldOutlined } from '@ant-design/icons';
import { Route, Routes, useNavigate } from 'react-router-dom';
import './App.css';

import Sidebar from './components/Sidebar';
import ToggleThemeButton from './components/ToggleThemeButton';
import PDFViewer from './components/PDFViewer';
import Login from './components/Login';
import HomePage from './components/HomePage';
import PasswordReset from './components/PasswordReset';

const { Sider, Header, Content } = Layout;

function App() {
    const [darkTheme, setDarkTheme] = useState(true);
    const [collapsed, setCollapsed] = useState(true);
    const [selectedKey, setSelectedKey] = useState('home');
    const navigate = useNavigate();

    const toggleTheme = () => {
        setDarkTheme(!darkTheme);
    };

    // Apply the theme to the root element
    useEffect(() => {
        document.body.className = darkTheme ? 'dark-theme' : 'light-theme';
    }, [darkTheme]);

    const handleMenuSelect = (key: string) => {
        setSelectedKey(key);
        navigate(`/${key}`);
    };

    return (
        <Layout style={{ height: '100vh' }}>
            <Sider
                collapsed={collapsed}
                collapsible
                trigger={null}
                theme={darkTheme ? 'dark' : 'light'}
                className="sidebar"
                width={250}
                style={{
                    position: 'fixed',
                    height: '100vh',
                    left: 0,
                    top: 0,
                    zIndex: 1000,
                    overflow: 'hidden',
                }}
            >
                <Sidebar darkTheme={darkTheme} onSelect={handleMenuSelect} />
                <ToggleThemeButton darkTheme={darkTheme} toggleTheme={toggleTheme} />
            </Sider>

            <Layout style={{ marginLeft: collapsed ? 80 : 250 }}>
                <Header
                    style={{
                        padding: 0,
                        position: 'fixed',
                        width: `calc(100% - ${collapsed ? 80 : 250}px)`,
                        zIndex: 1000,
                        backgroundColor: 'var(--header-background-color)',
                    }}
                >
                    <Button
                        type="text"
                        className="toggle"
                        onClick={() => setCollapsed(!collapsed)}
                        icon={collapsed ? <MenuUnfoldOutlined /> : <MenuFoldOutlined />}
                        style={{
                            backgroundColor: 'var(--navigation-button-color)',
                            border: '1px solid #d9d9d9',
                            padding: '8px 16px',
                            borderRadius: '4px',
                        }}
                    />
                </Header>

                <Content
                    style={{
                        padding: '24px',
                        marginTop: 64,
                        overflowY: 'auto',
                        height: 'calc(100vh - 64px)',
                        background: 'var(--background-color)',
                        color: 'var(--text-color)',
                    }}
                >
                    <Routes>
                        <Route path="/home" element={<HomePage onNavigate={handleMenuSelect} />} />
                        <Route path="/enrollment" element={<div>Enrollment</div>} />
                        <Route path="/all-olympiads" element={<PDFViewer />} />
                        <Route path="/documents" element={<div>Documents</div>} />
                        <Route path="/for-me" element={<div>For Me</div>} />
                        <Route path="/settings" element={<div>Settings Content</div>} />
                        <Route path="/my-profile" element={<Login />} />
                        <Route path="/reset-password" element={<PasswordReset />} />
                        <Route path="/" element={<HomePage onNavigate={handleMenuSelect} />} />
                    </Routes>
                </Content>
            </Layout>
        </Layout>
    );
}

export default App;
