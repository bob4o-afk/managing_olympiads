import React, { useEffect, useState } from 'react';
import { HiEye, HiEyeOff } from 'react-icons/hi';

import { supabase } from './supabaseClient';
import './ui/ResetPassword.css';


const PasswordReset: React.FC = () => {
    const [email, setEmail] = useState<string>('');
    const [newPassword, setNewPassword] = useState<string>('');
    const [confirmPassword, setConfirmPassword] = useState<string>('');
    const [loading, setLoading] = useState<boolean>(false);
    const [isPasswordReset, setIsPasswordReset] = useState<boolean>(false);
    const [message, setMessage] = useState<{ text: string; type: 'error' | 'success' } | null>(null);
    const [showNewPassword, setShowNewPassword] = useState<boolean>(false);
    const [showConfirmPassword, setShowConfirmPassword] = useState<boolean>(false);

    // Function to send a password reset link
    const handlePasswordReset = async () => {
        setLoading(true);
        setMessage(null);
        const { error } = await supabase.auth.resetPasswordForEmail(email, {
            redirectTo: `${window.location.origin}/reset-password` // Redirect to /reset-password
        });


        if (error) {
            setMessage({ text: `Error sending password reset email: ${error.message}`, type: 'error' });
        } else {
            setMessage({ text: 'Password reset email sent!', type: 'success' });
        }

        setLoading(false);
    };

    // Effect to handle password reset after redirection
    useEffect(() => {
        const handleAuthStateChange = async (event: string, session: any) => {
            if (event === 'PASSWORD_RECOVERY') {
                setIsPasswordReset(true);
            }
        };

        // Listen to auth state changes
        const { data: { subscription } } = supabase.auth.onAuthStateChange(handleAuthStateChange);

        // Cleanup subscription on component unmount
        return () => {
            subscription?.unsubscribe();
        };
    }, []);

    const handleUpdatePassword = async () => {
        setLoading(true);
        setMessage(null); // Clear previous messages

        if (newPassword !== confirmPassword) {
            setMessage({ text: 'Passwords do not match', type: 'error' });
            setLoading(false);
            return;
        }

        const { error } = await supabase.auth.updateUser({ password: newPassword });

        if (error) {
            setMessage({ text: `Error updating password: ${error.message}`, type: 'error' });
        } else {
            setMessage({ text: 'Password updated successfully!', type: 'success' });
            setTimeout(() => {
                window.location.href = 'http://localhost:3000/'; // Redirect to home or login page
            }, 2000); // Optional delay before redirect
        }

        setLoading(false);
    };

    return (
        <div className="container-reset-password">
            <h1>{isPasswordReset ? 'Update Password' : 'Reset Password'}</h1>

            {!isPasswordReset ? (
                <>
                    <input
                        type="email"
                        value={email}
                        onChange={(e) => setEmail(e.target.value)}
                        placeholder="Enter your email"
                    />
                    <button onClick={handlePasswordReset} disabled={loading}>
                        {loading ? 'Sending...' : 'Send Password Reset Email'}
                    </button>
                </>
            ) : (
                <>
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
                </>
            )}

            {message && (
                <div className={message.type === 'error' ? 'error-message' : 'success-message'}>
                    {message.text}
                </div>
            )}
        </div>
    );
};

export default PasswordReset;