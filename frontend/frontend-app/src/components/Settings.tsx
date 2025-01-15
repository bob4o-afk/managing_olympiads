import React, { useState, useEffect } from 'react';
import { Card, Switch, Button, Typography, Modal, Input } from 'antd';
import { HiEye, HiEyeOff } from 'react-icons/hi'; 

const { Title, Text } = Typography;

const Settings: React.FC = () => {
    const [notificationsEnabled, setNotificationsEnabled] = useState<boolean>(true);
    const [autoFillDocs, setAutoFillDocs] = useState<boolean>(true);
    const [session, setSession] = useState<any>(null);
    const [loading, setLoading] = useState<boolean>(true);
    const [showPasswordModal, setShowPasswordModal] = useState<boolean>(false);
    const [currentPassword, setCurrentPassword] = useState<string>('');
    const [showPassword, setShowPassword] = useState<boolean>(false);
    const [error, setError] = useState<string | null>(null);

    const validateSession = async () => {
        const storedSession = localStorage.getItem("userSession");
        const token = localStorage.getItem("authToken");

        if (storedSession && token) {
            try {
                const response = await fetch(`${process.env.REACT_APP_API_URL}/api/auth/validate-token`, {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                        "Authorization": `Bearer ${token}`,
                    },
                });

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
                    const token = localStorage.getItem("authToken");
                    const userResponse = await fetch(`${process.env.REACT_APP_API_URL}/api/user`, {
                        method: "GET",
                        headers: {
                            "Content-Type": "application/json",
                            "Authorization": `Bearer ${token}`,
                        },
                    });

                    if (!userResponse.ok) {
                        alert("Failed to fetch users. Please try again.");
                        return;
                    }

                    const users = await userResponse.json();
                    const user = users.find((u: { email: any; }) => u.email === session.email);

                    if (user) {
                        setNotificationsEnabled(user.notifications?.emailNotifications ?? true);
                        setAutoFillDocs(user.personalSettings?.autoFilling ?? true);
                    } else {
                        alert("User not found.");
                    }
                } catch (error) {
                    console.error("An error occurred while fetching user data:", error);
                    alert("An error occurred. Please try again.");
                }
            };

            fetchUserData();
        }
    }, [session]);

    const handleSavePreferences = async () => {
        if (!session) {
            alert("You must be logged in to save preferences.");
            return;
        }
        setShowPasswordModal(true);
    };

    const handlePasswordValidation = async () => {
        const token = localStorage.getItem("authToken");

        try {
            const response = await fetch(`${process.env.REACT_APP_API_URL}/api/auth/validate-password`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}`,
                },
                body: JSON.stringify({ password: currentPassword }),
            });

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

            const userResponse = await fetch(`${process.env.REACT_APP_API_URL}/api/user`, {
                method: "GET",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}`,
                },
            });

            if (!userResponse.ok) {
                alert("Failed to fetch users. Please try again.");
                return;
            }

            const users = await userResponse.json();
            const user = users.find((u: { email: any; }) => u.email === session.email);

            if (!user) {
                alert("User not found.");
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
                Password: currentPassword
            };

            const updateResponse = await fetch(`${process.env.REACT_APP_API_URL}/api/user/${user.userId}`, {
                method: "PUT",
                headers: {
                    "Content-Type": "application/json",
                    "Authorization": `Bearer ${token}`,
                },
                body: JSON.stringify(updatedUser),
            });

            if (updateResponse.ok) {
                alert("Preferences saved successfully!");
            } else {
                alert("Failed to save preferences. Please try again.");
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
            <div style={{ padding: '24px', maxWidth: '600px', margin: 'auto' }}>
                <Card style={{ textAlign: 'center', marginTop: '100px' }}>
                    <Title level={4}>Please Log In</Title>
                    <Text>You must be logged in to access the settings page.</Text>
                </Card>
            </div>
        );
    }

    return (
        <div style={{ padding: '24px', maxWidth: '600px', margin: 'auto' }}>
            <Card style={{ marginBottom: '16px' }}>
                <Title level={4}>Notifications</Title>
                <Text>Receive notifications about upcoming Olympiads and other updates.</Text>
                <div style={{ marginTop: '8px', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                    <Switch
                        checked={notificationsEnabled}
                        onChange={(checked) => setNotificationsEnabled(checked)}
                        style={{ marginRight: '8px', fontSize: '14px', width: '40px' }}
                    />
                    <Text>{notificationsEnabled ? 'Enabled' : 'Disabled'}</Text>
                </div>
            </Card>
            <Card style={{ marginBottom: '16px' }}>
                <Title level={4}>Auto-fill Documents</Title>
                <Text>Enable auto-fill for document fields to save time during Olympiad registration.</Text>
                <div style={{ marginTop: '8px', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                    <Switch
                        checked={autoFillDocs}
                        onChange={(checked) => setAutoFillDocs(checked)}
                        style={{ marginRight: '8px', fontSize: '14px', width: '40px' }}
                    />
                    <Text>{autoFillDocs ? 'Enabled' : 'Disabled'}</Text>
                </div>
            </Card>
            <div style={{ textAlign: 'center', marginTop: '16px' }}>
                <Button
                    type="primary"
                    onClick={handleSavePreferences}
                    style={{ marginRight: '8px', width: '150px' }}
                >
                    Save Preferences
                </Button>
                <Button
                    type="default"
                    onClick={handleResetPreferences}
                    style={{ width: '150px' }}
                >
                    Reset Preferences
                </Button>
            </div>

            <Modal
                title="Verify Password"
                open={showPasswordModal}
                onCancel={() => setShowPasswordModal(false)}
                footer={[
                    <Button key="submit" type="primary" onClick={handlePasswordValidation} style={{ 
                        width: '150px', margin: '0 auto', display: 'flex', justifyContent: 'center', alignItems: 'center', textAlign: 'center', marginBottom: '10px' }}>
                        Verify
                    </Button>, 
                    <Button key="cancel" onClick={() => setShowPasswordModal(false)} style={{ 
                        width: '150px', margin: '0 auto', display: 'flex', justifyContent: 'center', alignItems: 'center', textAlign: 'center' }}>
                        Cancel
                    </Button>
                ]}
            >
                <div className="password-container">
                    <Input
                        type={showPassword ? 'text' : 'password'}
                        placeholder="Enter your password"
                        value={currentPassword}
                        onChange={(e) => setCurrentPassword(e.target.value)}
                    />
                    <span
                        className="password-toggle"
                        onClick={() => setShowPassword(!showPassword)}
                        style={{ cursor: 'pointer' }}
                    >
                        {showPassword ? <HiEyeOff /> : <HiEye />}
                    </span>
                </div>

                {error && <Text style={{ color: 'black' }} type="danger">{error}</Text>}
            </Modal>

        </div>
    );
};

export default Settings;
