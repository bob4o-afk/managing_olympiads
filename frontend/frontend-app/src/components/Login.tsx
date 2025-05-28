import React, { useState, useEffect, useContext, useCallback } from "react";
import { useNavigate } from "react-router-dom";
import { EyeOutlined, EyeInvisibleOutlined } from "@ant-design/icons";
import "./ui/Login.css";
import LoadingPage from "./LoadingPage";
import { Button, Form, Input, Typography } from "antd";
import { LanguageContext } from "../contexts/LanguageContext";

const { Title } = Typography;

const Login: React.FC = () => {
  const [error, setError] = useState<string | null>(null);
  const [token, setToken] = useState<string | null>(null);
  const [loading, setLoading] = useState<boolean>(false);
  const [form] = Form.useForm();

  const { locale } = useContext(LanguageContext);
  const isBG = locale.startsWith("bg");

  const navigate = useNavigate();

  const clearStorage = useCallback(() => {
    const keepKeys = ["theme", "dataNoticeAccepted"];
    const allKeys = Object.keys(localStorage);

    allKeys.forEach((key) => {
      if (!keepKeys.includes(key)) {
        localStorage.removeItem(key);
      }
    });
    setToken(null);
    form.resetFields();
    setError(null);
  }, [form]);

  useEffect(() => {
    const storedSession = localStorage.getItem("userSession");
    if (storedSession) {
      const parsedSession = JSON.parse(storedSession);
      if (parsedSession?.email) {
        setToken(parsedSession.email);
        navigate("/my-profile");
      } else {
        clearStorage();
      }
    } else {
      clearStorage();
    }
  }, [navigate, clearStorage]);

  const handleLogin = async (values: {
    usernameOrEmail: string;
    password: string;
  }) => {
    setLoading(true);
    setError(null);

    try {
      const authResponse = await fetch(
        `${process.env.REACT_APP_API_URL}/api/auth/login`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
          },
          body: JSON.stringify({
            usernameOrEmail: values.usernameOrEmail,
            password: values.password,
          }),
        }
      );

      if (!authResponse.ok) {
        const errorText = await authResponse.text();
        console.error("API Error Response:", errorText);
        throw new Error(
          "Login failed. Please check your credentials and try again."
        );
      }

      const authData = await authResponse.json();
      const token = authData.token;
      setToken(token);
      localStorage.setItem("authToken", token);

      const userDetails = authData.user;

      const roleResponse = await fetch(
        `${process.env.REACT_APP_API_URL}/api/UserRoleAssignment/`,
        {
          method: "GET",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
          },
        }
      );

      if (!roleResponse.ok) {
        const errorText = await roleResponse.text();
        console.error("Role Fetch Error:", errorText);
        throw new Error("Failed to retrieve user role.");
      }

      const roleData: Array<{ userId: string; role: { roleName: string } }> =
        await roleResponse.json();

      const userRoleAssignment = roleData.find(
        (assignment) => assignment.userId === userDetails.userId
      );
      const role = userRoleAssignment
        ? userRoleAssignment.role.roleName
        : "Student";

      const userSession = {
        userId: userDetails.userId,
        full_name: userDetails.name,
        email: userDetails.email,
        role: role,
      };

      localStorage.setItem("userSession", JSON.stringify(userSession));
      navigate("/my-profile");
    } catch (err: unknown) {
      const errorMessage =
        err instanceof Error ? err.message : "An unknown error occurred.";
      setError(errorMessage);
    } finally {
      setLoading(false);
    }
  };

  const handlePasswordRecovery = (): void => {
    navigate("/request-password-change");
  };

  return (
    <div className="login-container">
      {loading ? (
        <LoadingPage />
      ) : !token ? (
        <div className="login-form">
          <Title level={2}>{isBG ? "Вход в профила" : "Login"}</Title>

          <Form
            form={form}
            layout="vertical"
            onFinish={handleLogin}
            className="login-form"
          >
            <Form.Item
              label={isBG ? "Потребител или имейл" : "Username or Email"}
              name="usernameOrEmail"
              rules={[
                {
                  required: true,
                  message: isBG
                    ? "Моля, въведете потребител или имейл"
                    : "Please enter your username or email",
                },
              ]}
            >
              <Input />
            </Form.Item>

            <Form.Item
              label={isBG ? "Парола" : "Password"}
              name="password"
              rules={[
                {
                  required: true,
                  message: isBG
                    ? "Моля, въведете парола"
                    : "Please enter your password",
                },
              ]}
            >
              <Input.Password
                className="password-input"
                autoComplete="current-password"
                iconRender={(visible) =>
                  visible ? (
                    <EyeInvisibleOutlined style={{ color: "var(--text-color)" }} />
                  ) : (
                    <EyeOutlined style={{ color: "var(--text-color)" }} />
                  )
                }
              />
            </Form.Item>

            <Form.Item>
              <Button htmlType="submit" className="button">
                {isBG ? "Вход" : "Login"}
              </Button>
            </Form.Item>
          </Form>

          <div className="recovery-section">
            <Button type="link" onClick={handlePasswordRecovery}>
              {isBG ? "Забравена парола?" : "Forgot Password?"}
            </Button>
          </div>

          {error && <p className="error-message">{error}</p>}
        </div>
      ) : (
        <LoadingPage />
      )}
    </div>
  );
};

export default Login;
