import { useState, FormEvent, useContext } from "react";
import { LanguageContext } from "../contexts/LanguageContext";
import { Button, Form, Input, notification, Typography } from "antd";
import "./ui/RequestPasswordReset.css";
import LoadingPage from "./LoadingPage";

const { Title } = Typography;

const RequestPasswordReset = () => {
  const [usernameOrEmail, setEmail] = useState("");
  const [loading, setLoading] = useState(false);

  const { locale } = useContext(LanguageContext);
  const isBG = locale.startsWith("bg");

  const handleResetPassword = async (e: FormEvent) => {
    setLoading(true);

    try {
      const response = await fetch(
        `${process.env.REACT_APP_API_URL}/api/auth/request-password-change`,
        {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          body: JSON.stringify({ usernameOrEmail }),
        }
      );

      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(
          errorData.message ||
            (isBG
              ? "Възникна грешка. Опитайте отново."
              : "An error occurred. Please try again.")
        );
      }

      notification.success({
        message: isBG ? "Успех" : "Success",
        description: isBG
          ? "Инструкциите за повторно задаване на парола са изпратени на вашия имейл."
          : "Password reset instructions sent to your email.",
      });
    } catch (error) {
      notification.error({
        message: isBG ? "Грешка" : "Error",
        description:
          error instanceof Error
            ? error.message
            : isBG
            ? "Възникна неочаквана грешка."
            : "An unexpected error occurred.",
      });
    } finally {
      setLoading(false);
    }
  };

  return (
    <>
      {loading && <LoadingPage />}

      <div className="reset-password-container">
        <Title level={3}>
          {isBG ? "Заявка за нова парола" : "Reset Password"}
        </Title>
        <Form
          layout="vertical"
          onFinish={handleResetPassword}
          className="reset-password-form"
        >
          <Form.Item
            name="usernameOrEmail"
            rules={[
              {
                required: true,
                message: isBG
                  ? "Моля, въведете имейл"
                  : "Please enter your email",
              },
            ]}
          >
            <Input
              type="email"
              placeholder={isBG ? "Въведете имейл" : "Enter your email"}
              value={usernameOrEmail}
              onChange={(e) => setEmail(e.target.value)}
              style={{
                backgroundColor: "var(--background-color)",
                color: "var(--text-color)",
                borderRadius: "4px",
                padding: "10px",
              }}
            />
          </Form.Item>

          <Form.Item>
            <Button
              className="button"
              htmlType="submit"
              loading={loading}
            >
              {loading
                ? isBG
                  ? "Изпращане..."
                  : "Sending..."
                : isBG
                ? "Изпрати имейл за нулиране"
                : "Send Password Reset Email"}
            </Button>
          </Form.Item>
        </Form>
      </div>
    </>
  );
};

export default RequestPasswordReset;
