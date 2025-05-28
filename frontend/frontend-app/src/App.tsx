import { useState, useEffect, useContext } from "react";
import { Button, Layout, Drawer, Grid } from "antd";
import { MenuUnfoldOutlined } from "@ant-design/icons";
import { Route, Routes, useNavigate } from "react-router-dom";
import "./App.css";

import Sidebar from "./components/Sidebar";
import ToggleThemeButton from "./components/ToggleThemeButton";
import ToggleLanguageButton from "./components/ToggleLanguageButton";
import HomePage from "./components/HomePage";
import PDFViewer from "./components/PDFViewer";
import CVTemplate from "./components/CVTemplate";
import Settings from "./components/Settings";
import MyProfile from "./components/MyProfile";
import UpdateInfo from "./components/UpdateInfo";
import EnrollmentPage from "./components/EnrollmentPage";
import ResetPassword from "./components/ResetPassword";
import RequestPasswordReset from "./components/RequestPasswordReset";
import Login from "./components/Login";
import Documents from "./components/Documents";
import StudentOlympiadEnrollments from "./components/StudentOlympiadEnrollments";
import OlympiadsAnimation from "./components/OlympiadsAnimation";
import { SESSION_KEY, generateSessionId } from "./constants/storage";
import { LanguageContext } from "./contexts/LanguageContext";

const { Sider, Header, Content } = Layout;
const { useBreakpoint } = Grid;

function App() {
  const [collapsed, setCollapsed] = useState(true);
  const [drawerVisible, setDrawerVisible] = useState(false);
  const [, setSelectedKey] = useState("home");
  const navigate = useNavigate();
  const [showAnimation, setShowAnimation] = useState(true);
  const screens = useBreakpoint();
  const isMobile = !screens.md;

  const [darkTheme, setDarkTheme] = useState(() => {
    const savedTheme = localStorage.getItem("theme");
    return savedTheme ? savedTheme === "dark" : true;
  });

  const { locale } = useContext(LanguageContext);
  const isBG = locale.startsWith("bg");

  const [showDataNotice, setShowDataNotice] = useState(false);

  const toggleTheme = () => {
    setDarkTheme((prevTheme) => {
      const newTheme = !prevTheme;
      localStorage.setItem("theme", newTheme ? "dark" : "light");
      return newTheme;
    });
  };

  useEffect(() => {
    const currentSession = sessionStorage.getItem(SESSION_KEY);
    if (!currentSession) {
      const newSession = generateSessionId();
      sessionStorage.setItem(SESSION_KEY, newSession);
      if (localStorage.getItem(SESSION_KEY) !== newSession) {
        const keepKeys = ["theme", "dataNoticeAccepted"];
        const allKeys = Object.keys(localStorage);

        allKeys.forEach((key) => {
          if (!keepKeys.includes(key)) {
            localStorage.removeItem(key);
          }
        });
        localStorage.setItem(SESSION_KEY, newSession);
      }
    }
    if (localStorage.getItem("animation")) {
      setShowAnimation(false);
    }
  }, []);

  useEffect(() => {
    if (!localStorage.getItem("dataNoticeAccepted")) {
      setShowDataNotice(true);
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
    document.body.className = darkTheme ? "dark-theme" : "light-theme";
  }, [darkTheme]);

  const handleMenuSelect = (key: string) => {
    setSelectedKey(key);
    navigate(`/${key}`);
    if (isMobile) setDrawerVisible(false);
  };

  return showAnimation ? (
    <OlympiadsAnimation />
  ) : (
    <Layout style={{ height: "100vh" }}>
      {isMobile ? null : (
        <Sider
          collapsed={collapsed}
          collapsible
          trigger={null}
          theme={darkTheme ? "dark" : "light"}
          className="sidebar"
          width={collapsed ? 80 : 250}
          style={{
            position: "fixed",
            height: "100vh",
            left: 0,
            top: 0,
            zIndex: 1000,
            display: "flex",
            flexDirection: "column",
          }}
        >
          <div style={{ flexGrow: 1, overflowY: "auto" }}>
            <Sidebar darkTheme={darkTheme} onSelect={handleMenuSelect} />
          </div>

          <div className="sidebar-bottom-buttons">
            <ToggleLanguageButton />

            <ToggleThemeButton
              darkTheme={darkTheme}
              toggleTheme={toggleTheme}
            />
          </div>
        </Sider>
      )}

      <Layout
        style={{ marginLeft: isMobile ? 0 : collapsed ? 80 : 250 }}
        className="layout-content"
      >
        <Header
          style={{
            padding: 0,
            position: "fixed",
            width: `calc(100% - ${isMobile ? 0 : collapsed ? 80 : 250}px)`,
            zIndex: 1000,
            backgroundColor: "var(--header-background-color)",
          }}
          className="header"
        >
          <Button
            type="text"
            className="toggle"
            onClick={() =>
              isMobile ? setDrawerVisible(true) : setCollapsed(!collapsed)
            }
            icon={<MenuUnfoldOutlined />}
            style={{
              backgroundColor: "var(--navigation-button-color)",
              border: "1px solid #d9d9d9",
              padding: "8px 16px",
              borderRadius: "4px",
              marginLeft: "16px",
            }}
          />
        </Header>

        {isMobile && (
          <Drawer
            placement="left"
            closable={false}
            onClose={() => setDrawerVisible(false)}
            open={drawerVisible}
            className="drawer"
            width="60vw"
          >
            <div
              style={{
                display: "flex",
                flexDirection: "column",
                height: "100%",
              }}
            >
              <Sidebar
                darkTheme={darkTheme}
                onSelect={handleMenuSelect}
                isMobile={isMobile}
                closeDrawer={() => setDrawerVisible(false)}
              />

              <div className="sidebar-bottom-buttons">
                <ToggleLanguageButton />
                <ToggleThemeButton
                  darkTheme={darkTheme}
                  toggleTheme={toggleTheme}
                />
              </div>
            </div>
          </Drawer>
        )}

        <Content
          style={{
            marginTop: 64,
            overflowY: "auto",
            height: "calc(100vh - 64px)",
            background: "var(--background-color)",
            color: "var(--text-color)",
          }}
        >
          <Routes>
            <Route
              path="/home"
              element={<HomePage onNavigate={handleMenuSelect} />}
            />
            <Route path="/enrollment" element={<EnrollmentPage />} />
            <Route
              path="/enrollments"
              element={<StudentOlympiadEnrollments />}
            />
            <Route path="/all-olympiads" element={<PDFViewer />} />
            <Route path="/documents" element={<Documents />} />
            <Route
              path="/for-me"
              element={
                <div>
                  <CVTemplate />
                </div>
              }
            />
            <Route path="/settings" element={<Settings />} />
            <Route path="/my-profile" element={<MyProfile />} />
            <Route
              path="/request-password-change"
              element={<RequestPasswordReset />}
            />
            <Route path="/reset-password" element={<ResetPassword />} />
            <Route path="/update-info" element={<UpdateInfo />} />
            <Route path="/login" element={<Login />} />
            <Route path="/animation" element={<OlympiadsAnimation />} />
            <Route
              path="/"
              element={<HomePage onNavigate={handleMenuSelect} />}
            />
          </Routes>
        </Content>
      </Layout>
      {showDataNotice && (
        <div className="data-notice">
          <p>
            {isBG
              ? "Този сайт събира и съхранява Ваши данни с цел подобряване на функционалността и потребителското изживяване. Използвайки сайта, Вие се съгласявате с това."
              : "This site collects and stores your data to improve functionality and user experience. By using the site, you consent to this."}
          </p>
          <Button
            type="primary"
            style={{
              width: "80%",
              maxWidth: "300px",
              minWidth: "180px",
              marginTop: "8px",
            }}
            onClick={() => {
              localStorage.setItem("dataNoticeAccepted", "true");
              setShowDataNotice(false);
            }}
          >
            {isBG ? "Разбрах и приемам" : "Understood and accepted"}
          </Button>
        </div>
      )}
    </Layout>
  );
}

export default App;
