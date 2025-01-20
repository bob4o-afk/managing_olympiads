import React, { useState } from 'react';
import { HiEye, HiEyeOff } from 'react-icons/hi';
import { useSearchParams, useNavigate } from "react-router-dom";
import './ui/ResetPassword.css';

const ResetPassword: React.FC = () => {
    const [newPassword, setNewPassword] = useState<string>('');
    const [confirmPassword, setConfirmPassword] = useState<string>('');
    const [loading, setLoading] = useState<boolean>(false);
    const [message, setMessage] = useState<{ text: string; type: 'error' | 'success' } | null>(null);
    const [showNewPassword, setShowNewPassword] = useState<boolean>(false);
    const [showConfirmPassword, setShowConfirmPassword] = useState<boolean>(false);
    const [searchParams] = useSearchParams();

    const navigate = useNavigate();
    const token = searchParams.get('token'); // Extract token from URL

    const handleUpdatePassword = async () => {
        setLoading(true);
        setMessage(null);

        if (newPassword !== confirmPassword) {
            setMessage({ text: 'Passwords do not match', type: 'error' });
            setLoading(false);
            return;
        }

        try {
            const response = await fetch(`${process.env.REACT_APP_API_URL}/api/auth/reset-password?token=${token}`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ NewPassword: newPassword }),
            });

            if (!response.ok) {
                const { message } = await response.json();
                throw new Error(message);
            }

            setMessage({ text: 'Password updated successfully!', type: 'success' });
            setTimeout(() => {
                navigate('/my-profile');
            }, 2000); 
        } catch (error: any) {
            setMessage({ text: `Error updating password: ${error.message}`, type: 'error' });
        }

        setLoading(false);
    };

    const handleCancel = () => {
        navigate('/login');
    };

    return (
        <div className="container-reset-password">
            <h1>Update Password</h1>
            <div className="password-container">
                <input
                    type={showNewPassword ? 'text' : 'password'}
                    value={newPassword}
                    onChange={(e) => setNewPassword(e.target.value)}
                    placeholder="Enter new password"
                />
                <span
                    className="password-toggle"
                    onClick={() => setShowNewPassword(!showNewPassword)}
                >
                    {showNewPassword ? <HiEyeOff /> : <HiEye />}
                </span>
            </div>
            <div className="password-container">
                <input
                    type={showConfirmPassword ? 'text' : 'password'}
                    value={confirmPassword}
                    onChange={(e) => setConfirmPassword(e.target.value)}
                    placeholder="Confirm new password"
                />
                <span
                    className="password-toggle"
                    onClick={() => setShowConfirmPassword(!showConfirmPassword)}
                >
                    {showConfirmPassword ? <HiEyeOff /> : <HiEye />}
                </span>
            </div>
            <button onClick={handleUpdatePassword} disabled={loading}>
                {loading ? 'Updating...' : 'Update Password'}
            </button>

            <button onClick={handleCancel} style={{ marginTop: '10px' }}>
                Cancel
            </button>

            {message && (
                <div className={message.type === 'error' ? 'error-message' : 'success-message'}>
                    {message.text}
                </div>
            )}
        </div>
    );
};

export default ResetPassword;
