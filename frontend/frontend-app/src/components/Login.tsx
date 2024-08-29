import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";

import { supabase } from "./supabaseClient";
import './ui/Login.css';

function Login(): JSX.Element {
    const [email, setEmail] = useState<string>("");
    const [password, setPassword] = useState<string>("");
    const [error, setError] = useState<string | null>(null);
    const [session, setSession] = useState<any>(null);
    const navigate = useNavigate();

    useEffect(() => {
        // Check localStorage for session data when the component mounts
        const storedSession = localStorage.getItem("supabaseSession");
        if (storedSession) {
            setSession(JSON.parse(storedSession));
        } else {
            // Fetch the session from Supabase if not found in localStorage
            const fetchSession = async () => {
                const { data: { session } } = await supabase.auth.getSession();
                if (session) {
                    setSession(session);
                    localStorage.setItem("supabaseSession", JSON.stringify(session));
                }
            };
            fetchSession();
        }
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
            localStorage.setItem("supabaseSession", JSON.stringify(data?.session));
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
            localStorage.removeItem("supabaseSession"); // Clear session from localStorage
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
