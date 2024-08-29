import React, { useState, useEffect } from "react";
import { supabase } from "./supabaseClient";
import { useNavigate } from "react-router-dom";
import './ui/Login.css'; // Import the CSS file

function Login(): JSX.Element {
    const [email, setEmail] = useState<string>("");
    const [password, setPassword] = useState<string>("");
    const [error, setError] = useState<string | null>(null);
    const [session, setSession] = useState<any>(null);
    // const [showRecovery, setShowRecovery] = useState<boolean>(false);
    // const [recoveryEmail, setRecoveryEmail] = useState<string>("");
    const navigate = useNavigate();

    useEffect(() => {
        // Check the current session when the component mounts
        const fetchSession = async () => {
            const { data: { session } } = await supabase.auth.getSession();
            setSession(session);
        };
        fetchSession();
    }, []);

    const handleLogin = async (e: React.FormEvent) => {
        e.preventDefault();
        const { data, error } = await supabase.auth.signInWithPassword({
            email,
            password,
        });

        if (error) {
            setError(error.message);
            console.error("Login error:", error.message);
        } else {
            setSession(data?.session);
            setError(null);
            console.log("Logged in:", data.user);
        }
    };

    const handleLogout = async () => {
        const { error } = await supabase.auth.signOut();
        if (error) {
            console.error("Logout error:", error.message);
        } else {
            setSession(null);
            console.log("Logged out successfully");
        }
    };

    const handlePasswordRecovery = () => {
        navigate('/reset-password');
    };

    return (
        <div className="login-container">
            {!session ? (
                <>
                    <h2>Login</h2>
                    <form onSubmit={handleLogin}>
                        <div className="form-group">
                            <label>Email</label>
                            <input
                                type="email"
                                value={email}
                                onChange={(e) => setEmail(e.target.value)}
                                required
                            />
                        </div>
                        <div className="form-group">
                            <label>Password</label>
                            <input
                                type="password"
                                value={password}
                                onChange={(e) => setPassword(e.target.value)}
                                required
                            />
                        </div>
                        <button type="submit">Login</button>
                    </form>
                    {error && <p className="error-message">{error}</p>}
                    <div className="recovery-section">
                        <button onClick={handlePasswordRecovery}>
                            Forgot Password?
                        </button>
                    </div>
                </>
            ) : (
                <div className="session-info">
                    <p>Logged in as: {session.user.email}</p>
                    <button onClick={handleLogout}>Logout</button>
                </div>
            )}
        </div>
    );
}

export default Login;
