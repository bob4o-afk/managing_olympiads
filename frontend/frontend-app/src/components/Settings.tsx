import React, { useState } from 'react';
import { Card, Switch, Button, Typography } from 'antd';

const { Title, Text } = Typography;

const Settings: React.FC = () => {
    const [notificationsEnabled, setNotificationsEnabled] = useState<boolean>(true);
    const [autoFillDocs, setAutoFillDocs] = useState<boolean>(true);

    const handleNotificationsToggle = (checked: boolean) => {
        setNotificationsEnabled(checked);
    };

    const handleAutoFillToggle = (checked: boolean) => {
        setAutoFillDocs(checked);
    };

    const handleResetPreferences = () => {
        setNotificationsEnabled(true);
        setAutoFillDocs(true);
    };

    return (
        <div style={{ padding: '24px', maxWidth: '600px', margin: 'auto' }}>
            <Card style={{ marginBottom: '16px' }}>
                <Title level={4}>Notifications</Title>
                <Text>Receive notifications about upcoming Olympiads and other updates.</Text>
                <div style={{ marginTop: '8px', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
                    <Switch
                        checked={notificationsEnabled}
                        onChange={handleNotificationsToggle}
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
                        onChange={handleAutoFillToggle}
                        style={{ marginRight: '8px', fontSize: '14px', width: '40px' }}
                    />
                    <Text>{autoFillDocs ? 'Enabled' : 'Disabled'}</Text>
                </div>
            </Card>
            <div style={{ textAlign: 'center' }}>
                <Button type="primary" onClick={handleResetPreferences} style={{ width: '150px' }}>
                    Reset Preferences
                </Button>
            </div>
        </div>
    );
};

export default Settings;
