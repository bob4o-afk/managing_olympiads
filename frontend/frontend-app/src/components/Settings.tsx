import React, { useState, useEffect } from "react";
import { Card, Switch, Button, Typography, Modal, notification } from "antd";
import { HiEye, HiEyeOff } from "react-icons/hi";
import { UserSession } from "../types/Session";
import "./ui/Settings.css";

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

  const validateSession = async () => {
    const storedSession = localStorage.getItem("userSession");
    const token = localStorage.getItem("authToken");

    if (storedSession && token) {
      try {
        const response = await fetch(
          `${process.env.REACT_APP_API_URL}/api/auth/validate-token`,
          {
            method: "POST",
            headers: {
              "Content-Type": "application/json",
              Authorization: `Bearer ${token}`,
            },
          }
        );

        if (response.ok) {
          setSession(JSON.parse(storedSession));
        } else {
          console.warn("Session is invalid or expired. Logging out...");
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
          console.log(session);
          const token = localStorage.getItem("authToken");
          const userResponse = await fetch(
            `${process.env.REACT_APP_API_URL}/api/user/${session.userId}`,
            {
              method: "GET",
              headers: {
                "Content-Type": "application/json",
                Authorization: `Bearer ${token}`,
              },
            }
          );

          if (!userResponse.ok) {
            notification.error({
              message: "Error",
              description: "Failed to fetch users. Please try again.",
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
              message: "Error",
              description: "User not found.",
            });
          }
        } catch (error) {
          console.error("An error occurred while fetching user data:", error);
          notification.error({
            message: "Error",
            description: "An error occurred. Please try again.",
          });
        }
      };

      fetchUserData();
    }
  }, [session]);

  const handleSavePreferences = async () => {
    if (!session) {
      notification.error({
        message: "Error",
        description: "You must be logged in to save preferences.",
      });
      return;
    }
    setShowPasswordModal(true);
  };

  const handlePasswordValidation = async () => {
    const token = localStorage.getItem("authToken");

    try {
      const response = await fetch(
        `${process.env.REACT_APP_API_URL}/api/auth/validate-password`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
          },
          body: JSON.stringify({ password: currentPassword }),
        }
      );

      if (!response.ok) {
        setError("Invalid password. Please try again.");
        return;
      }

      setError(null);
      await savePreferences();
      setShowPasswordModal(false);
    } catch (error) {
      setError("An error occurred while validating your password.");
      console.error("Error validating password:", error);
    }
  };

  const savePreferences = async () => {
    try {
      const token = localStorage.getItem("authToken");

      if (session) {
        const userResponse = await fetch(
          `${process.env.REACT_APP_API_URL}/api/user/${session.userId}`,
          {
            method: "GET",
            headers: {
              "Content-Type": "application/json",
              Authorization: `Bearer ${token}`,
            },
          }
        );

        if (!userResponse.ok) {
          notification.error({
            message: "Error",
            description: "Failed to fetch users. Please try again.",
          });
          return;
        }

        const user = await userResponse.json();

        if (!user) {
          notification.error({
            message: "Error",
            description: "User not found.",
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
          `${process.env.REACT_APP_API_URL}/api/user/${user.userId}`,
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
            message: "Success",
            description: "Preferences saved successfully!",
          });
        } else {
          notification.error({
            message: "Error",
            description: "Failed to save preferences. Please try again.",
          });
        }
      }
    } catch (error) {
      console.error("An error occurred while saving preferences:", error);
      alert("An error occurred. Please try again.");
    }
  };

  const handleResetPreferences = () => {
    setNotificationsEnabled(true);
    setAutoFillDocs(true);
  };

  if (loading) {
    return <p>Loading session...</p>;
  }

  if (!session) {
    return (
      <div style={{ padding: "24px", maxWidth: "600px", margin: "auto" }}>
        <Card style={{ textAlign: "center", marginTop: "100px" }}>
          <Title level={4}>Please Log In</Title>
          <Text>You must be logged in to access the settings page.</Text>
        </Card>
      </div>
    );
  }

  return (
    <div style={{ padding: "24px", maxWidth: "600px", margin: "auto" }}>
      <Card style={{ marginBottom: "16px" }}>
        <Title level={4}>Notifications</Title>
        <Text>
          Receive notifications about upcoming Olympiads and other updates.
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
          <Text>{notificationsEnabled ? "Enabled" : "Disabled"}</Text>
        </div>
      </Card>
      <Card style={{ marginBottom: "16px" }}>
        <Title level={4}>Auto-fill Documents</Title>
        <Text>
          Enable auto-fill for document fields to save time during Olympiad
          registration.
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
            style={{ marginRight: "8px", fontSize: "14px", width: "40px" }}
          />
          <Text>{autoFillDocs ? "Enabled" : "Disabled"}</Text>
        </div>
      </Card>
      <div style={{ textAlign: "center", marginTop: "16px" }}>
        <Button
          type="primary"
          onClick={handleSavePreferences}
          style={{ marginRight: "8px", width: "150px" }}
        >
          Save Preferences
        </Button>
        <Button
          type="default"
          onClick={handleResetPreferences}
          style={{ width: "150px" }}
        >
          Reset Preferences
        </Button>
      </div>

      <Modal
        title="Verify Password"
        open={showPasswordModal}
        onCancel={() => setShowPasswordModal(false)}
        className="password-modal"
        footer={
          <div
            style={{ display: "flex", justifyContent: "center", gap: "16px" }}
          >
            <Button
              key="submit"
              onClick={handlePasswordValidation}
              className="modal-button-submit"
            >
              Verify
            </Button>
            <Button
              key="cancel"
              onClick={() => setShowPasswordModal(false)}
              className="modal-button-cancel"
            >
              Cancel
            </Button>
          </div>
        }
      >
        <div className="password-container">
          <div className="form-group">
            <label>Password</label>
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
                {showPassword ? <HiEyeOff /> : <HiEye />}
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
