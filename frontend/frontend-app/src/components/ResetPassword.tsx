import React, { useState, useContext } from "react";
import { EyeOutlined, EyeInvisibleOutlined } from "@ant-design/icons";
import { useSearchParams, useNavigate } from "react-router-dom";
import { LanguageContext } from "../contexts/LanguageContext";
import { Button, Input, Typography, notification } from "antd";
import LoadingPage from "./LoadingPage";
import "./ui/ResetPassword.css";
import { defaultFetchOptions } from "../config/apiConfig";
import { API_ROUTES } from "../config/api";

const { Title } = Typography;

const ResetPassword: React.FC = () => {
  const [newPassword, setNewPassword] = useState<string>("");
  const [confirmPassword, setConfirmPassword] = useState<string>("");
  const [loading, setLoading] = useState<boolean>(false);

  const [searchParams] = useSearchParams();
  const token = searchParams.get("token");
  const navigate = useNavigate();

  const { locale } = useContext(LanguageContext);
  const isBG = locale.startsWith("bg");

  const handleUpdatePassword = async () => {
    setLoading(true);

    if (newPassword !== confirmPassword) {
      notification.error({
        message: isBG ? "Грешка" : "Error",
        description: isBG ? "Паролите не съвпадат." : "Passwords do not match.",
      });
      setLoading(false);
      return;
    }

    try {
      const response = await fetch(API_ROUTES.resetPasswordWithToken(token ?? ""), {
        ...defaultFetchOptions,
        body: JSON.stringify({ NewPassword: newPassword }),
      });

      if (!response.ok) {
        const { message } = await response.json();
        throw new Error(message);
      }

      notification.success({
        message: isBG ? "Успех" : "Success",
        description: isBG
          ? "Паролата е променена успешно!"
          : "Password updated successfully!",
      });

      setTimeout(() => {
        navigate("/my-profile");
      }, 2000);
    } catch (error) {
      notification.error({
        message: isBG ? "Грешка" : "Error",
        description:
          error instanceof Error
            ? `${
                isBG ? "Грешка при обновяване: " : "Error updating password: "
              }${error.message}`
            : isBG
            ? "Възникна неочаквана грешка."
            : "An unexpected error occurred.",
      });
    }

    setLoading(false);
  };

  const handleCancel = () => {
    navigate("/login");
  };

  return (
    <>
      {loading && <LoadingPage />}
      <div className="reset-password-container">
        <Title level={3}>
          {isBG ? "Промяна на парола" : "Update Password"}
        </Title>

        <div className="reset-password-form">
          <div className="password-container">
            <Input.Password
              className="password-input"
              value={newPassword}
              onChange={(e) => setNewPassword(e.target.value)}
              placeholder={isBG ? "Нова парола" : "New password"}
              iconRender={(visible) =>
                visible ? (
                  <EyeInvisibleOutlined
                    style={{ color: "var(--text-color)" }}
                  />
                ) : (
                  <EyeOutlined style={{ color: "var(--text-color)" }} />
                )
              }
            />
          </div>

          <div className="password-container">
            <Input.Password
              className="password-confirm"
              value={confirmPassword}
              onChange={(e) => setConfirmPassword(e.target.value)}
              placeholder={
                isBG ? "Потвърдете новата парола" : "Confirm new password"
              }
              iconRender={(visible) =>
                visible ? (
                  <EyeInvisibleOutlined
                    style={{ color: "var(--text-color)" }}
                  />
                ) : (
                  <EyeOutlined style={{ color: "var(--text-color)" }} />
                )
              }
            />
          </div>

          <Button
            className="button"
            onClick={handleUpdatePassword}
            loading={loading}
          >
            {loading
              ? isBG
                ? "Обновяване..."
                : "Updating..."
              : isBG
              ? "Обнови парола"
              : "Update Password"}
          </Button>

          <Button
            className="cancel-button"
            onClick={handleCancel}
            style={{ marginTop: "10px" }}
          >
            {isBG ? "Отказ" : "Cancel"}
          </Button>
        </div>
      </div>
    </>
  );
};

export default ResetPassword;
