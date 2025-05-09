import React, { useState, useEffect } from 'react';
import { Button, Layout } from 'antd';
import { MenuUnfoldOutlined, MenuFoldOutlined } from '@ant-design/icons';
import { Route, Routes, useNavigate } from 'react-router-dom';
import './App.css';

import Sidebar from './components/Sidebar';
import ToggleThemeButton from './components/ToggleThemeButton';
import HomePage from './components/HomePage';
import PDFViewer from './components/PDFViewer';
import CVTemplate from './components/CVTemplate';
import Settings from './components/Settings';
import MyProfile from './components/MyProfile';
import UpdateInfo from './components/UpdateInfo';
import EnrollmentPage from './components/EnrollmentPage';
import ResetPassword from './components/ResetPassword';
import RequestPasswordReset from './components/RequestPasswordReset';
import Login from './components/Login';
import Documents from './components/Documents';
import StudentOlympiadEnrollments from "./components/StudentOlympiadEnrollments";
import OlympiadsAnimation from './components/OlympiadsAnimation';
import { SESSION_KEY, generateSessionId } from "./constants/storage";


const { Sider, Header, Content } = Layout;

function App() {
    const [darkTheme, setDarkTheme] = useState(true);
    const [collapsed, setCollapsed] = useState(true);
    const [, setSelectedKey] = useState('home');
    const navigate = useNavigate();
    const [showAnimation, setShowAnimation] = useState(true);

    const toggleTheme = () => {
        setDarkTheme(!darkTheme);
    };


    useEffect(() => {
        setShowAnimation(false);

        const currentSession = sessionStorage.getItem(SESSION_KEY);
        if (!currentSession) {
            const newSession = generateSessionId();
            sessionStorage.setItem(SESSION_KEY, newSession);
    
            if (localStorage.getItem(SESSION_KEY) !== newSession) {
                localStorage.clear();
                localStorage.setItem(SESSION_KEY, newSession);
            }
        }
        if(localStorage.getItem("animation")){
            setShowAnimation(false);
        }
    }, []);
    
    useEffect(() => {
        if (showAnimation) {
            setTimeout(() => {
                localStorage.setItem("animation", "false");
                setShowAnimation(false);
            }, 12700);
        }
    }, [showAnimation]);


    useEffect(() => {
        document.body.className = darkTheme ? 'dark-theme' : 'light-theme';
    }, [darkTheme]);

    const handleMenuSelect = (key: string) => {
        setSelectedKey(key);
        navigate(`/${key}`);
    };

    return showAnimation ?
    (
        <OlympiadsAnimation />

    ):
    (
        <Layout style={{ height: '100vh' }}>
            <Sider
                collapsed={collapsed}
                collapsible
                trigger={null}
                theme={darkTheme ? 'dark' : 'light'}
                className="sidebar"
                width={collapsed ? 80 : 250}
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

            <Layout
                style={{
                    marginLeft: collapsed ? 80 : 250,
                }}
                className="layout-content"
            >
                <Header
                    style={{
                        padding: 0,
                        position: 'fixed',
                        width: `calc(100% - ${collapsed ? 80 : 250}px)`,
                        zIndex: 1000,
                        backgroundColor: 'var(--header-background-color)',
                    }}
                    className="header"
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
                        <Route path="/enrollment" element={<EnrollmentPage />} />
                        <Route path="/enrollments" element={<StudentOlympiadEnrollments />} />
                        <Route path="/all-olympiads" element={<PDFViewer />} />
                        <Route path="/documents" element={<Documents/>} />
                        <Route path="/for-me" element={<div><CVTemplate /></div>} />
                        <Route path="/settings" element={<Settings />} />
                        <Route path="/my-profile" element={<MyProfile />} />
                        <Route path="/request-password-change" element={<RequestPasswordReset />} />
                        <Route path="/reset-password" element={<ResetPassword />} />
                        <Route path="/update-info" element={<UpdateInfo />} />
                        <Route path="/login" element={<Login />} />
                        <Route path="/animation" element={<OlympiadsAnimation />} />
                        <Route path="/" element={<HomePage onNavigate={handleMenuSelect} />} />
                    </Routes>
                </Content>
            </Layout>
        </Layout>
    );
}

export default App;
