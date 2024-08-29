import React, { useState, useEffect } from 'react';
import { Card, Button, Typography } from 'antd';
import { supabase } from './supabaseClient';
import Login from './Login';

const { Title, Text } = Typography;

const MyProfile: React.FC = () => {
    const [session, setSession] = useState<any>(null);

    useEffect(() => {
        const fetchSession = async () => {
            const { data } = await supabase.auth.getSession();
            setSession(data?.session);
        };

        // Fetch session initially and set up a listener for session changes
        fetchSession();
        const { data: { subscription } } = supabase.auth.onAuthStateChange((_, session) => {
            setSession(session);
        });

        // Clean up the subscription when the component unmounts
        return () => {
            subscription?.unsubscribe();
        };
    }, []);

    return (
        <div style={{ padding: '24px', maxWidth: '600px', margin: 'auto' }}>
            {session ? (
                <>
                    <Card style={{ marginBottom: '16px', backgroundColor: 'var(--card-background-color)' }}>
                        <Title style={{ color: 'var(--text-color)' }} level={4}>My Profile</Title>
                        <Text style={{ color: 'var(--text-color)' }}>
                            <strong>Name:</strong> {session.user.user_metadata.full_name || 'N/A'}
                        </Text><br />
                        <Text style={{ color: 'var(--text-color)' }}>
                            <strong>Email:</strong> {session.user.email}
                        </Text><br />
                        <Text style={{ color: 'var(--text-color)' }}>
                            <strong>Role:</strong> {session.user.user_metadata.role || 'Student'}
                        </Text>
                    </Card>
                    <Card style={{ marginBottom: '16px', backgroundColor: 'var(--card-background-color)' }}>
                        <Title style={{ color: 'var(--text-color)' }} level={4}>Account Management</Title>
                        <Button
                            type="default"
                            style={{
                                marginBottom: '8px',
                                color: 'var(--button-text-color)',
                                backgroundColor: 'var(--button-background-color)',
                                border: 'none'
                            }}
                            onClick={() => window.location.href = 'http://localhost:3000/reset-password'}
                        >
                            Change Password
                        </Button><br />
                        <Button
                            type="default"
                            style={{
                                marginBottom: '8px',
                                color: 'var(--button-text-color)',
                                backgroundColor: 'var(--button-background-color)',
                                border: 'none'
                            }}
                            onClick={() => window.location.href = 'http://localhost:3000/update-info'}
                        >
                            Update Profile Information
                        </Button>
                    </Card>
                    <Login />
                </>
            ) : (
                <Login />
            )}
        </div>
    );
};

export default MyProfile;
