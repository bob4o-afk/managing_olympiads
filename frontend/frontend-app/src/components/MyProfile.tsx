import React, { useState, useEffect, useCallback } from "react";
import { Card, Button, Typography } from "antd";
import { useNavigate } from "react-router-dom";

const { Title, Text } = Typography;

const MyProfile: React.FC = () => {
    const [session, setSession] = useState<UserSession | null>(null);
    const [role, setRole] = useState<string>(""); 
    const [loading, setLoading] = useState<boolean>(true);
    const navigate = useNavigate();

    const handleLogout = useCallback(() => {
        localStorage.removeItem("authToken");
        localStorage.removeItem("userSession");
        setSession(null);
        setRole("");
        navigate("/login");
    }, [navigate]);

    const validateSession = useCallback(async () => {
        const storedSession = localStorage.getItem("userSession");
        const token = localStorage.getItem("authToken");

        if (storedSession && token) {
            try {
                const response = await fetch(`${process.env.REACT_APP_API_URL}/api/auth/validate-token`, {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                        Authorization: `Bearer ${token}`,
                    },
                });

                if (response.ok) {
                    return true;
                } else if (response.status === 401) {
                    console.warn("Session validation failed: Unauthorized. Logging out...");
                    handleLogout();
                } else if (response.status === 500) {
                    console.error("Server error occurred. Redirecting to login...");
                    handleLogout();
                } else {
                    console.warn(`Session validation failed: ${response.statusText} (${response.status}).`);
                    handleLogout();
                }
            } catch (error) {
                console.error("An error occurred during session validation:", error);
                handleLogout();
            }
        } else {
            console.warn("No stored session or token found. Redirecting to login...");
            navigate("/login");
        }
        return false;
    }, [handleLogout, navigate]);

    useEffect(() => {
        const initializeSession = async () => {
            const isValid = await validateSession();
            if (isValid) {
                const storedSession = localStorage.getItem("userSession");
                if (storedSession) {
                    const parsedSession = JSON.parse(storedSession);
                    setSession(parsedSession);
                    setRole(parsedSession.role);
                }
            }
            setLoading(false);
        };
        initializeSession();
    }, [validateSession]);

    const handleAction = useCallback(
        async (actionUrl: string) => {
            const isValid = await validateSession();
            if (isValid) {
                window.location.href = actionUrl;
            }
        },
        [validateSession]
    );

    if (loading) {
        return <p>Loading profile...</p>;
    }

    return (
        <div style={{ padding: "24px", maxWidth: "600px", margin: "auto" }}>
            {session ? (
                <>
                    <Card style={{ marginBottom: "16px", backgroundColor: "var(--card-background-color)" }}>
                        <Title style={{ color: "var(--text-color)" }} level={4}>
                            My Profile
                        </Title>
                        <Text style={{ color: "var(--text-color)" }}>
                            <strong>Name:</strong> {session.full_name || "N/A"}
                        </Text>
                        <br />
                        <Text style={{ color: "var(--text-color)" }}>
                            <strong>Email:</strong> {session.email}
                        </Text>
                        <br />
                        <Text style={{ color: "var(--text-color)" }}>
                            <strong>Role:</strong> {role || "Loading role..."}
                        </Text>
                    </Card>
                    <Card style={{ marginBottom: "16px", backgroundColor: "var(--card-background-color)" }}>
                        <Title style={{ color: "var(--text-color)" }} level={4}>
                            Account Management
                        </Title>
                        <Button
                            type="default"
                            style={{
                                marginBottom: "8px",
                                color: "var(--button-text-color)",
                                backgroundColor: "var(--button-background-color)",
                                border: "none",
                            }}
                            onClick={() => handleAction("/reset-password")}
                        >
                            Change Password
                        </Button>
                        <br />
                        <Button
                            type="default"
                            style={{
                                marginBottom: "8px",
                                color: "var(--button-text-color)",
                                backgroundColor: "var(--button-background-color)",
                                border: "none",
                            }}
                            onClick={() => handleAction("/update-info")}
                        >
                            Update Profile Information
                        </Button>
                        <br />
                        <Button
                            type="default"
                            style={{
                                marginBottom: "8px",
                                color: "var(--button-text-color)",
                                backgroundColor: "var(--button-background-color)",
                                border: "none",
                            }}
                            onClick={handleLogout}
                        >
                            Logout
                        </Button>
                    </Card>
                </>
            ) : (
                <p>Session expired. Redirecting...</p>
            )}
        </div>
    );
};

export default MyProfile;
