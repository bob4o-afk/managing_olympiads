import React, { useState, useEffect, useContext } from "react";
import { Card, Switch, Button, Typography, Modal, notification } from "antd";
import { EyeOutlined, EyeInvisibleOutlined } from "@ant-design/icons";
import { UserSession } from "../types/Session";
import "./ui/Settings.css";
import { LanguageContext } from "../contexts/LanguageContext";
import { decryptSession } from "../utils/encryption";
import {
  getAuthGetOptions,
  getAuthPostOptions,
  getAuthPostOptionsNoBody,
} from "../config/apiConfig";
import { API_ROUTES } from "../config/api";

const { Title, Text } = Typography;

const Settings: React.FC = () => {
  const [notificationsEnabled, setNotificationsEnabled] =
    useState<boolean>(true);
  const [autoFillDocs, setAutoFillDocs] = useState<boolean>(true);
  const [session, setSession] = useState<UserSession | null>(null);
  const [loading, setLoading] = useState<boolean>(true);
  const [showPasswordModal, setShowPasswordModal] = useState<boolean>(false);
  const [currentPassword, setCurrentPassword] = useState<string>("");
  const [showPassword, setShowPassword] = useState<boolean>(false);
  const [error, setError] = useState<string | null>(null);

  const { locale } = useContext(LanguageContext);
  const isBG = locale.startsWith("bg");

  const validateSession = async () => {
    const storedSession = localStorage.getItem("userSession");
    const token = localStorage.getItem("authToken");

    if (storedSession && token) {
      try {
        const response = await fetch(
          API_ROUTES.validateToken,
          getAuthPostOptionsNoBody(token)
        );

        if (response.ok) {
          try {
            const parsedSession = decryptSession(storedSession);
            setSession(parsedSession);
          } catch (error) {
            console.error("Failed to decrypt session:", error);
            localStorage.removeItem("userSession");
            localStorage.removeItem("authToken");
            setSession(null);
          }
        } else {
          localStorage.removeItem("userSession");
          localStorage.removeItem("authToken");
          setSession(null);
        }
      } catch (error) {
        console.error("An error occurred while validating the session:", error);
        setSession(null);
      }
    } else {
      setSession(null);
    }
    setLoading(false);
  };

  useEffect(() => {
    validateSession();
  }, []);

  useEffect(() => {
    if (session) {
      const fetchUserData = async () => {
        try {
          const token = localStorage.getItem("authToken");
          const userResponse = await fetch(
            API_ROUTES.userById(session.userId),
            getAuthGetOptions(token ?? "")
          );

          if (!userResponse.ok) {
            notification.error({
              message: isBG ? "Грешка" : "Error",
              description: isBG
                ? "Неуспешно зареждане на потребител. Опитайте отново."
                : "Failed to fetch users. Please try again.",
            });
            return;
          }

          const user = await userResponse.json();

          if (user) {
            setNotificationsEnabled(
              user.notifications?.emailNotifications ?? true
            );
            setAutoFillDocs(user.personalSettings?.autoFilling ?? true);
          } else {
            notification.error({
              message: isBG ? "Грешка" : "Error",
              description: isBG
                ? "Потребителят не е намерен."
                : "User not found.",
            });
          }
        } catch (error) {
          console.error("An error occurred while fetching user data:", error);
          notification.error({
            message: isBG ? "Грешка" : "Error",
            description: isBG
              ? "Възникна грешка. Опитайте отново."
              : "An error occurred. Please try again.",
          });
        }
      };

      fetchUserData();
    }
  }, [session, isBG]);

  const handleSavePreferences = async () => {
    if (!session) {
      notification.error({
        message: isBG ? "Грешка" : "Error",
        description: isBG
          ? "Трябва да влезете, за да запазите настройките."
          : "You must be logged in to save preferences.",
      });
      return;
    }
    setShowPasswordModal(true);
  };

  const handlePasswordValidation = async () => {
    const token = localStorage.getItem("authToken");

    try {
      const response = await fetch(
        API_ROUTES.validatePassword,
        getAuthPostOptions(token ?? "", { password: currentPassword })
      );

      if (!response.ok) {
        setError(
          isBG
            ? "Невалидна парола. Опитайте отново."
            : "Invalid password. Please try again."
        );
        return;
      }

      setError(null);
      await savePreferences();
      setShowPasswordModal(false);
    } catch (error) {
      setError(
        isBG
          ? "Възникна грешка при проверката на паролата."
          : "An error occurred while validating your password."
      );
      console.error("Error validating password:", error);
    }
  };

  const savePreferences = async () => {
    try {
      const token = localStorage.getItem("authToken");

      if (session) {
        const userResponse = await fetch(
          API_ROUTES.userById(session.userId),
          getAuthGetOptions(token ?? "")
        );

        if (!userResponse.ok) {
          notification.error({
            message: isBG ? "Грешка" : "Error",
            description: isBG
              ? "Неуспешно зареждане на потребител. Опитайте отново."
              : "Failed to fetch users. Please try again.",
          });
          return;
        }

        const user = await userResponse.json();

        if (!user) {
          notification.error({
            message: isBG ? "Грешка" : "Error",
            description: isBG
              ? "Потребителят не е намерен."
              : "User not found.",
          });
          return;
        }

        const updatedUser = {
          ...user,
          Notifications: {
            ...user.Notifications,
            emailNotifications: notificationsEnabled,
          },
          PersonalSettings: {
            ...user.PersonalSettings,
            autoFilling: autoFillDocs,
          },
          Password: currentPassword,
        };

        const updateResponse = await fetch(
          API_ROUTES.userById(session.userId),
          {
            method: "PUT",
            headers: {
              "Content-Type": "application/json",
              Authorization: `Bearer ${token}`,
            },
            body: JSON.stringify(updatedUser),
          }
        );

        if (updateResponse.ok) {
          notification.success({
            message: isBG ? "Успешно" : "Success",
            description: isBG
              ? "Настройките са запазени успешно!"
              : "Preferences saved successfully!",
          });
        } else {
          notification.error({
            message: isBG ? "Грешка" : "Error",
            description: isBG
              ? "Неуспешно запазване на настройките. Опитайте отново."
              : "Failed to save preferences. Please try again.",
          });
        }
      }
    } catch (error) {
      console.error("An error occurred while saving preferences:", error);
      alert(
        isBG
          ? "Възникна грешка. Опитайте отново."
          : "An error occurred. Please try again."
      );
    }
  };

  const handleResetPreferences = () => {
    setNotificationsEnabled(true);
    setAutoFillDocs(true);
  };

  if (loading) {
    return <p>{isBG ? "Зареждане на сесия..." : "Loading session..."}</p>;
  }

  if (!session) {
    return (
      <div style={{ padding: "24px", maxWidth: "600px", margin: "auto" }}>
        <Card
          style={{
            backgroundColor: "white",
            padding: "20px",
            borderRadius: "8px",
          }}
        >
          <Text className="text-card">
            {isBG
              ? "Трябва да влезете в профила си, за да достъпите настройките."
              : "You must be logged in to access the settings page."}
          </Text>
        </Card>
      </div>
    );
  }

  return (
    <div style={{ padding: "24px", maxWidth: "600px", margin: "auto" }}>
      <Card style={{ marginBottom: "16px" }}>
        <Title level={4}>{isBG ? "Известия" : "Notifications"}</Title>
        <Text className="text">
          {isBG
            ? "Получавайте известия за предстоящи олимпиади и други новини."
            : "Receive notifications about upcoming Olympiads and other updates."}
        </Text>
        <div
          style={{
            marginTop: "8px",
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
          }}
        >
          <Switch
            checked={notificationsEnabled}
            onChange={(checked) => setNotificationsEnabled(checked)}
            style={{ marginRight: "8px", fontSize: "14px", width: "40px" }}
          />
          <Text className="text">
            {notificationsEnabled
              ? isBG
                ? "Активирани"
                : "Enabled"
              : isBG
              ? "Изключени"
              : "Disabled"}
          </Text>
        </div>
      </Card>

      <Card style={{ marginBottom: "16px" }}>
        <Title level={4}>
          {isBG ? "Автоматично попълване" : "Auto-fill Documents"}
        </Title>
        <Text className="text">
          {isBG
            ? "Разрешете автоматично попълване на документи при регистрация за олимпиади."
            : "Enable auto-fill for document fields to save time during Olympiad registration."}
        </Text>
        <div
          style={{
            marginTop: "8px",
            display: "flex",
            alignItems: "center",
            justifyContent: "center",
          }}
        >
          <Switch
            checked={autoFillDocs}
            onChange={(checked) => setAutoFillDocs(checked)}
            style={{ marginRight: "8px", width: "40px" }}
          />
          <Text className="text">
            {autoFillDocs
              ? isBG
                ? "Активирано"
                : "Enabled"
              : isBG
              ? "Изключено"
              : "Disabled"}
          </Text>
        </div>
      </Card>

      <div className="button-container">
        <Button
          className="button"
          onClick={handleSavePreferences}
          style={{ width: "180px", height: "45px" }}
        >
          {isBG ? "Запази настройките" : "Save Preferences"}
        </Button>
        <Button
          className="cancel-button"
          onClick={handleResetPreferences}
          style={{ width: "180px", height: "45px" }}
        >
          {isBG ? "Нулирай настройките" : "Reset Preferences"}
        </Button>
      </div>

      <Modal
        title={isBG ? "Потвърдете паролата" : "Verify Password"}
        open={showPasswordModal}
        onCancel={() => setShowPasswordModal(false)}
        className="password-modal"
        footer={
          <div
            style={{ display: "flex", justifyContent: "center", gap: "16px" }}
          >
            <Button
              onClick={handlePasswordValidation}
              className="modal-button-submit"
            >
              {isBG ? "Потвърди" : "Verify"}
            </Button>
            <Button
              onClick={() => setShowPasswordModal(false)}
              className="modal-button-cancel"
            >
              {isBG ? "Отказ" : "Cancel"}
            </Button>
          </div>
        }
      >
        <div className="password-container">
          <div className="form-group">
            <label>{isBG ? "Парола" : "Password"}</label>
            <div className="password-input-wrapper">
              <input
                type={showPassword ? "text" : "password"}
                value={currentPassword}
                onChange={(e) => setCurrentPassword(e.target.value)}
                required
              />
              <span
                className="password-toggle"
                onClick={() => setShowPassword(!showPassword)}
              >
                {showPassword ? <EyeInvisibleOutlined /> : <EyeOutlined />}
              </span>
            </div>
          </div>
          {error && <Text className="error-text">{error}</Text>}
        </div>
      </Modal>
    </div>
  );
};

export default Settings;
