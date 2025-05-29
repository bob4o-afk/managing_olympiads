import React, { useState, useContext } from "react";
import { useNavigate } from "react-router-dom";
import { Form, Input, Button, Typography, notification, Card } from "antd";
import "./ui/UpdateInfo.css";
import LoadingPage from "./LoadingPage";
import { LanguageContext } from "../contexts/LanguageContext";
import { UserSession } from "../types/Session";
import { decryptSession } from "../utils/encryption";
import { API_ROUTES } from "../config/api";
import { getAuthPatchOptions } from "../config/apiConfig";

const { Title } = Typography;

const UpdateInfo: React.FC = () => {
  const { locale } = useContext(LanguageContext);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [userSession, setUserSession] = useState<UserSession | null>(null);

  const [form] = Form.useForm();
  const navigate = useNavigate();

  const handleLogout = () => {
    localStorage.removeItem("authToken");
    localStorage.removeItem("userSession");
    navigate("/login");
  };

  const handleUpdateInfo = async (values: { name: string; email: string }) => {
    setIsLoading(true);
    const token = localStorage.getItem("authToken");
    const storedSession = localStorage.getItem("userSession");
    if (storedSession) {
      try {
        const userSession = decryptSession(storedSession);
        setUserSession(userSession);
      } catch (error) {
        console.error("Failed to decrypt session:", error);
      }
    }
    const userId = userSession?.userId;

    if (!token || !userId) {
      setError("User not authenticated.");
      setIsLoading(false);
      return;
    }

    try {
      const response = await fetch(
        API_ROUTES.userById(userId),
        getAuthPatchOptions(token, {
          name: values.name,
          email: values.email,
        })
      );

      if (!response.ok) {
        throw new Error("Failed to update user information.");
      }

      notification.success({
        message: "Success",
        description: "You will have to log in again.",
      });

      setTimeout(() => {
        handleLogout();
      }, 2000);
    } catch (err) {
      setError((err as Error).message);
    } finally {
      setIsLoading(false);
    }
  };

  const validateFullName = (_: any, value: string) => {
    if (!value) return Promise.resolve();
    const parts = value.trim().split(/\s+/);
    if (parts.length !== 3) {
      return Promise.reject(
        locale.startsWith("bg")
          ? "Моля, въведете три имена."
          : "Please enter your full name (first, middle, and last)."
      );
    }
    return Promise.resolve();
  };

  return (
    <>
      {isLoading && <LoadingPage />}
      <div className="update-info-container">
        <Card className="profile-card">
          <Title level={3} className="update-info-title">
            {locale.startsWith("bg")
              ? "Актуализиране на профилна информация"
              : "Update Profile Information"}
          </Title>

          <Form
            form={form}
            layout="vertical"
            onFinish={handleUpdateInfo}
            requiredMark={false}
          >
            <Form.Item
              label={locale.startsWith("bg") ? "Три имена" : "Full name"}
              name="name"
              rules={[
                {
                  required: true,
                  message: locale.startsWith("bg")
                    ? "Моля, въведете три имена."
                    : "Please enter your full name",
                },
                { validator: validateFullName },
              ]}
            >
              <Input className="ant-input" />
            </Form.Item>

            <Form.Item
              label={locale.startsWith("bg") ? "Имейл" : "Email"}
              name="email"
              rules={[
                {
                  required: true,
                  message: locale.startsWith("bg")
                    ? "Моля, въведете имейл."
                    : "Please enter your email",
                },
                {
                  type: "email",
                  message: locale.startsWith("bg")
                    ? "Моля, въведете валиден имейл."
                    : "Enter a valid email",
                },
              ]}
            >
              <Input className="ant-input" />
            </Form.Item>

            <Form.Item>
              <Button htmlType="submit" className="button" loading={isLoading}>
                {locale.startsWith("bg")
                  ? "Актуализирай информацията"
                  : "Update Information"}
              </Button>

              <Button
                htmlType="button"
                className="cancel-button"
                onClick={() => navigate(-1)}
              >
                {locale.startsWith("bg") ? "Връщане назад" : "Go back"}
              </Button>
            </Form.Item>

            {error && <p className="error-message">{error}</p>}
          </Form>
        </Card>
      </div>
    </>
  );
};

export default UpdateInfo;
