import React, { useEffect, useState } from 'react';
import { supabase } from './supabaseClient';

const PasswordReset: React.FC = () => {
    const [email, setEmail] = useState<string>('');
    const [loading, setLoading] = useState<boolean>(false);

    // Function to send a password reset link
    const handlePasswordReset = async () => {
        setLoading(true);
        const { data, error } = await supabase.auth.resetPasswordForEmail(email);

        if (error) {
            alert(`Error sending password reset email: ${error.message}`);
        } else {
            alert('Password reset email sent!');
        }
        setLoading(false);
    };

    // Effect to handle password reset after redirection
    useEffect(() => {
        const handleAuthStateChange = async (event: string, session: any) => {
            if (event === 'PASSWORD_RECOVERY') {
                const newPassword = prompt('What would you like your new password to be?');
                if (newPassword) {
                    const { data, error } = await supabase.auth.updateUser({ password: newPassword });

                    if (data) {
                        alert('Password updated successfully!');
                    }
                    if (error) {
                        alert(`Error updating password: ${error.message}`);
                    }
                }
            }
        };

        // Listen to auth state changes
        const { data: { subscription } } = supabase.auth.onAuthStateChange(handleAuthStateChange);

        // Cleanup subscription on component unmount
        return () => {
            subscription?.unsubscribe();
        };
    }, []);

    return (
        <div>
            <h1>Reset Password</h1>
            <input
                type="email"
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                placeholder="Enter your email"
            />
            <button onClick={handlePasswordReset} disabled={loading}>
                {loading ? 'Sending...' : 'Send Password Reset Email'}
            </button>
        </div>
    );
};

export default PasswordReset;
